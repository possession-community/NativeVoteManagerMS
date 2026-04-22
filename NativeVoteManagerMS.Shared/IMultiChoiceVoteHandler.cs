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

    /// <summary>
    /// Called after a participant disconnects. The library has already removed the client
    /// from the participant and voter lists before this is invoked, and <paramref name="state"/>
    /// reflects the post-removal counts. Use this to apply policy decisions such as cancelling
    /// the vote when the remaining participant count drops below a threshold.
    /// </summary>
    void OnParticipantDisconnected(IGameClient client, MultiChoiceVoteState state) {}
}
