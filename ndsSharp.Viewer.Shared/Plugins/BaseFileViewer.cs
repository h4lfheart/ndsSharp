using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using ndsSharp.Core.Objects;
using ndsSharp.Core.Objects.Exports;
using ndsSharp.Viewer.Shared.Framework;

namespace ndsSharp.Viewer.Shared.Plugins;

public abstract partial class BaseFileViewer<T> : WindowBase<T> where T : BaseFileViewerModel, new()
{

}