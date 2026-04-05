using System.Collections.Generic;

namespace NativeVoteManagerMS.Shared.Types;

public record YesNoVoteState(
    YesNoVoteOptions Options,
    int YesCount,
    int NoCount,
    int VotedCount,
    int ParticipantCount
);

public record MultiChoiceVoteState(
    MultiChoiceVoteOptions Options,
    IReadOnlyList<VoteChoiceResult> CurrentChoices,
    int VotedCount,
    int ParticipantCount
);
