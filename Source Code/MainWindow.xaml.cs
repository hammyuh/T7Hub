using System.IO;
using System.Windows;

namespace T7_Hub
{
    public partial class MainWindow : Window
    {
        private readonly Config config;

        public MainWindow()
        {
            InitializeComponent();

            FileManager.Load();
            FileManager.CreateStandbyFolders();
            Loaded += async (_, _) =>
            {
                await HashUpdater.UpdateHashes();
            };

            Config config = ConfigManager.Load();

            if (string.IsNullOrWhiteSpace(config.GamePath))
            {
                string? bo3Path = GameLocator.FindBO3();

                if (bo3Path != null)
                {
                    config.GamePath = bo3Path;
                    ConfigManager.Save(config);
                }
            }

            if (config.HasCompletedSetup)
            {
                MainFrame.Navigate(new HomePage());
            }
            else
            {
                MainFrame.Navigate(new Setup());
            }
        }

        private void UpdateBO3Path()
        {
            if (!string.IsNullOrEmpty(config.GamePath) &&
                Directory.Exists(config.GamePath))
            {
                return;
            }

            string? bo3Path = GameLocator.FindBO3();

            if (bo3Path != null)
            {
                config.GamePath = bo3Path;
                ConfigManager.Save(config);
            }
        }
    }
}