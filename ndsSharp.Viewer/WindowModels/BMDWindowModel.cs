using System.Collections.Generic;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ndsSharp.Core.Conversion.Models.Mesh;
using ndsSharp.Core.Conversion.Models.Processing;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Viewer.Shared.Framework;
using ndsSharp.Viewer.Shared.Rendering;
using ndsSharp.Viewer.Shared.Rendering.Rendering;

namespace ndsSharp.Viewer.WindowModels;

public partial class BMDWindowModel : WindowModelBase
{
    [ObservableProperty] private string _modelName;
    [ObservableProperty] private int _maximumModelIndex;
    [ObservableProperty] private int _currentModelIndex = 0;

    [ObservableProperty] private ModelPreviewControl? _renderingControl;

    private List<Model> Models = [];
    
    public void LoadBMD(BMD bmd)
    {
        if (RenderingControl is null)
        {
            RenderingControl = new ModelPreviewControl();
        }
        else
        {
            RenderingControl.Context.Renderer.Clear();
        }
        
        TitleString = $"BMD Viewer - {bmd.Owner!.Path}";
        
        Models = bmd.ExtractModels();
        MaximumModelIndex = Models.Count - 1;
        
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
        RenderingControl.Context.ModelQueue.Enqueue(new Actor(Models[index]));
    }
}