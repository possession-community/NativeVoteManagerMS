using Sharp.Shared.Objects;

namespace NativeVoteManagerMS.Shared;

public interface IPermissionCompat
{
    bool HasPermission(IGameClient client, string permission);
}
