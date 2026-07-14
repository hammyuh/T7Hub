using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace T7_Hub
{
    public partial class SettingsPage : Page
    {
        private readonly Config config;

        public SettingsPage()
        {
            InitializeComponent();

            config = ConfigManager.Load();
            BO3PathTB.Text = config.GamePath;

            Opacity = 0;

            Loaded += (_, _) =>
            {
                BeginAnimation(
                    OpacityProperty,
                    new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250))
                );
            };
        }

        private bool ValidateBO3Path()
        {
            string path = BO3PathTB.Text;

            if (string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("Please select your Black Ops III folder.");
                return false;
            }

            if (!Directory.Exists(path))
            {
                MessageBox.Show("The selected folder does not exist.");
                return false;
            }

            if (!File.Exists(Path.Combine(path, "BlackOps3.exe")))
            {
                MessageBox.Show("This does not appear to be a Black Ops III installation.");
                return false;
            }

            return true;
        }

        private bool SaveSettings()
        {
            if (!ValidateBO3Path())
                return false;

            config.GamePath = BO3PathTB.Text;
            ConfigManager.Save(config);

            return true;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new();

            if (dialog.ShowDialog() == true)
            {
                BO3PathTB.Text = dialog.FolderName;

                if (!ValidateBO3Path())
                {
                    BO3PathTB.Text = config.GamePath;
                }
            }
        }

        private void LocateButton_Click(object sender, RoutedEventArgs e)
        {
            string? path = GameLocator.FindBO3();

            if (path != null)
            {
                BO3PathTB.Text = path;
            }
            else
            {
                MessageBox.Show(
                    "Could not locate Black Ops III installation. Please locate manually."
                );
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveSettings())
            {
                NavigationService.Navigate(new HomePage());
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveSettings())
            {
                ApplyButton.IsEnabled = false;
            }
        }

        private void BO3PathTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyButton.IsEnabled = config.GamePath != BO3PathTB.Text;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePage());
        }

        private void FactoryResetButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "This will delete all stored clients, backups, and T7 Hub configuration.\n\nYour Black Ops III installation will NOT be deleted.\n\nContinue?",
                "Factory Reset",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result != MessageBoxResult.Yes)
                return;


            string hubPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "T7 Hub"
            );


            try
            {
                if (Directory.Exists(hubPath))
                {
                    Directory.Delete(hubPath, true);
                }

                MessageBox.Show(
                    "T7 Hub has been reset. The application will restart.",
                    "Factory Reset Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );


                Process.Start(
                    Environment.ProcessPath!
                );

                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Factory reset failed:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}