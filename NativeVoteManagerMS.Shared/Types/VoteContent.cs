namespace NativeVoteManagerMS.Shared.Types;

public record VoteContent(
    int Index,
    string InternalName,
    LocalizedString VisibleName,
    LocalizedString? VisibleDescription = null
);