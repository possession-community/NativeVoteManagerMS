using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NativeVoteManagerMS.Shared;
using Sharp.Modules.MenuManager.Shared;
using Sharp.Shared;

namespace NativeVoteManagerFPMMenuConnector;

public class NativeVoteManagerFPMMenuConnector : IModSharpModule
{
    public NativeVoteManagerFPMMenuConnector(ISharedSystem  sharedSystem,
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

    public string DisplayName => "NativeVoteManagerMS - FPMMenuConnector";
    public string DisplayAuthor => "faketuna";

    private readonly ISharedSystem _sharedSystem;
    private readonly ILogger _logger;
    private INativeVoteManager _nativeVoteManager = null!;
    private FPMMenuCompat _fpmMenuCompat = null!;
    private IMenuManager _menuManager = null!;

    public bool Init()
    {
        return true;
    }

    public void PostInit()
    {
    }

    public void OnAllModulesLoaded()
    {
        _menuManager = _sharedSystem.GetSharpModuleManager()
            .GetRequiredSharpModuleInterface<IMenuManager>(IMenuManager.Identity).Instance!;
        
        _fpmMenuCompat = new FPMMenuCompat(_sharedSystem, _menuManager);
        _nativeVoteManager = _sharedSystem.GetSharpModuleManager().GetRequiredSharpModuleInterface<INativeVoteManager>(INativeVoteManager.ModSharpModuleIdentity).Instance!;
        _nativeVoteManager.SetDefaultMenuCompat(_fpmMenuCompat);
    }

    public void Shutdown()
    {
    }
}
