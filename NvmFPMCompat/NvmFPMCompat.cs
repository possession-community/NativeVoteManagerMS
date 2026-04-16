using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NativeVoteManagerMS.Shared;
using Sharp.Modules.AdminManager.Shared;
using Sharp.Modules.MenuManager.Shared;
using Sharp.Shared;

namespace NvmFPMCompat;

public class NvmFPMCompat : IModSharpModule
{
    public NvmFPMCompat(ISharedSystem  sharedSystem,
        string         dllPath,
        string         sharpPath,
        Version?       version,
        IConfiguration coreConfiguration,
        bool           hotReload)
    {
        ArgumentNullException.ThrowIfNull(dllPath);
        ArgumentNullException.ThrowIfNull(sharpPath);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(coreConfiguration);
        ArgumentNullException.ThrowIfNull(sharedSystem);
        _sharedSystem = sharedSystem;

        var factory = _sharedSystem.GetLoggerFactory();

        // ReSharper disable VirtualMemberCallInConstructor
        var logger = factory.CreateLogger(DisplayName);
        // ReSharper restore VirtualMemberCallInConstructor

        _logger = logger;
    }

    public string DisplayName => "NativeVoteManagerMS - FPMCompat";
    public string DisplayAuthor => "faketuna";

    private readonly ISharedSystem _sharedSystem;
    private readonly ILogger _logger;
    private INativeVoteManager _nativeVoteManager = null!;

    public bool Init()
    {
        return true;
    }

    public void PostInit()
    {
    }

    public void OnAllModulesLoaded()
    {
        _nativeVoteManager = _sharedSystem.GetSharpModuleManager()
            .GetRequiredSharpModuleInterface<INativeVoteManager>(INativeVoteManager.ModSharpModuleIdentity).Instance!;

        var menuManager = _sharedSystem.GetSharpModuleManager()
            .GetRequiredSharpModuleInterface<IMenuManager>(IMenuManager.Identity).Instance!;
        _nativeVoteManager.SetDefaultMenuCompat(new FpmMenuCompat(_sharedSystem, menuManager));

        var adminManager = _sharedSystem.GetSharpModuleManager()
            .GetRequiredSharpModuleInterface<IAdminManager>(IAdminManager.Identity).Instance!;
        _nativeVoteManager.SetDefaultPermissionCompat(new FpmPermissionCompat(adminManager));
    }

    public void Shutdown()
    {
    }
}
