using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NativeVoteManagerMS.Shared.Types;
using Sharp.Shared;
using Sharp.Shared.GameEntities;
using Sharp.Shared.Listeners;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace NativeVoteManagerMS.Handlers;

internal class NativeYesNoHandler : IVoteTypeHandler, IEventListener
{
    private readonly ISharedSystem _sharedSystem;
    private readonly ILogger _logger;
    private readonly YesNoVoteOptions _options;

    private readonly List<IGameClient> _yesVoters = new();
    private readonly List<IGameClient> _noVoters = new();

    private static readonly VoteContent YesContent = new(){Index = 0, InternalName = "yes", VisibleName = _ => "Yes"};
    private static readonly VoteContent NoContent = new(){ Index = 1, InternalName = "no", VisibleName = _ => "No"};
    
    private IGameEvent? _cachedVoteChangedEvent = null;

    public NativeYesNoHandler(ISharedSystem sharedSystem, ILogger logger, YesNoVoteOptions options)
    {
        _sharedSystem = sharedSystem;
        _logger = logger;
        _options = options;
    }

    public float Duration => _options.VoteDuration;
    
    private IVoteController? _voteController = null;

    public void Start()
    {
        _voteController = (IVoteController)_sharedSystem.GetEntityManager().FindEntityByClassname(null, "vote_controller")!;
        CleanupVoteController();
        
        _voteController.PotentialVotes = _options.Participants!.Count;
        
        _sharedSystem.GetEventManager().InstallEventListener(this);
        
        
        RefreshVotes();
        
        
        foreach (var participant in _options.Participants)
        {
            SendVoteStartUm(participant);
        }
        
        _options.VoteHandler.OnVoteInitiated();
    }

    public void OnVoteCast(IGameClient chooser, bool isYes)
    {
        _yesVoters.Remove(chooser);
        _noVoters.Remove(chooser);

        if (isYes)
            _yesVoters.Add(chooser);
        else
            _noVoters.Add(chooser);
        
        RefreshVotes();
        _options.VoteHandler.OnChoice(chooser, isYes, GetState());
    }

    public YesNoVoteState GetState()
    {
        return new YesNoVoteState(
            _options,
            _yesVoters.Count,
            _noVoters.Count,
            _yesVoters.Count + _noVoters.Count,
            _options.Participants!.Count
        );
    }

    public VoteResult BuildResult()
    {
        var choices = new List<VoteChoiceResult>
        {
            new(YesContent, _yesVoters.AsReadOnly()),
            new(NoContent, _noVoters.AsReadOnly())
        }.AsReadOnly();

        var winner = choices
            .Where(c => c.Voters.Count > 0)
            .OrderByDescending(c => c.Voters.Count)
            .FirstOrDefault()
            ?.Content;

        return new VoteResult(choices, _options.Participants!, winner);
    }

    public bool CheckPassCondition(VoteResult result) =>
        (_options.PassCondition ?? VotePassConditions.Default())(result);

    public void OnVotePassed(VoteResult result)
    {
        foreach (var participant in _options.Participants!)
        {
            SendVotePassedUm(participant);
        }
        _options.VoteHandler.OnVotePassed(result);
    }

    public void OnVoteFailed(VoteResult result)
    {
        foreach (var participant in _options.Participants!)
        {
            SendVoteFailedUm(participant);
        }
        _options.VoteHandler.OnVoteFailed(result);
    }

    public void OnVoteCancelled()
    {
        foreach (var participant in _options.Participants!)
        {
            SendVoteFailedUm(participant, NativeYesNoEndReason.NoReason);
        }
        _options.VoteHandler.OnVoteCancelled();
    }

    public void Close()
    {
    }

    public void Cleanup()
    {
        _sharedSystem.GetEventManager().RemoveEventListener(this);
        CleanupVoteController();
        _yesVoters.Clear();
        _noVoters.Clear();
    }

