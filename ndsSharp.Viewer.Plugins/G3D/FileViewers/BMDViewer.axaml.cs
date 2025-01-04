using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ndsSharp.Core.Conversion.Models.Mesh;
using ndsSharp.Core.Conversion.Models.Processing;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Viewer.Shared.Plugins;
using ndsSharp.Viewer.Shared.Rendering;
using ndsSharp.Viewer.Shared.Rendering.Rendering;
using ndsSharp.Viewer.Shared.Services;

namespace ndsSharp.Viewer.Plugins.G3D.FileViewers;

public partial class BMDViewer : BaseFileViewer<BMDViewerModel>
{
    public override bool OnlyOneWindow => true;
    
    public BMDViewer()
    {
        InitializeComponent();
        DataContext = WindowModel;
    }
}

public partial class BMDViewerModel : BaseFileViewerModel<BMD>
{
    public override string Title => "BMD Viewer";

    [ObservableProperty] private ModelPreviewControl? _renderingControl;

    [ObservableProperty] private string _modelName;
    
    [ObservableProperty] private int _currentModelIndex = 0;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(MaximumModelIndex))] private ObservableCollection<Model> _models = [];
    public int MaximumModelIndex => Models.Count - 1;
    
    public override async void Load(BMD obj)
    {
        if (RenderingControl is null)
        {
            await TaskService.RunDispatcherAsync(() =>
            {
                RenderingControl = new ModelPreviewControl();
            });
        }
        
        Models = new ObservableCollection<Model>(obj.ExtractModels());
        
        SetModel(0);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(CurrentModelIndex):
            {
                SetModel(CurrentModelIndex);
                break;
            }
        }
    }

    private void SetModel(int index)
    {
        ModelName = Models[index].Name;
        RenderingControl?.Context.Renderer.Clear();
        RenderingControl?.Context.ModelQueue.Enqueue(new Actor(Models[index]));
    }
}