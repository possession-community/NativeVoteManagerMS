using NativeVoteManagerMS.Shared.Types;
using Sharp.Shared.Objects;

namespace NativeVoteManagerMS.Shared;

public interface IMultiChoiceVoteHandler
{
    void OnChoice(IGameClient chooser, VoteContent content, MultiChoiceVoteState state) {}
    void OnVoteInitiated() {}
    void OnVoteCancelled() {}
    void OnVotePassed(VoteResult result) {}
    void OnVoteFailed(VoteResult result) {}
}
