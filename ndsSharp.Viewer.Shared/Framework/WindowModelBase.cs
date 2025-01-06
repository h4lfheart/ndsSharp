using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using ndsSharp.Viewer.Shared.Extensions;

namespace ndsSharp.Viewer.Shared.Framework;

public partial class WindowModelBase : ViewModelBase
{
    [ObservableProperty] private string _titleString = "ndsSharp.Viewer";
    [ObservableProperty] private static WindowIcon _iconSource = new(ImageExtensions.AvaresBitmap("avares://ndsSharp.Viewer/Assets/icon.png"));
}