using NativeVoteManagerMS.Shared.Types;

namespace NativeVoteManagerMS.Shared;

public interface INativeVoteManager
{
    const string ModSharpModuleIdentity = "NativeVoteManagerMS.Shared.INativeVoteManager";

    void InitiateYesNoVote(YesNoVoteOptions options);

    void InitiateMultiChoiceVote(MultiChoiceVoteOptions options);
    void InitiateMultiChoiceVote(MultiChoiceVoteOptions options, IMenuCompat customMenuCompat);

    void CancelVote();
    void EndVote();

    bool IsAnyVoteInProgress { get; }
    YesNoVoteState? GetYesNoVoteState();
    MultiChoiceVoteState? GetMultiChoiceVoteState();

    void SetDefaultMenuCompat(IMenuCompat menuCompat);
}
