using System.Collections.Generic;
using Sharp.Shared.Objects;

namespace NativeVoteManagerMS.Shared.Types;

public record VoteChoiceResult(
    VoteContent Content,
    IReadOnlyList<IGameClient> Voters
);

public record VoteResult(
    IReadOnlyList<VoteChoiceResult> Choices,
    IReadOnlyList<IGameClient> Participants,
    VoteContent? Winner
);
