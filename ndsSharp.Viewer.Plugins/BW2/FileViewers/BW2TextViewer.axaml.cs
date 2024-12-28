using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ndsSharp.Plugins.BW2.Text;
using ndsSharp.Viewer.Shared.Extensions;
using ndsSharp.Viewer.Shared.Plugins;

namespace ndsSharp.Viewer.Plugins.BW2.FileViewers;

public partial class BW2TextViewer : BaseFileViewer<BW2TextViewerModel>
{
    public BW2TextViewer()
    {
        InitializeComponent();
        DataContext = WindowModel;
    }

}

public partial class BW2TextViewerModel : BaseFileViewerModel<BW2Text>
{
    public override string Title => "Text Viewer";

    [ObservableProperty] private ObservableCollection<string> _textEntries = [];
    [ObservableProperty] private string _currentEntry;

    public override void Load(BW2Text obj)
    {
        TextEntries = new ObservableCollection<string>(obj.TextEntries);
    }

    [RelayCommand]
    public void Copy()
    {
        ApplicationExtensions.CopyToClipboard(CurrentEntry);
    }
}