using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Viewer.Shared.Extensions;
using ndsSharp.Viewer.Shared.Plugins;
using ImageExtensions = ndsSharp.Core.Conversion.Textures.Images.ImageExtensions;

namespace ndsSharp.Viewer.Plugins.G3D.FileViewers;

public partial class BTXViewer : BaseFileViewer<BTXViewerModel>
{
    public BTXViewer()
    {
        InitializeComponent();
        DataContext = WindowModel;
    }
}

public partial class BTXViewerModel : BaseFileViewerModel<BTX>
{
    public override string Title => "BTX Viewer";
    
    [ObservableProperty] private string _textureName;
    [ObservableProperty] private Bitmap _currentBitmap;
    
    [ObservableProperty] private int _currentTextureIndex = 0;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(MaximumTextureIndex))] private ObservableCollection<BaseImage> _textures = [];
    public int MaximumTextureIndex => Textures.Count - 1;
    
    public override void Load(BTX obj)
    {
        Textures = new ObservableCollection<BaseImage>(obj.TextureData.Textures);
        
        SetTexture(0);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(CurrentTextureIndex):
            {
                SetTexture(CurrentTextureIndex);
                break;
            }
        }
    }

    private void SetTexture(int index)
    {
        TextureName = Textures[index].Name;
        CurrentBitmap = Textures[index].ToImage().ToWriteableBitmap();
    }
}