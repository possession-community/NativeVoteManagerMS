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
            Title = _ => "Title But will not used in YesNoNative",
            Description = _ => "Description But will not used in YesNoNative",
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