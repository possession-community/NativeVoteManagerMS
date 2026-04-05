using System.Collections.Generic;
using NativeVoteManagerMS.Shared;
using NativeVoteManagerMS.Shared.Types;
using Sharp.Shared;
using Sharp.Shared.Enums;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace NativeVoteExample.Commands;

public class MultiChoiceVoteCommand(INativeVoteManager voteManager, ISharedSystem sharedSystem)
{
    public ECommandAction Execute(IGameClient executor, StringCommand command)
    {
        if (command.ArgCount < 3)
        {
            executor.Print(HudPrintChannel.Chat, "Requires more than 2 choices");
            return ECommandAction.Stopped;
        }
        List<VoteContent> voteContents = new();

        for (int i = 2; i < command.ArgCount; i++)
        {
            var cacheI = i;
            if (i % 2 == 0)
            {
                voteContents.Add(new ()
                {
                    Index = cacheI,
                    InternalName = $"InternalIdentifier_{cacheI}",
                    VisibleName = _ => $"{command.GetArg(cacheI)}",
                    VisibleDescription = _ => $"VisibleDescription_{cacheI}"
                });
            }
            else
            {
                voteContents.Add(new ()
                {
                    Index = cacheI,
                    InternalName = $"InternalIdentifier_{cacheI}",
                    VisibleName = _ => $"{command.GetArg(cacheI)}",
                });
            }
            
        }
        
        var multiChoiceOption = new MultiChoiceVoteOptions
        {
            Title = _ => "Title",
            Description = _ => "Description",
            Participants = null,
            PassCondition = ConditionCheck,
            VoteDuration = 10.0F,
            VoteHandler = new MultiChoiceVoteHandler(sharedSystem),
            VoteContents = voteContents
        };
        
        voteManager.InitiateMultiChoiceVote(multiChoiceOption);
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