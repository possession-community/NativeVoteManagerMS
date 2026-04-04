using System.Linq;

namespace NativeVoteManagerMS.Shared.Types;

public delegate bool VotePassCondition(VoteResult result);

public static class VotePassConditions
{
    public static VotePassCondition Default(float threshold = 0.5f, int minimumVoters = 0)
    {
        return result =>
        {
            if (result.Winner is null) return false;

            var totalVoters = result.Choices.Sum(c => c.Voters.Count);
            if (totalVoters < minimumVoters) return false;
            if (totalVoters == 0) return false;

            var winnerVoters = result.Choices.First(c => c.Content == result.Winner).Voters.Count;
            return (float)winnerVoters / totalVoters >= threshold;
        };
    }
}
