using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ndsSharp.Viewer.Models.Files;

public partial class TreeItem : ObservableObject
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _filePath;
    [ObservableProperty] private ENodeType _type;
    
    [ObservableProperty] private bool _selected;
    [ObservableProperty] private bool _expanded;

    [ObservableProperty] private Dictionary<string, TreeItem> _children = [];

    public TreeItem(string name, ENodeType type, string filePath = "")
    {
        Name = name;
        Type = type;
        FilePath = filePath;
    }
}

public enum ENodeType
{
    Folder,
    File
}