using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace T7_Hub
{
    public partial class Setup : Page
    {
        private DispatcherTimer clientTimer;
        private Config config;


        public Setup()
        {
            InitializeComponent();

            config = ConfigManager.Load();

            Loaded += Setup_Loaded;

            FileManager.CreateStandbyFolders();

            clientTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            clientTimer.Tick += ClientTimer_Tick;
            clientTimer.Start();
        }


        private void Setup_Loaded(object sender, RoutedEventArgs e)
        {
            string? path = GameLocator.FindBO3();

            if (path != null)
            {
                BO3PathTB.Text = path;
                SaveGamePath(path);
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(
                        "Could not locate Black Ops III installation. Please locate manually."
                    );
                }, DispatcherPriority.ApplicationIdle);
            }

            UpdateClientStatus();
        }


        private void ClientTimer_Tick(object? sender, EventArgs e)
        {
            UpdateClientStatus();
        }


        private void UpdateClientStatus()
        {
            ClientList.Children.Clear();

            string[] clients =
            {
                "T7 Patch",
                "BOIII Community",
                "Ezz BOIII",
                "T7x",
                "CleanOps T7",
                "Stock BO3"
            };


            foreach (string client in clients)
            {
                bool installed;

                if (client == "Stock BO3")
                {
                    installed = FileManager.CheckStockBO3();
                }
                else
                {
                    installed = FileManager.CheckClient(client);
                }


                TextBlock text = new TextBlock
                {
                    Text = installed ? $"✓ {client}" : $"✗ {client}",
                    Foreground = installed ? Brushes.LimeGreen : Brushes.Gray,
                    FontSize = 14,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5)
                };


                ClientList.Children.Add(text);
            }
        }


        private void SaveGamePath(string path)
        {
            config.GamePath = path;
            ConfigManager.Save(config);
        }


        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new();

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FolderName;

                if (ValidateBO3Path(path))
                {
                    BO3PathTB.Text = path;
                    SaveGamePath(path);
                }
            }
        }


        private void LocateButton_Click(object sender, RoutedEventArgs e)
        {
            string? path = GameLocator.FindBO3();

            if (path != null)
            {
                BO3PathTB.Text = path;
                SaveGamePath(path);
            }
            else
            {
                MessageBox.Show(
                    "Could not locate Black Ops III installation. Please locate manually."
                );
            }
        }


        private bool ValidateBO3Path(string path)
        {
            if (!Directory.Exists(path))
            {
                MessageBox.Show(
                    "The selected folder does not exist."
                );

                return false;
            }


            if (!File.Exists(Path.Combine(path, "BlackOps3.exe")))
            {
                MessageBox.Show(
                    "This does not appear to be a Black Ops III installation."
                );

                return false;
            }


            return true;
        }


        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            config.HasCompletedSetup = true;

            ConfigManager.Save(config);

            NavigationService.Navigate(new HomePage());
        }


        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "T7 Hub",
                "Standby"
            );

            Directory.CreateDirectory(path);

            Process.Start("explorer.exe", path);
        }
    }
}