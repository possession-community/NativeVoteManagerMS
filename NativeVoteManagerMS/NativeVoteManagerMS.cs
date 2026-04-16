using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NativeVoteManagerMS.Shared;
using Sharp.Modules.LocalizerManager.Shared;
using Sharp.Shared;

namespace NativeVoteManagerMS;

public class NativeVoteManagerMs : IModSharpModule
{
    public NativeVoteManagerMs(ISharedSystem  sharedSystem,
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

    public string DisplayName => "NativeVoteManagerMS";
    public string DisplayAuthor => "faketuna";

    private readonly ISharedSystem _sharedSystem;
    private readonly ILogger _logger;

    private NativeVoteManager _voteManager = null!;

    public bool Init()
    {
        _voteManager = new NativeVoteManager(_sharedSystem, _logger);
        _sharedSystem.GetModSharp().InstallGameListener(_voteManager);
        _sharedSystem.GetClientManager().InstallCommandListener("vote", _voteManager.OnVoteCommand);
        _sharedSystem.GetClientManager().InstallCommandCallback("cancelvote", _voteManager.OnCancelVoteCommand);
        _sharedSystem.GetClientManager().InstallCommandCallback("revote", _voteManager.OnRevoteCommand);
        return true;
    }

    public void PostInit()
    {
        _sharedSystem.GetSharpModuleManager().RegisterSharpModuleInterface<INativeVoteManager>(this, INativeVoteManager.ModSharpModuleIdentity, _voteManager);
    }

    public void OnAllModulesLoaded()
    {
        var localizerManager = _sharedSystem.GetSharpModuleManager()
            .GetOptionalSharpModuleInterface<ILocalizerManager>(ILocalizerManager.Identity)?.Instance;

        if (localizerManager is not null)
        {
            localizerManager.LoadLocaleFile("nativevotemanager");
            _voteManager.SetLocalizerManager(localizerManager);
        }
        else
        {
            _logger.LogWarning("LocalizerManager not found. Using default English messages.");
        }
    }

    public void Shutdown()
    {
        _voteManager.CancelVote();
        _sharedSystem.GetModSharp().RemoveGameListener(_voteManager);
        _sharedSystem.GetClientManager().RemoveCommandListener("vote", _voteManager.OnVoteCommand);
        _sharedSystem.GetClientManager().RemoveCommandCallback("cancelvote", _voteManager.OnCancelVoteCommand);
        _sharedSystem.GetClientManager().RemoveCommandCallback("revote", _voteManager.OnRevoteCommand);
    }
}
