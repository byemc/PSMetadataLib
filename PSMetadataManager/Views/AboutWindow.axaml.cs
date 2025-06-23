using System;
using System.Diagnostics;
using Avalonia.Controls;

namespace PSMetadataManager.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
    }

    public void OpenSourceCodeCommand()
    {
        var target = "https://shinonome.rocks/bye/PSMetadataLib";
        
        if (OperatingSystem.IsLinux())
            Process.Start("xdg-open", target);
        else if (OperatingSystem.IsMacOS())
            Process.Start("open", target);
        else
            Process.Start(target);
    }

    public void CloseWindow()
    {
        Close();
    }
}