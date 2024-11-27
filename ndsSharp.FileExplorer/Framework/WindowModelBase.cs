using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using ndsSharp.FileExplorer.Extensions;

namespace ndsSharp.FileExplorer.Framework;

public partial class WindowModelBase : ViewModelBase
{
    [ObservableProperty] private string _titleString;
    [ObservableProperty] private static WindowIcon _iconSource = new(ImageExtensions.AvaresBitmap("avares://ndsSharp.FileExplorer/Assets/icon.png"));
}