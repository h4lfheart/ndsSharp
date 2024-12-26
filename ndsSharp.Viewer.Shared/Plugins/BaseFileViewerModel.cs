using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using ndsSharp.Core.Objects;
using ndsSharp.Core.Objects.Exports;
using ndsSharp.Viewer.Shared.Framework;

namespace ndsSharp.Viewer.Shared.Plugins;

public abstract partial class BaseFileViewerModel<T> : BaseFileViewerModel where T : BaseDeserializable
{
    public virtual void Load(T obj)
    {
        
    }
    
    public override void Load(BaseDeserializable obj)
    {
        base.Load(obj);
        
        Load((T) obj);
    }
}

public abstract partial class BaseFileViewerModel : WindowModelBase
{
    public virtual string Title => "Viewer";

    public virtual void Load(BaseDeserializable obj)
    {
        TitleString = $"{Title} - {obj.Owner!.Path}";
    }
}