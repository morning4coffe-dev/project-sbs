﻿using System.Reflection;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace ProjectSBS.Services.Interop;

public class InteropService : IInteropService
{
    private ApplicationTheme _appTheme;
    private CoreDispatcher _dispatcher;

    public InteropService()
    {
        _dispatcher = Window.Current.Dispatcher;

        UISettings uiSettings = new UISettings();
        uiSettings.ColorValuesChanged += HandleSystemThemeChange;
    }

    private void HandleSystemThemeChange(UISettings sender, object args)
    {
        if (Window.Current.Content is FrameworkElement frameworkElement)
        {
            UpdateTitleBar(frameworkElement.RequestedTheme);
        }
    }

    public async Task SetThemeAsync(ElementTheme theme)
    {
        await SetRequestedThemeAsync(theme);
    }

    public Version GetAppVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        return AssemblyName.GetAssemblyName(assembly.Location).Version ?? new Version(1, 0);
    }

    private async Task SetRequestedThemeAsync(ElementTheme theme)
    {
        await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = theme;
                UpdateTitleBar(theme);
            }
        });
    }

    private void UpdateTitleBar(ElementTheme theme)
    {
        Color? color = null;
        _appTheme = Application.Current.RequestedTheme;

        switch (theme)
        {
            case ElementTheme.Default:
                color = (Color)Application.Current.Resources["SystemBaseHighColor"];
                break;
            case ElementTheme.Light:
                if (_appTheme == ApplicationTheme.Light)
                { color = ((Color)Application.Current.Resources["SystemBaseHighColor"]); }
                else
                { color = (Color)Application.Current.Resources["SystemAltHighColor"]; }
                break;
            case ElementTheme.Dark:
                if (_appTheme == ApplicationTheme.Light)
                { color = ((Color)Application.Current.Resources["SystemAltHighColor"]); }
                else
                { color = (Color)Application.Current.Resources["SystemBaseHighColor"]; }
                break;
            default:
                break;
        }

        var titleBar = ApplicationView.GetForCurrentView().TitleBar;
        titleBar.ForegroundColor = color;
    }

    public async Task OpenStoreReviewUrlAsync()
    {
        // TODO: Packaged version
        await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9PN77P9WJ3CX"));
    }

    public void UpdateAppTitle(string title)
    {
        ApplicationView.GetForCurrentView().Title = title;
    }
}
