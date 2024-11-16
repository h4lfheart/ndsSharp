using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.FileExplorer.Extensions;
using ndsSharp.FileExplorer.Framework;
using SixLabors.ImageSharp;

namespace ndsSharp.FileExplorer.WindowModels;

public partial class BTXWindowModel : WindowModelBase
{
    [ObservableProperty] private string _textureName;
    [ObservableProperty] private Bitmap _currentBitmap;
    [ObservableProperty] private int _maximumTextureIndex;
    [ObservableProperty] private int _currentTextureIndex = 0;

    private List<BaseImage> Images;
    
    public void LoadBTX(BTX0 btx)
    {
        TitleString = $"BTX Viewer - {btx.Owner!.Path}";

        Images = btx.TextureData.Textures;
        MaximumTextureIndex = Images.Count - 1;
        
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
        CurrentBitmap = Images[index].ToImage().ToWriteableBitmap();
        TextureName = Images[index].Name;
    }
}