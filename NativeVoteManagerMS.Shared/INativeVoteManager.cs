using NativeVoteManagerMS.Shared.Types;

namespace NativeVoteManagerMS.Shared;

public interface INativeVoteManager
{
    const string ModSharpModuleIdentity = "NativeVoteManagerMS.Shared.INativeVoteManager";

    VoteInitiateResult InitiateYesNoVote(YesNoVoteOptions options);

    VoteInitiateResult InitiateMultiChoiceVote(MultiChoiceVoteOptions options);
    VoteInitiateResult InitiateMultiChoiceVote(MultiChoiceVoteOptions options, IMenuCompat customMenuCompat);

    VoteCancelResult CancelVote();
    VoteEndResult EndVote();

    bool IsAnyVoteInProgress { get; }
    YesNoVoteState? GetYesNoVoteState();
    MultiChoiceVoteState? GetMultiChoiceVoteState();

    void SetDefaultMenuCompat(IMenuCompat menuCompat);
}
