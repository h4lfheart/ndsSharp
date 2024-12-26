using CommunityToolkit.Mvvm.ComponentModel;

namespace ndsSharp.Viewer.Shared.Framework;

public class ViewModelBase : ObservableValidator
{
    public virtual async Task Initialize() { }  
    public virtual async Task OnViewOpened() { }
    public virtual async Task OnViewExited() { }
}