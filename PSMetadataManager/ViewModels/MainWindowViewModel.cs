using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PSMetadataManager.Messages;

namespace PSMetadataManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public void ExitMenuItem_Command()
    {
        if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime app)
        {
            app.Shutdown();
        }
    }

    [RelayCommand]
    private async Task ShowAboutWindowAsync()
    {
        var about = await WeakReferenceMessenger.Default.Send(new AboutWindowMessage());
    }
}
