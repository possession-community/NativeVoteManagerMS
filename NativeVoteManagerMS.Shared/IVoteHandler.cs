using NativeVoteManagerMS.Shared.Types;
using Sharp.Shared.Objects;

namespace NativeVoteManagerMS.Shared;

/// <summary>
/// Handler of vote system
/// </summary>
public interface IVoteHandler
{
    void OnChoice(IGameClient chooser, VoteState voteState) {}
    void OnVoteInitiated(VoteOptions voteOptions) {}
    void OnVoteCancelled() {}
    void OnVotePassed(VoteResult result) {}
    void OnVoteFailed(VoteResult result) {}
}