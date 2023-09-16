using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using YoutubeDownloader.Commands;
using YoutubeDownloader.DbContexts;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;
using YoutubeDownloader.Services.VideoCreators;
using YoutubeDownloader.Services.VideoProviders;
using YoutubeDownloader.Services.yt_dlp;
using YoutubeDownloader.Stores;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .UseSerilog((host, loggerConfiguration) =>
                {
                    loggerConfiguration.WriteTo.File("Logger.txt", rollingInterval: RollingInterval.Day)
                        .MinimumLevel.Debug()
                        .Enrich.WithExceptionDetails()
                        .Enrich.FromLogContext();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    string connectionString = hostContext.Configuration.GetConnectionString("Default");

                    services.AddSingleton(new DownloaderDbContextFactory(connectionString));
                    services.AddSingleton<IVideoCreator, DatabaseVideoCreator>();
                    services.AddSingleton<IVideoProvider, DatabaseVideoProvider>();

                    services.AddSingleton((s) => new DownloaderStore(s.GetRequiredService<IVideoProvider>(), s.GetRequiredService<IVideoCreator>()));

                    services.AddSingleton((s) => new Downloader(s.GetRequiredService<DownloaderStore>(), s.GetRequiredService<ILogger<YtdlpDownloader>>()));
                    services.AddSingleton<NavigationStore>();

                    services.AddTransient<DownloadViewModel>((s) => CreateDownloadViewModel(s));
                    services.AddSingleton<Func<DownloadViewModel>>((s) => () => s.GetRequiredService<DownloadViewModel>());
                    services.AddSingleton<NavigationService<DownloadViewModel>>();

                    services.AddTransient<DownloadHistoryViewModel>((s) => CreateDownloadHistoryViewModel(s));
                    services.AddSingleton<Func<DownloadHistoryViewModel>>((s) => () =>  s.GetRequiredService<DownloadHistoryViewModel>());
                    services.AddSingleton<NavigationService<DownloadHistoryViewModel>>();

                    services.AddTransient<AboutViewModel>();
                    services.AddSingleton<Func<AboutViewModel>>((s) => () => s.GetRequiredService<AboutViewModel>());
                    services.AddSingleton<NavigationService<AboutViewModel>>();

                   // services.AddSingleton<DownloadCommand>();

                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton(s => new MainWindow()
                    {
                        DataContext = s.GetRequiredService<MainViewModel>()
                    });
                })
                .Build();
        }

        private DownloadViewModel CreateDownloadViewModel(IServiceProvider s)
        {
            return DownloadViewModel.LoadViewModel(
                s.GetRequiredService<DownloaderStore>(),
                s.GetRequiredService<Downloader>(),
                s.GetRequiredService<ILogger<DownloadViewModel>>()
                );
        }

        private DownloadHistoryViewModel CreateDownloadHistoryViewModel(IServiceProvider s)
        {
            return DownloadHistoryViewModel.LoadViewModel(
                s.GetRequiredService<DownloaderStore>(),
                s.GetRequiredService<NavigationService<DownloadViewModel>>()
                );
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var loggerFactory = _host.Services.GetRequiredService<ILogger<MainWindow>>();
                //var logger = loggerFactory.CreateLogger<Program>();
                loggerFactory.LogError(args.ExceptionObject as Exception, "An error happened");
            };

            DownloaderDbContextFactory downloaderDbContextFactory = _host.Services.GetRequiredService<DownloaderDbContextFactory>();
            using (DownloaderDbContext dbContext = downloaderDbContextFactory.CreateDbContext())
            {
                dbContext.Database.Migrate();
            }

            NavigationService<DownloadViewModel> navigationService = _host.Services.GetRequiredService<NavigationService<DownloadViewModel>>();
            navigationService.Navigate();

            MainWindow = _host.Services.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Downloader downloader = _host.Services.GetRequiredService<Downloader>();
            downloader.CancelDownload();

            _host.Dispose();

            base.OnExit(e);
        }

       /* private DownloadViewModel CreateDownloadViewModel()
        {
            return DownloadViewModel.LoadViewModel(_downloaderStore, _downloader);
        }

        private DownloadHistoryViewModel CreateDownloadHistoryViewModel()
        {
            return DownloadHistoryViewModel.LoadViewModel(_downloaderStore, new NavigationService(_navigationStore, CreateDownloadViewModel));
        }

        private ViewModelBase CreateAboutViewModel()
        {
            return new AboutViewModel();
        }*/
    }
}
