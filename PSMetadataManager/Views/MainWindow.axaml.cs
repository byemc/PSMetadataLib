using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using PSMetadataManager.Messages;
using PSMetadataManager.ViewModels;

namespace PSMetadataManager.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        if (Design.IsDesignMode) return;
        
        WeakReferenceMessenger.Default.Register<MainWindow,AboutWindowMessage>(this, static (w, m) =>
        {
            var dialog = new AboutWindow
            {
                DataContext = new AboutWindow()
            };
            
            m.Reply(dialog.ShowDialog<AboutWindow?>(w));
        });
    }
}