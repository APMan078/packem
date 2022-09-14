using Autofac;
using Newtonsoft.Json;
using Packem.Mobile.Application;
using Packem.Mobile.Models;
using System.IO;
using System.Reflection;
using Xamarin.Forms;

[assembly: ExportFont("MaterialIcons-Regular.otf", Alias = "MaterialIcons-Regular")]
[assembly: ExportFont("MaterialIconsOutlined-Regular.otf", Alias = "MaterialIconsOutlined-Regular")]
namespace Packem.Mobile
{
    public partial class App : Xamarin.Forms.Application
    {
        public static IContainer Container;

        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider
                .RegisterLicense("NjM5NDA5QDMyMzAyZTMxMmUzMFZHU3VrNlhVNDBReW83Z3FZVkFabndqcTBxN2lXSWNyN3FJNy93ak1rZ289");

            InitializeComponent();

            //class used for registration
            var builder = new ContainerBuilder();

            //scan and register all classes in the assembly
            var dataAccess = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(dataAccess)
                   .AsImplementedInterfaces()
                   .AsSelf();

            //get container
            Container = builder.Build();

            //DependencyService.Register<MockDataStore>();
            //MainPage = Container.Resolve<LoadingView>();
            MainPage = new AuthenticationShell();
            //MainPage = new AppShell();
        }

        // https://onthefencedevelopment.com/2019/10/16/appsettings-in-xamarin-forms/
        private static AppSettings appSettings;
        public static AppSettings AppSettings
        {
            get
            {
                if (appSettings is null)
                    LoadAppSettings();

                return appSettings;
            }
        }
        private static void LoadAppSettings()
        {
#if RELEASE
            var appSettingsResourceSteam = 
                Assembly.GetAssembly(typeof(AppSettings)).GetManifestResourceStream("Packem.Mobile.Configuration.appsettings.release.json");
#else
            var appSettingsResourceSteam =
                Assembly.GetAssembly(typeof(AppSettings)).GetManifestResourceStream("Packem.Mobile.Configuration.appsettings.debug.json");
#endif

            if (appSettingsResourceSteam is null)
                return;

            using (var streamReader = new StreamReader(appSettingsResourceSteam))
            {
                var json = streamReader.ReadToEnd();
                appSettings = JsonConvert.DeserializeObject<AppSettings>(json);
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
