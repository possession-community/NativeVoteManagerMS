using System;
using System.Collections.Generic;
using System.Linq;
using NativeVoteManagerMS.Shared;
using NativeVoteManagerMS.Shared.Types;
using Sharp.Shared.Objects;

namespace NativeVoteManagerMS.Handlers;

internal class MultiChoiceHandler : IVoteTypeHandler
{
    internal readonly IMenuCompat MenuCompat;
    private readonly MultiChoiceVoteOptions _options;
    private readonly Dictionary<VoteContent, List<IGameClient>> _votes = new();

    public MultiChoiceHandler(IMenuCompat menuCompat, MultiChoiceVoteOptions options)
    {
        MenuCompat = menuCompat;
        _options = options;
    }

    public float Duration => _options.VoteDuration;

    public void Start()
    {
        foreach (var content in _options.VoteContents)
        {
            _votes[content] = new List<IGameClient>();
        }

        MenuCompat.OnChoice = OnPlayerChoice;
        MenuCompat.SetVoteOptions(_options);
        foreach (var pa in _options.Participants!)
        {
            MenuCompat.OpenMenu(pa);
        }
        _options.VoteHandler.OnVoteInitiated();
    }

    private void OnPlayerChoice(IGameClient chooser, VoteContent content)
    {
        foreach (var voters in _votes.Values)
        {
            voters.Remove(chooser);
        }

        if (_votes.TryGetValue(content, out var list))
        {
            list.Add(chooser);
        }

        MenuCompat.CloseMenu(chooser);
        _options.VoteHandler.OnChoice(chooser, content, GetState());
    }


    public MultiChoiceVoteState GetState()
    {
        var choices = _votes
            .Select(kv => new VoteChoiceResult(kv.Key, kv.Value.AsReadOnly()))
            .ToList()
            .AsReadOnly();

        var votedCount = _votes.Values.Sum(v => v.Count);

        return new MultiChoiceVoteState(
            _options,
            choices,
            votedCount,
            _options.Participants!.Count
        );
    }

    public VoteResult BuildResult()
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

        return new VoteResult(choices, _options.Participants!, winner);
    }

    public bool CheckPassCondition(VoteResult result) =>
        (_options.PassCondition ?? VotePassConditions.Default())(result);

    public void OnVotePassed(VoteResult result) =>
        _options.VoteHandler.OnVotePassed(result);

    public void OnVoteFailed(VoteResult result) =>
        _options.VoteHandler.OnVoteFailed(result);

    public void OnVoteCancelled() =>
        _options.VoteHandler.OnVoteCancelled();

    public void Close()
    {
        if (_options.Participants is { } participants)
        {
            foreach (var participant in participants)
            {
                MenuCompat.CloseMenu(participant);
            }
        }
    }

    public void Cleanup()
    {
        MenuCompat.Cleanup();
        _votes.Clear();
    }
}
