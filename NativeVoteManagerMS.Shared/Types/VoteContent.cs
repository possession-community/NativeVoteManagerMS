namespace NativeVoteManagerMS.Shared.Types;

public record VoteContent
{
    public required int Index { get; init; }
    public required string InternalName { get; init; }
    public required LocalizedString VisibleName { get; init; }
    public LocalizedString? VisibleDescription { get; init; }
}
