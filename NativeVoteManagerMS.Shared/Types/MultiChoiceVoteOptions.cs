using System.Collections.Generic;
using Sharp.Shared.Objects;

namespace NativeVoteManagerMS.Shared.Types;

/// <summary>
/// Options for multi-choice vote
/// </summary>
public record MultiChoiceVoteOptions
{
    /// <summary>Title of this vote.</summary>
    public required LocalizedString Title { get; init; }

    /// <summary>Description of this vote.</summary>
    public required LocalizedString Description { get; init; }

    /// <summary>Duration of this vote. Set to below zero to never ends until INativeVoteManager.EndVote() called.</summary>
    public required float VoteDuration { get; init; }

    /// <summary>Handler of this vote.</summary>
    public required IMultiChoiceVoteHandler VoteHandler { get; init; }

    /// <summary>Choices of this vote.</summary>
    public required IReadOnlyList<VoteContent> VoteContents { get; init; }

    /// <summary>Condition to determine if the vote passes. If null, defaults to majority vote.</summary>
    public VotePassCondition? PassCondition { get; init; }

    /// <summary>Custom participant list. If null, all connected players will be included as a participant.</summary>
    public IReadOnlyList<IGameClient>? Participants { get; init; }

    /// <summary>Whether to randomly shuffle the vote options.</summary>
    public bool RandomShuffle { get; init; }

    /// <summary>Custom options for the menu compat implementation. Keys and values are implementation-specific.</summary>
    public IReadOnlyDictionary<string, object>? MenuOptions { get; init; }
}
