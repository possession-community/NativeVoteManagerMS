using NativeVoteManagerMS.Shared;
using NativeVoteManagerMS.Shared.Types;
using Sharp.Shared;
using Sharp.Shared.Objects;

namespace NativeVoteExample.YesNoNative;

public class YesNoVoteHandler(ISharedSystem sharedSystem): IYesNoVoteHandler
{
    public void OnChoice(IGameClient chooser, bool isYes, YesNoVoteState state)
    {
        sharedSystem.GetModSharp().PrintToChatAll($"{chooser.Name} chose yes? {isYes}");
        sharedSystem.GetModSharp().PrintToChatAll($"Voted/Participants {state.VotedCount}/{state.ParticipantCount} | Yes/No  {state.YesCount}/{state.NoCount}");
    }

    public void OnVoteInitiated()
    {
        sharedSystem.GetModSharp().PrintToChatAll("NativeYesNoVote Initialized");
    }

    public void OnVoteCancelled()
    {
        sharedSystem.GetModSharp().PrintToChatAll("NativeYesNoVote Cancelled");
    }

    public void OnVotePassed(VoteResult result)
    {
        sharedSystem.GetModSharp().PrintToChatAll("NativeYesNoVote Passed");
    }

    public void OnVoteFailed(VoteResult result)
    {
        sharedSystem.GetModSharp().PrintToChatAll("NativeYesNoVote Failed");
    }

    public void OnParticipantDisconnected(IGameClient client, YesNoVoteState state)
    {
        sharedSystem.GetModSharp().PrintToChatAll($"{client.Name} disconnected | Voted/Participants {state.VotedCount}/{state.ParticipantCount}");
    }
}