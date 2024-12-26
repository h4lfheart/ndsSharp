using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using ndsSharp.Viewer.Shared.Extensions;

namespace ndsSharp.Viewer.Shared.Framework;

public partial class WindowModelBase : ViewModelBase
{
    [ObservableProperty] private string _titleString;
    [ObservableProperty] private static WindowIcon _iconSource = new(ImageExtensions.AvaresBitmap("avares://ndsSharp.Viewer/Assets/icon.png"));
}