using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NativeVoteExample.Commands;
using NativeVoteExample.YesNoNative;
using NativeVoteManagerMS.Shared;
using Sharp.Shared;

namespace NativeVoteExample;

public class NativeVoteExample: IModSharpModule
{
    public NativeVoteExample(ISharedSystem  sharedSystem,
        string         dllPath,
        string         sharpPath,
        Version       version,
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

    public string DisplayName => "NativeVoteManagerMS - Example";
    public string DisplayAuthor => "faketuna";

    private readonly ISharedSystem _sharedSystem;
    private readonly ILogger _logger;
    private INativeVoteManager _nativeVoteManager = null!;
    
    private YesNoVoteCommand _yesNoVoteCommand = null!;
    private MultiChoiceVoteCommand _multiChoiceVoteCommand = null!;

    public bool Init()
    {
        return true;
    }

    public void PostInit()
    {
    }

    public void OnAllModulesLoaded()
    {
        _nativeVoteManager = _sharedSystem.GetSharpModuleManager().GetRequiredSharpModuleInterface<INativeVoteManager>(INativeVoteManager.ModSharpModuleIdentity).Instance!;

        _yesNoVoteCommand = new YesNoVoteCommand(_nativeVoteManager, _sharedSystem);
        _sharedSystem.GetClientManager().InstallCommandCallback("yesnov", _yesNoVoteCommand.Execute);

        _multiChoiceVoteCommand = new MultiChoiceVoteCommand(_nativeVoteManager, _sharedSystem);
        _sharedSystem.GetClientManager().InstallCommandCallback("mltcv", _multiChoiceVoteCommand.Execute);

    }

    public void Shutdown()
    {
        _sharedSystem.GetClientManager().RemoveCommandCallback("yesnov", _yesNoVoteCommand.Execute);
        _sharedSystem.GetClientManager().RemoveCommandCallback("mltcv", _multiChoiceVoteCommand.Execute);
    }
}
