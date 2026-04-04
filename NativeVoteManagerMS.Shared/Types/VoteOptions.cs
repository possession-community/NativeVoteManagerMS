using System.Collections.Generic;
using Sharp.Shared.Objects;

namespace NativeVoteManagerMS.Shared.Types;

/// <summary>
/// Options for vote
/// </summary>
/// <param name="Title">Title of this vote. Receives <see cref="System.Globalization.CultureInfo"/> for localization, null defaults to en-US.</param>
/// <param name="Description">Description of this vote. Receives <see cref="System.Globalization.CultureInfo"/> for localization, null defaults to en-US.</param>
/// <param name="VoteType">Native YesNo vote or menu type vote.</param>
/// <param name="VoteDuration">Duration of this vote. Set to below zero to never ends until INativeVoteManager.EndVote() called.</param>
/// <param name="VoteHandler">Handler of this vote</param>
/// <param name="VoteContents">Choices of this vote. Ignored for <see cref="VoteType.YesNoNative"/>.</param>
/// <param name="PassCondition">Condition to determine if the vote passes. Receives the <see cref="VoteResult"/> and returns true if passed.</param>
/// <param name="Participants">Custom participant list. If null, all connected players will be included as a participant.</param>
/// <param name="MenuOptions">Custom options for the menu compat implementation. Keys and values are implementation-specific.</param>
public record VoteOptions(
    LocalizedString Title,
    LocalizedString Description,
    VoteType VoteType,
    float VoteDuration,
    IVoteHandler VoteHandler,
    IReadOnlyList<VoteContent> VoteContents,
    VotePassCondition? PassCondition = null,
    IReadOnlyList<IGameClient>? Participants = null,
    bool RandomShuffle = false,
    IReadOnlyDictionary<string, object>? MenuOptions = null
);