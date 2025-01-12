using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ndsSharp.Core.Plugins.BW2.Text;
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
    
    [ObservableProperty] private int _maximumSectionIndex;
    [ObservableProperty] private int _currentSectionIndex = 0;
    
    [ObservableProperty] private int _currentEntryIndex = 0;

    private List<BW2TextSection> _sections = [];

    public override void Load(BW2Text obj)
    {
        _sections = obj.Sections;
        
        MaximumSectionIndex = _sections.Count - 1;

        SetText(0);
    }
    
    [RelayCommand]
    public void Copy()
    {
        ApplicationExtensions.CopyToClipboard(TextEntries[CurrentEntryIndex]);
    }
    
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(CurrentSectionIndex):
            {
                SetText(CurrentSectionIndex);
                break;
            }
        }
    }
    
    private void SetText(int index)
    {
        TextEntries = new ObservableCollection<string>(_sections[index].Strings.Select(str => str.ToString()));
        CurrentEntryIndex = 0;
    }
}