    private void CleanupVoteController()
    {
        if (_voteController == null)
            return;

        var cast = _voteController.GetVotesCast();
        for (int i = 0; i < _sharedSystem.GetModSharp().GetGlobals().MaxClients; i++)
        {
            cast[i] = (int)YesNoNativeVoteOption.VoteUncast;
        }

        var optionCount = _voteController.GetVoteOptionCount();
        for (int i = 0; i < 5; i++)
        {
            optionCount[i] = 0;
        }
    }

    private void RefreshVotes()
    {
        if(_voteController == null)
            return;

        var @event = GetVoteChangedEvent();

        var controllersOptionCount = _voteController.GetVoteOptionCount();
        
        @event.SetInt("vote_option1", controllersOptionCount[0]);
        @event.SetInt("vote_option2", controllersOptionCount[1]);
        @event.SetInt("vote_option3", controllersOptionCount[2]);
        @event.SetInt("vote_option4", controllersOptionCount[3]);
        @event.SetInt("vote_option5", controllersOptionCount[4]);
        @event.SetInt("potentialVotes", _options.Participants!.Count);
        
        @event.Fire(false);
    }

    private void SendVoteStartUm(IGameClient target, string dispStrOverride = "", string detailStrOverride = "")
    {
        var start = new CCSUsrMsg_VoteStart
        {
            Team = -1,
            PlayerSlot = _options.VoteInitiator,
            VoteType = -1,
            // #SFUI_Vote_None
            // #SFUI_vote_passed_nextlevel_extend
            DispStr = "#SFUI_Vote_None",
            DetailsStr = "",
            OtherTeamStr = "#SFUI_otherteam_vote_unimplemented",
            IsYesNoVote = true
        };
        
        
        if (dispStrOverride != "")
            start.DispStr = dispStrOverride;
        
        if (detailStrOverride != "")
            start.DetailsStr = detailStrOverride;

        _sharedSystem.GetModSharp().SendNetMessage(new RecipientFilter(target.GetPlayerController()!), start);
    }

    private void SendVoteFailedUm(IGameClient target, NativeYesNoEndReason reason = NativeYesNoEndReason.NotEnoughPlayersVoted)
    {
        var failed = new CCSUsrMsg_VoteFailed
        {
            Reason = (int)reason,
            Team = -1,
        };
        _sharedSystem.GetModSharp().SendNetMessage(new RecipientFilter(target.GetPlayerController()!), failed);
    }

    private void SendVotePassedUm(IGameClient target, string dispStrOverride = "", string detailStrOverride = "")
    {
        var passed = new CCSUsrMsg_VotePass()
        {
            Team = -1,
            VoteType = 2,
            DispStr = "#SFUI_vote_passed",
            DetailsStr = ""
        };
        
        if (dispStrOverride != "")
            passed.DispStr = dispStrOverride;
        
        if (detailStrOverride != "")
            passed.DetailsStr = detailStrOverride;

        _sharedSystem.GetModSharp().SendNetMessage(new RecipientFilter(target.GetPlayerController()!), passed);
    }
    
    
    
    
    
    
    
    
    private IGameEvent GetVoteChangedEvent()
    {
        if (_cachedVoteChangedEvent != null)
            return _cachedVoteChangedEvent;
        
        _cachedVoteChangedEvent = _sharedSystem.GetEventManager().CreateEvent("vote_changed", true);
        if (_cachedVoteChangedEvent is null)
            throw new InvalidOperationException("Event object should not be null, but it happened.");

        return _cachedVoteChangedEvent;
    }
    
    public void FireGameEvent(IGameEvent @event)
    {
        if (@event.Name != "vote_cast")
            return;

        var controller = @event.GetPlayerController("userid");
        
        if (controller == null)
            return;
        
        var gameClient = _sharedSystem.GetClientManager().GetGameClient(controller.PlayerSlot);
        
        if (gameClient == null)
            return;
        
        var option = @event.GetInt("vote_option");

        
        
        OnVoteCast(gameClient, option == (int)YesNoNativeVoteOption.VoteOption1);
    }

    public int ListenerVersion => 1;
    public int ListenerPriority => 99999;
}
