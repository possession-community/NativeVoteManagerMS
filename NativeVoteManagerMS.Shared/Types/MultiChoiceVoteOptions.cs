using System.Collections.Generic;
using Sharp.Shared.Objects;

namespace NativeVoteManagerMS.Shared.Types;

/// <summary>
/// Options for multi-choice vote
/// </summary>
/// <param name="Title">Title of this vote.</param>
/// <param name="Description">Description of this vote.</param>
/// <param name="VoteDuration">Duration of this vote. Set to below zero to never ends until INativeVoteManager.EndVote() called.</param>
/// <param name="VoteHandler">Handler of this vote</param>
/// <param name="VoteContents">Choices of this vote.</param>
/// <param name="PassCondition">Condition to determine if the vote passes. If null, defaults to majority vote.</param>
/// <param name="Participants">Custom participant list. If null, all connected players will be included as a participant.</param>
/// <param name="RandomShuffle">Whether to randomly shuffle the vote options.</param>
/// <param name="MenuOptions">Custom options for the menu compat implementation. Keys and values are implementation-specific.</param>
public record MultiChoiceVoteOptions(
    LocalizedString Title,
    LocalizedString Description,
    float VoteDuration,
    IMultiChoiceVoteHandler VoteHandler,
    IReadOnlyList<VoteContent> VoteContents,
    VotePassCondition? PassCondition = null,
    IReadOnlyList<IGameClient>? Participants = null,
    bool RandomShuffle = false,
    IReadOnlyDictionary<string, object>? MenuOptions = null
);
