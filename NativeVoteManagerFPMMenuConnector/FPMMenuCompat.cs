using NativeVoteManagerMS.Shared;
using NativeVoteManagerMS.Shared.Types;
using Sharp.Modules.MenuManager.Shared;
using Sharp.Shared;
using Sharp.Shared.Objects;

namespace NativeVoteManagerFPMMenuConnector;

public class FPMMenuCompat(ISharedSystem sharedSystem, IMenuManager menuManager): IMenuCompat
{
    private MultiChoiceVoteOptions _voteOptions = null!;
    private Dictionary<IGameClient, Menu> _menuCaches = new();
    
    public void OpenMenu(IGameClient target)
    {
        if (_menuCaches.TryGetValue(target, out var menu))
        {
            if (menuManager.IsInCurrentMenu(target, menu))
                return;
            
            menuManager.DisplayMenu(target, menu);
            return;
        }

        bool menuShuffle = _voteOptions.RandomShuffle;

        var menuBuilder = Menu.Create();
        
        menuBuilder.Title(_voteOptions.Title());

        if (menuShuffle)
        {
            var shuffledContent = _voteOptions.VoteContents.Shuffle();
            foreach (var content in shuffledContent)
            {
                menuBuilder.Item(content.VisibleName(), controller =>
                {
                    OnChoice(target, content);
                });
                
                if (content.VisibleDescription is not null)
                {
                    menuBuilder.DisabledItem(content.VisibleDescription());
                }
            }
        }
        else
        {
            foreach (var content in _voteOptions.VoteContents)
            {
                menuBuilder.Item(content.VisibleName(), controller =>
                {
                    OnChoice(target, content);
                });
                
                if (content.VisibleDescription is not null)
                {
                    menuBuilder.DisabledItem(content.VisibleDescription());
                }
            }
        }
        
        
        var newMenu =  menuBuilder.Build();
        _menuCaches[target] = newMenu;
        menuManager.DisplayMenu(target, newMenu);
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