using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Viewer.Shared.Framework;
using ndsSharp.Viewer.WindowModels;

namespace ndsSharp.Viewer.Windows;

public partial class BTXWindow : WindowBase<BTXWindowModel>
{
    public BTXWindow()
    {
        InitializeComponent();
        DataContext = WindowModel;
    }

    public BTXWindow(BTX btx) : this()
    {
        WindowModel.LoadBTX(btx);
    }

    public static BTXWindow Create(BTX btx)
    {
        var window = new BTXWindow(btx);
        
        window.Show();
        window.BringToTop();
        return window;
    }
}