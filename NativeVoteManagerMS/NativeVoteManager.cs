using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NativeVoteManagerMS.Shared;
using NativeVoteManagerMS.Shared.Types;
using Sharp.Shared;
using Sharp.Shared.Objects;

namespace NativeVoteManagerMS;

public class NativeVoteManager(ISharedSystem sharedSystem, ILogger logger) : INativeVoteManager
{
    private IMenuCompat? _defaultMenuCompat;
    private IMenuCompat? _activeMenuCompat;
    private VoteOptions? _activeVoteOptions;
    private readonly Dictionary<VoteContent, List<IGameClient>> _votes = new();
    private Guid? _voteTimerId;

    public void SetDefaultMenuCompat(IMenuCompat menuCompat)
    {
        _defaultMenuCompat = menuCompat;
        logger.LogInformation("Default menu compat has been set");
    }

    public void InitiateVote(VoteOptions voteOptions)
    {
        if (_defaultMenuCompat is null)
            throw new InvalidOperationException("Default menu compat is not set. Install a menu compat plugin or provide one via InitiateVote overload.");

        InitiateVote(voteOptions, _defaultMenuCompat);
    }

    public void InitiateVote(VoteOptions voteOptions, IMenuCompat customMenuCompat)
    {
        if (_activeVoteOptions is not null)
            throw new InvalidOperationException("A vote is already in progress.");

        voteOptions = voteOptions with
        {
            Participants = voteOptions.Participants ?? sharedSystem.GetModSharp().GetIServer().GetGameClients(),
            PassCondition = voteOptions.PassCondition ?? VotePassConditions.Default()
        };

        _activeVoteOptions = voteOptions;
        _activeMenuCompat = customMenuCompat;
        _votes.Clear();

        foreach (var content in voteOptions.VoteContents)
        {
            _votes[content] = new List<IGameClient>();
        }

        customMenuCompat.OnChoice = OnPlayerChoice;
        customMenuCompat.SetVoteOptions(voteOptions);

        voteOptions.VoteHandler.OnVoteInitiated(voteOptions);

        if (voteOptions.VoteDuration > 0)
        {
            _voteTimerId = sharedSystem.GetModSharp().PushTimer(EndVote, voteOptions.VoteDuration);
        }
    }

    private void OnPlayerChoice(IGameClient chooser, VoteContent content)
    {
        if (_activeVoteOptions is null) return;

        foreach (var voters in _votes.Values)
        {
            voters.Remove(chooser);
        }

        if (_votes.TryGetValue(content, out var list))
        {
            list.Add(chooser);
        }

        _activeMenuCompat?.CloseMenu(chooser);
        _activeVoteOptions.VoteHandler.OnChoice(chooser, GetVoteState()!);
    }

    public bool IsAnyVoteInProgress => _activeVoteOptions is not null;

    public VoteState? GetVoteState()
    {
        if (_activeVoteOptions is null) return null;

        var choices = _votes
            .Select(kv => new VoteChoiceResult(kv.Key, kv.Value.AsReadOnly()))
            .ToList()
            .AsReadOnly();

        var votedCount = _votes.Values.Sum(v => v.Count);

        return new VoteState(
            _activeVoteOptions,
            choices,
            votedCount,
            _activeVoteOptions.Participants!.Count
        );
    }

    public void EndVote()
    {
        if (_activeVoteOptions is null) return;

        StopTimer();

        var result = BuildResult();
        var passCondition = _activeVoteOptions.PassCondition!;

        if (passCondition(result))
        {
            _activeVoteOptions.VoteHandler.OnVotePassed(result);
        }
        else
        {
            _activeVoteOptions.VoteHandler.OnVoteFailed(result);
        }

        Cleanup();
    }

    public void CancelVote()
    {
        if (_activeVoteOptions is null) return;

        StopTimer();
        _activeVoteOptions.VoteHandler.OnVoteCancelled();
        Cleanup();
    }

    private VoteResult BuildResult()
    {
        var choices = _votes
            .Select(kv => new VoteChoiceResult(kv.Key, kv.Value.AsReadOnly()))
            .ToList()
            .AsReadOnly();

        var winner = choices
            .Where(c => c.Voters.Count > 0)
            .OrderByDescending(c => c.Voters.Count)
            .FirstOrDefault()
            ?.Content;

        return new VoteResult(choices, _activeVoteOptions!.Participants!, winner);
    }

    private void StopTimer()
    {
        if (_voteTimerId is { } timerId)
        {
            sharedSystem.GetModSharp().StopTimer(timerId);
            _voteTimerId = null;
        }
    }

    private void Cleanup()
    {
        if (_activeMenuCompat is { } menu && _activeVoteOptions?.Participants is { } participants)
        {
            foreach (var participant in participants)
            {
                menu.CloseMenu(participant);
            }
        }

        _activeVoteOptions = null;
        _activeMenuCompat = null;
        _votes.Clear();
    }
}
