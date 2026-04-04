using System;
using NativeVoteManagerMS.Shared.Types;
using Sharp.Shared.Objects;

namespace NativeVoteManagerMS.Shared;

public interface IMenuCompat
{
    void OpenMenu(IGameClient target);
    void CloseMenu(IGameClient target);

    void SetVoteOptions(VoteOptions options);

    /// <summary>
    /// Callback invoked when a player selects a vote option.
    /// Set by NativeVoteManager to bridge menu selection to vote processing.
    /// </summary>
    Action<IGameClient, VoteContent> OnChoice { get; set; }
}