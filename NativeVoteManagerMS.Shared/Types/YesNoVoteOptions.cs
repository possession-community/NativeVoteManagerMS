using System.Collections.Generic;
using Sharp.Shared.Objects;

namespace NativeVoteManagerMS.Shared.Types;

/// <summary>
/// Options for native YesNo vote
/// </summary>
public record YesNoVoteOptions
{
    /// <summary>Title of this vote.</summary>
    public required LocalizedString Title { get; init; }

    /// <summary>Description of this vote.</summary>
    public required LocalizedString Description { get; init; }

    /// <summary>Duration of this vote. Set to below zero to never ends until INativeVoteManager.EndVote() called.</summary>
    public required float VoteDuration { get; init; }

    /// <summary>Handler of this vote.</summary>
    public required IYesNoVoteHandler VoteHandler { get; init; }

    /// <summary>Vote initiator slot. Defaults to 99 (server).</summary>
    public int VoteInitiator { get; init; } = 99;

    /// <summary>Condition to determine if the vote passes. If null, defaults to majority vote.</summary>
    public VotePassCondition? PassCondition { get; init; }

    /// <summary>Custom participant list. If null, all connected players will be included as a participant.</summary>
    public IReadOnlyList<IGameClient>? Participants { get; init; }
}
