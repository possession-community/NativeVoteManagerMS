using System.Collections.Generic;
using System.Reflection;
using NativeVoteManagerMS.Shared.Types;

namespace NativeVoteManagerMS.Shared;

public interface INativeVoteManager
{
    const string ModSharpModuleIdentity = "NativeVoteManagerMS.Shared.INativeVoteManager";
    
    /// <summary>
    /// Initiate a vote with 
    /// </summary>
    /// <param name="voteOptions"></param>
    void InitiateVote(VoteOptions voteOptions);

    /// <summary>
    /// Initiate a vote with custom menu compat
    /// </summary>
    /// <param name="voteOptions"></param>
    /// <param name="customMenuCompat"></param>
    void InitiateVote(VoteOptions voteOptions, IMenuCompat customMenuCompat);
    
    void CancelVote();
    void EndVote();

    bool IsAnyVoteInProgress { get; }
    VoteState? GetVoteState();

    void SetDefaultMenuCompat(IMenuCompat customMenuCompat);
}