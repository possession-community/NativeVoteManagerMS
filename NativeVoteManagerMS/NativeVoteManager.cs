using System;
using Microsoft.Extensions.Logging;
using NativeVoteManagerMS.Handlers;
using NativeVoteManagerMS.Shared;
using NativeVoteManagerMS.Shared.Types;
using Sharp.Shared;

namespace NativeVoteManagerMS;

public class NativeVoteManager(ISharedSystem sharedSystem, ILogger logger) : INativeVoteManager
{
    private IMenuCompat? _defaultMenuCompat;
    private IVoteTypeHandler? _activeHandler;
    private Guid? _voteTimerId;

    public void SetDefaultMenuCompat(IMenuCompat menuCompat)
    {
        _defaultMenuCompat = menuCompat;
        logger.LogInformation("Default menu compat has been set");
    }

    public void InitiateYesNoVote(YesNoVoteOptions options)
    {
        EnsureNoActiveVote();

        if (options.Participants is null)
        {
            options = options with
            {
                Participants = sharedSystem.GetModSharp().GetIServer().GetGameClients(true, true)
            };
        }

        var handler = new NativeYesNoHandler(sharedSystem, logger, options);
        StartVote(handler);
    }

    public void InitiateMultiChoiceVote(MultiChoiceVoteOptions options)
    {
        if (_defaultMenuCompat is null)
            throw new InvalidOperationException("Default menu compat is not set. Install a menu compat plugin or provide one via InitiateMultiChoiceVote overload.");

        InitiateMultiChoiceVote(options, _defaultMenuCompat);
    }

    public void InitiateMultiChoiceVote(MultiChoiceVoteOptions options, IMenuCompat customMenuCompat)
    {
        EnsureNoActiveVote();

        if (options.Participants is null)
        {
            options = options with
            {
                Participants = sharedSystem.GetModSharp().GetIServer().GetGameClients(true, true)
            };
        }

        var handler = new MultiChoiceHandler(customMenuCompat, options);
        StartVote(handler);
    }

    private void StartVote(IVoteTypeHandler handler)
    {
        _activeHandler = handler;
        handler.Start();

        if (handler.Duration > 0)
        {
            _voteTimerId = sharedSystem.GetModSharp().PushTimer(EndVote, handler.Duration);
        }
    }

    public bool IsAnyVoteInProgress => _activeHandler is not null;

    public YesNoVoteState? GetYesNoVoteState() =>
        (_activeHandler as NativeYesNoHandler)?.GetState();

    public MultiChoiceVoteState? GetMultiChoiceVoteState() =>
        (_activeHandler as MultiChoiceHandler)?.GetState();

    public void EndVote()
    {
        if (_activeHandler is null) return;

        StopTimer();

        var result = _activeHandler.BuildResult();
        var passed = _activeHandler.CheckPassCondition(result);

        if (passed)
        {
            _activeHandler.OnVotePassed(result);
        }
        else
        {
            _activeHandler.OnVoteFailed(result);
        }

        Cleanup();
    }

    public void CancelVote()
    {
        if (_activeHandler is null) return;

        StopTimer();
        _activeHandler.OnVoteCancelled();
        Cleanup();
    }

    private void EnsureNoActiveVote()
    {
        if (_activeHandler is not null)
            throw new InvalidOperationException("A vote is already in progress.");
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
        _activeHandler?.Close();
        _activeHandler?.Cleanup();
        _activeHandler = null;
    }
}
