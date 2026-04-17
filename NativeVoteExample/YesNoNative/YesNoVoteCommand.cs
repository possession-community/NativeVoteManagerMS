using NativeVoteManagerMS.Shared;
using NativeVoteManagerMS.Shared.Types;
using Sharp.Shared;
using Sharp.Shared.Enums;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace NativeVoteExample.YesNoNative;

public class YesNoVoteCommand(INativeVoteManager voteManager, ISharedSystem sharedSystem)
{
    public ECommandAction Execute(IGameClient executor, StringCommand command)
    {
        
        var yesNoOption = new YesNoVoteOptions
        {
            // SFUI key — CS2 engine translates client-side. Description is the '%s1%' substitution arg.
            Title = "#SFUI_vote_kick_player",
            Description = executor.Name,
            Participants = null,
            PassCondition = ConditionCheck,
            VoteDuration = 10.0F,
            VoteHandler = new YesNoVoteHandler(sharedSystem),
            VoteInitiator = executor.Slot
        };
        
        voteManager.InitiateYesNoVote(yesNoOption);
        return ECommandAction.Stopped;
    }

    private bool ConditionCheck(VoteResult result)
    {
        var voteYes =
            result.Choices[0].Voters.Count;
        var voteNo =
            result.Choices[1].Voters.Count;
        
        if (voteYes > voteNo)
            return true;
        
        return false;
    }
}