using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using FluentAvalonia.UI.Controls;
using ndsSharp.Viewer.Models;
using ndsSharp.Viewer.Models.Files;
using ndsSharp.Viewer.Shared.Framework;
using ndsSharp.Viewer.Views;
using ndsSharp.Viewer.WindowModels;

namespace ndsSharp.Viewer.Windows;

public partial class MainWindow : WindowBase<MainWindowModel>
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = WindowModel;
        
        WindowModel.ContentFrame = ContentFrame;
        WindowModel.NavigationView = NavigationView;
        WindowModel.Navigate<FilesView>();
    }
    
    private void OnPointerPressedUpperBar(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }
    
    private async void OnItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
    {
        var tag = e.InvokedItemContainer.Tag;
        switch (tag)
        {
            case "OpenROM":
            {
                if (await BrowseFileDialog(fileTypes: Globals.RomFileType) is { } romPath)
                {
                    WindowModel.LoadFiles(romPath);
                }
                break;
            }
            default:
            {
                var viewName = $"ndsSharp.Viewer.Views.{tag}View";
        
                var type = Type.GetType(viewName);
                if (type is null)
                {
                    MainWM.Message("Unimplemented View", $"The {tag} view has not been implemented yet.");
                    return;
                }
        
                WindowModel.Navigate(type);
                
                break;
            }
        }
    }
}