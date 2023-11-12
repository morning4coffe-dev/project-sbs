﻿namespace ProjectSBS.Presentation;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel => (MainViewModel)DataContext;

    public MainPage()
    {
        this.InitializeComponent();

        this.DataContext = App.Services?.GetRequiredService<MainViewModel>();
        this.Loaded += Page_Loaded;
        this.Unloaded += Page_Unloaded;
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        //TODO Impacts perf a lot, mainly on mobile
        //while (App.Services?.GetRequiredService<MainViewModel>() == null)
        //{
        //    //Wait till Host is created
        //    await Task.Delay(200);

        //    //TODO Add loading indicator
        //}

        var nav = App.Services?.GetRequiredService<INavigation>()!;
        nav.NestedFrame = Frame;

        //this.DataContext = App.Services?.GetRequiredService<MainViewModel>();

        ViewModel.Load();
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Unload();
    }

    private void OnNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        ViewModel.Navigate(args);
    }
}