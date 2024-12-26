using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ndsSharp.Viewer.Models.Files;

public partial class FlatItem : ObservableObject
{
    [ObservableProperty] private string _path;

    public FlatItem(string path)
    {
        Path = path;
    }

    [RelayCommand]
    public async Task CopyPath()
    {
        await Clipboard.SetTextAsync(Path);
    }
}