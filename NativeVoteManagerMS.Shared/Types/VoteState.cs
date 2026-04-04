using System.Collections.Generic;

namespace NativeVoteManagerMS.Shared.Types;

public record VoteState(
    VoteOptions Options,
    IReadOnlyList<VoteChoiceResult> CurrentChoices,
    int VotedCount,
    int ParticipantCount
);
