using NativeVoteManagerMS.Shared.Types;

namespace NativeVoteManagerMS;

internal interface IVoteTypeHandler
{
    float Duration { get; }
    void Start();
    VoteResult BuildResult();
    bool CheckPassCondition(VoteResult result);
    void OnVotePassed(VoteResult result);
    void OnVoteFailed(VoteResult result);
    void OnVoteCancelled();
    void Close();
    void Cleanup();
}
