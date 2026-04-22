using NativeVoteManagerMS.Shared.Types;
using Sharp.Shared.Objects;

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
    void OnParticipantDisconnected(IGameClient client);
    void Close();
    void Cleanup();
}
