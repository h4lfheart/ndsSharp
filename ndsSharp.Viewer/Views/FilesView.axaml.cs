using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ndsSharp.Viewer.Models;
using ndsSharp.Viewer.Models.Files;
using ndsSharp.Viewer.Shared.Framework;
using ndsSharp.Viewer.ViewModels;

namespace ndsSharp.Viewer.Views;

public partial class FilesView : ViewBase<FilesViewModel>
{
    public FilesView()
    {
        InitializeComponent();
    }
    
    private void OnSearchKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        if (sender is not TextBox textBox) return;

        ViewModel.SearchFilter = textBox.Text ?? string.Empty;
    }

    private void OnFlatItemDoubleTapped(object? sender, TappedEventArgs e)
    { 
        if (sender is not ListBox listBox) return;
        if (listBox.SelectedItem is not FlatItem item) return;
        
        ViewModel.TreeViewJumpTo(item.Path);
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
        
        ViewModel.SearchFilter = string.Empty;
        ViewModel.FlatViewJumpTo(item.FilePath);
    }
}