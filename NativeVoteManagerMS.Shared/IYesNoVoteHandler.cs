using NativeVoteManagerMS.Shared.Types;
using Sharp.Shared.Objects;

namespace NativeVoteManagerMS.Shared;

public interface IYesNoVoteHandler
{
    void OnChoice(IGameClient chooser, bool isYes, YesNoVoteState state) {}
    void OnVoteInitiated() {}
    void OnVoteCancelled() {}
    void OnVotePassed(VoteResult result) {}
    void OnVoteFailed(VoteResult result) {}
}
