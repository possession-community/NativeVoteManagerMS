using NativeVoteManagerMS.Shared;
using Sharp.Modules.AdminManager.Shared;
using Sharp.Shared.Managers;
using Sharp.Shared.Objects;

namespace NvmFPMCompat;

public class FpmPermissionCompat(IAdminManager adminManager) : IPermissionCompat
{
    public bool HasPermission(IGameClient client, string permission)
    {
        var admin = adminManager.GetAdmin(client.SteamId);
        return admin is not null && admin.HasPermission(permission);
    }
}
