using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ndsSharp.Core.Conversion.Models.Processing;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Plugins.BW2;
using ndsSharp.Viewer.Shared.Framework;
using ndsSharp.Viewer.Shared.Plugins;
using ndsSharp.Viewer.Shared.Rendering;
using ndsSharp.Viewer.Shared.Rendering.Rendering;
using ndsSharp.Viewer.Shared.Services;
using OpenTK.Mathematics;

namespace ndsSharp.Viewer.Plugins.BW2.Windows;

public partial class BW2MatrixWindow : BasePluginWindow<BW2MatrixWindowModel>
{
    public BW2MatrixWindow()
    {
        InitializeComponent();
        DataContext = WindowModel;
    }
}

public partial class BW2MatrixWindowModel : BasePluginWindowModel
{
    [ObservableProperty] private ModelPreviewControl _renderingControl = null!;
    [ObservableProperty] private int _currentMatrixIndex;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _loadingText;
    [ObservableProperty] private ESeason _targetSeason = ESeason.Spring;

    private BW2Plugin Plugin => Provider.GetPluginInterface<BW2Plugin>();
    
    public override async Task Initialize()
    {
        TitleString = "Matrix Viewer";
        
        await TaskService.RunDispatcherAsync(() =>
        {
            RenderingControl = new ModelPreviewControl(useGrid: true);
        });
    }

    public void LoadMatrix(int matrixIndex)
    {
        IsLoading = true;
        RenderingControl.Context.Renderer.Clear();
        
        TitleString = $"Matrix Viewer - Matrix {matrixIndex}";
        
        var modelQueue = new Queue<Actor>();
        var mapCountingIndex = 0;
        var boundingBox = new Box3();
        
        var matrix = Plugin.GetMatrix(matrixIndex);
        foreach (var (x, y, mapIndex) in matrix.Maps)
        {
            mapCountingIndex++;
            LoadingText = $"{mapCountingIndex} / {matrix.Maps.Length}";
            
            if (mapIndex == -1) continue;
            
            var map = Plugin.GetMap(mapIndex);

            var headerIndex = matrix.HasHeaders ? matrix.Headers[x, y] : -1;
            var header = headerIndex != -1 
                ? Plugin.GetZone(headerIndex) 
                : Plugin.ZoneContainer.Headers.FirstOrDefault(header => header.MatrixIndex == matrixIndex);
            if (header is null) continue;
            
            var area = Plugin.GetArea(header.AreaIndex);

            var mapTextures = Plugin.GetAreaMapTextures(area, area.IsExterior ? TargetSeason : ESeason.Spring);
               
            var buildingContainer = Plugin.GetAreaBuildingContainer(area);
            var buildingTextures = Plugin.GetAreaBuildingTextures(area);

            var mapBaseModel = map.Model.ExtractModels(overrideTextureData: mapTextures.TextureData).First();
            var mapBaseActor = new Actor(mapBaseModel, new Vector3(x * 512, 0, y * 512));
            boundingBox.Extend(mapBaseActor.Location);
            modelQueue.Enqueue(mapBaseActor);

            if (map.Actors.Length > 0)
            {
                foreach (var mapActor in map.Actors)
                {
                    var buildingDefinition = buildingContainer.Definitions.FirstOrDefault(x => x.ID == mapActor.ModelID);
                    if (buildingDefinition is null) continue;
                    
                    var buildingModel = buildingContainer.Models[buildingContainer.Definitions.IndexOf(buildingDefinition)];

                    var renderLocation = new Vector3();
                    renderLocation.X = mapActor.Location.X;
                    renderLocation.Y = mapActor.Location.Y;
                    renderLocation.Z = -mapActor.Location.Z;
                    renderLocation += mapBaseActor.Location;

                    var model = buildingModel.ExtractModels(buildingTextures.TextureData).First();
                    var actor = new Actor(model, renderLocation, Quaternion.FromAxisAngle(Vector3.UnitY, mapActor.Rotation * MathF.PI / 180f));

                    modelQueue.Enqueue(actor);
                }
            }
        }

        RenderingControl.Context.Camera.Position = boundingBox.Center + Vector3.UnitY * 512 + Vector3.UnitZ * 512;
        RenderingControl.Context.Camera.Yaw = -90;
        RenderingControl.Context.Camera.Pitch = -45;
        RenderingControl.Context.Camera.UpdateDirection();

        RenderingControl.Context.ModelQueue = modelQueue;
        
        while (RenderingControl.Context.ModelQueue.Count > 0) { }
        IsLoading = false;
    }

    [RelayCommand]
    public void Load()
    {
        TaskService.Run(() =>
        {
            LoadMatrix(CurrentMatrixIndex);
        });
    }
}