using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ndsSharp.FileExplorer.Framework;

public partial class WindowModelBase : ViewModelBase
{
    [ObservableProperty] private string _titleString;
    [ObservableProperty] private static WindowIcon _iconSource;
}