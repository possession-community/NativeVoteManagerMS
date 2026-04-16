using System;
using System.Collections.Generic;
using System.Linq;
using NativeVoteManagerMS.Shared;
using NativeVoteManagerMS.Shared.Types;
using Sharp.Modules.MenuManager.Shared;
using Sharp.Shared;
using Sharp.Shared.Objects;

namespace NvmFPMCompat;

public class FpmMenuCompat(ISharedSystem sharedSystem, IMenuManager menuManager): IMenuCompat
{
    public ISharedSystem SharedSystem { get; } = sharedSystem;
    private MultiChoiceVoteOptions _voteOptions = null!;
    private Dictionary<IGameClient, Menu> _menuCaches = new();
    
    public void OpenMenu(IGameClient target)
    {
        if (!_menuCaches.TryGetValue(target, out var menu))
        {
            var menuBuilder = Menu.Create();
            menuBuilder.Title(_voteOptions.Title.Resolve());

            var contents = _voteOptions.RandomShuffle
                ? _voteOptions.VoteContents.Shuffle()
                : _voteOptions.VoteContents;

            foreach (var content in contents)
            {
                menuBuilder.Item(content.VisibleName.Resolve(), _ =>
                {
                    OnChoice(target, content);
                });
            }
            
            menu = menuBuilder.Build();
            _menuCaches[target] = menu;
        }

        if (menuManager.IsInMenu(target) && menuManager.IsInCurrentMenu(target, menu))
            return;

        menuManager.DisplayMenu(target, menu);
    }

    public void CloseMenu(IGameClient target)
    {
        if (_menuCaches.TryGetValue(target, out var menu))
        {
            if (!menuManager.IsInCurrentMenu(target, menu))
                return;
            
            menuManager.QuitMenu(target);
        }
    }

    public void SetVoteOptions(MultiChoiceVoteOptions options)
    {
        _voteOptions = options;
    }

    public void Cleanup()
    {
        _menuCaches.Clear();
    }

    public Action<IGameClient, VoteContent> OnChoice { get; set; } = null!;
}