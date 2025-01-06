using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ndsSharp.Viewer.Models;
using ndsSharp.Viewer.Models.Files;
using ndsSharp.Viewer.Shared.Framework;
using ndsSharp.Viewer.ViewModels;

namespace ndsSharp.Viewer.Views;

public partial class PluginsView : ViewBase<PluginsViewModel>
{
    public PluginsView() : base(PluginsVM)
    {
        InitializeComponent();
    }
    
}