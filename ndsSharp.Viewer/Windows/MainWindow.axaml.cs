using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using ndsSharp.Viewer.Models;
using ndsSharp.Viewer.Models.Files;
using ndsSharp.Viewer.Shared.Framework;
using ndsSharp.Viewer.WindowModels;

namespace ndsSharp.Viewer.Windows;

public partial class MainWindow : WindowBase<MainWindowModel>
{
    public MainWindowModel WindowModel;
    
    public MainWindow()
    {
        InitializeComponent();

        WindowModel = new MainWindowModel();
        DataContext = WindowModel;
    }

    private void OnSearchKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        if (sender is not TextBox textBox) return;

        WindowModel.SearchFilter = textBox.Text ?? string.Empty;
    }

    private void OnFlatItemDoubleTapped(object? sender, TappedEventArgs e)
    { 
        if (sender is not ListBox listBox) return;
        if (listBox.SelectedItem is not FlatItem item) return;
        
        WindowModel.TreeViewJumpTo(item.Path);
    }

    private void OnTreeItemTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not TreeView treeView) return;
        if (treeView.SelectedItem is not TreeItem item) return;
        if (string.IsNullOrWhiteSpace(item.FilePath))
        {
            item.Expanded = !item.Expanded;
            return;
        }
        
        WindowModel.SearchFilter = string.Empty;
        WindowModel.FlatViewJumpTo(item.FilePath);
    }

    
    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var targetItem = Enumerable.FirstOrDefault<FlatItem>(WindowModel.SelectedFlatViewItems);
        if (targetItem is null) return;

        var targetFile = WindowModel.Provider.Files[targetItem.Path];
        WindowModel.Properties = new PropertiesData
        {
            Name = targetFile.Name[..targetFile.Name.IndexOf('.')],
            Type = targetFile.Type,
            Offset = targetFile.Pointer.Offset,
            Length = targetFile.Pointer.Length
        };
    }
}