using System.Collections.Generic;
using Sharp.Shared.Objects;

namespace NativeVoteManagerMS.Shared.Types;

/// <summary>
/// Options for native YesNo vote
/// </summary>
/// <param name="Title">Title of this vote.</param>
/// <param name="Description">Description of this vote.</param>
/// <param name="VoteDuration">Duration of this vote. Set to below zero to never ends until INativeVoteManager.EndVote() called.</param>
/// <param name="VoteHandler">Handler of this vote</param>
/// <param name="PassCondition">Condition to determine if the vote passes. If null, defaults to majority vote.</param>
/// <param name="Participants">Custom participant list. If null, all connected players will be included as a participant.</param>
public record YesNoVoteOptions(
    LocalizedString Title,
    LocalizedString Description,
    float VoteDuration,
    IYesNoVoteHandler VoteHandler,
    int VoteInitiator = 99,
    VotePassCondition? PassCondition = null,
    IReadOnlyList<IGameClient>? Participants = null
);
