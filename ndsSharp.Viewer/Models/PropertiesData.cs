using CommunityToolkit.Mvvm.ComponentModel;

namespace ndsSharp.Viewer.Models;

public partial class PropertiesData : ObservableObject
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _type;
    [ObservableProperty] private int _offset;
    [ObservableProperty] private int _length;

}