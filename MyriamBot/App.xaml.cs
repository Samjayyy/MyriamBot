using Microsoft.ProjectOxford.Face;
using System.Configuration;
using System.Windows;
using Unity;

namespace MyriamBot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly string FaceApiKey = ConfigurationManager.AppSettings["FaceApiKey1"];
        private static readonly string FaceApiEndpoint = ConfigurationManager.AppSettings["FaceApiEndpoint"];

        public const string PIC_DIR = "pics";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // init IOC
            var container = new UnityContainer();
            //  face service client
            container.RegisterType<IFaceApiHelper, FaceApiHelper>();
            container.RegisterInstance<IFaceServiceClient>(new FaceServiceClient(FaceApiKey, FaceApiEndpoint));

            // Create pictures directory
            System.IO.Directory.CreateDirectory(PIC_DIR);

            // show main window
            var mainWindow = container.Resolve<MainWindow>();
            mainWindow.Show();
        }
    }
}
