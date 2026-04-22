using NativeVoteManagerMS.Shared;
using NativeVoteManagerMS.Shared.Types;
using Sharp.Shared;
using Sharp.Shared.Objects;

namespace NativeVoteExample.Commands;

public class MultiChoiceVoteHandler(ISharedSystem sharedSystem): IMultiChoiceVoteHandler
{
    public void OnChoice(IGameClient chooser, VoteContent content, MultiChoiceVoteState state)
    {
        sharedSystem.GetModSharp().PrintToChatAll($"{chooser.Name} chose {content.VisibleName} | Voted/Participants {state.VotedCount}/{state.ParticipantCount}");
    }

    public void OnVoteInitiated()
    {
        sharedSystem.GetModSharp().PrintToChatAll("MultiChoiceVote Initialized");
    }

    public void OnVoteCancelled()
    {
        sharedSystem.GetModSharp().PrintToChatAll("MultiChoiceVote Cancelled");
    }

    public void OnVotePassed(VoteResult result)
    {
        sharedSystem.GetModSharp().PrintToChatAll("MultiChoiceVote Passed");
    }

    public void OnVoteFailed(VoteResult result)
    {
        sharedSystem.GetModSharp().PrintToChatAll("MultiChoiceVote Failed");
    }

    public void OnParticipantDisconnected(IGameClient client, MultiChoiceVoteState state)
    {
        sharedSystem.GetModSharp().PrintToChatAll($"{client.Name} disconnected | Voted/Participants {state.VotedCount}/{state.ParticipantCount}");
    }
}