using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Media;
using System.IO;

namespace T7_Hub
{
    public partial class HomePage : Page
    {
        private readonly Config config;

        private readonly Dictionary<string, Brush> ExeStatusColors = new()
        {
            { "Old", Brushes.Yellow },
            { "New", Brushes.LimeGreen },
            { "Unknown", Brushes.Red },
            { "Missing", Brushes.Gray }
        };


        private static readonly Dictionary<string, Action> launchActions = new()
        {
            { "Stock BO3", LauncherBackend.LaunchStock },
            { "T7 Patch", LauncherBackend.LaunchStock },
            { "CleanOps T7", LauncherBackend.LaunchStock },
            { "BOIII Community", LauncherBackend.LaunchBOIII },
            { "Ezz BOIII", LauncherBackend.LaunchBOIII },
            { "T7x", LauncherBackend.LaunchT7x }
        };


        public HomePage()
        {
            InitializeComponent();

            BeginAnimation(
                OpacityProperty,
                new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250))
            );

            config = ConfigManager.Load();

            UpdateClientAvailability();
            DetectCurrentProfile();
            LoadProfile();
            UpdateExeStatus();
            if (ProfileSelector.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content?.ToString() == "CleanOps T7")
            {
                CleanOpsWarning.Visibility = Visibility.Visible;
            }
            else
            {
                CleanOpsWarning.Visibility = Visibility.Collapsed;
            }
        }


        private void DetectCurrentProfile()
        {
            if (string.IsNullOrWhiteSpace(config.GamePath))
                return;

            string detected = FileManager.DetectAppliedClient(config.GamePath);

            if (detected != "Unknown")
            {
                config.appliedClient = detected;
                ConfigManager.Save(config);
            }
        }


        private void LoadProfile()
        {
            foreach (ComboBoxItem item in ProfileSelector.Items)
            {
                if (item.Content?.ToString() == config.appliedClient)
                {
                    ProfileSelector.SelectedItem = item;
                    return;
                }
            }

            ProfileSelector.SelectedIndex = 0;
        }


        private void UpdateClientAvailability()
        {
            foreach (ComboBoxItem item in ProfileSelector.Items)
            {
                string client = item.Content?.ToString() ?? "";

                item.IsEnabled =
                    client == "Stock BO3" ||
                    FileManager.CheckClient(client);
            }
        }


        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SettingsPage());
        }


        private bool ApplySelectedClient()
        {
            if (ProfileSelector.SelectedItem is not ComboBoxItem item)
                return false;


            string client = item.Content?.ToString() ?? "";


            if (string.IsNullOrWhiteSpace(config.GamePath))
            {
                MessageBox.Show("Please set your Black Ops III folder in Settings.");
                return false;
            }


            if (client != "Stock BO3" && !FileManager.CheckClient(client))
            {
                MessageBox.Show("This client is not installed.");
                return false;
            }


            FileManager.SwapClient(client, config.GamePath);

            UpdateExeStatus();

            ApplyButton.IsEnabled = false;

            return true;
        }


        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileSelector.SelectedItem is not ComboBoxItem item)
                return;


            string client = item.Content?.ToString() ?? "";


            if (client != config.appliedClient)
            {
                if (!ApplySelectedClient())
                    return;
            }


            if (launchActions.TryGetValue(client, out Action? launch))
            {
                launch();
            }
        }


        private void ProfileSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProfileSelector.SelectedItem is not ComboBoxItem item)
                return;

            string selectedClient = item.Content?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(config.GamePath))
            {
                ApplyButton.IsEnabled = true;
                return;
            }


            string detectedClient = FileManager.DetectAppliedClient(config.GamePath);


            ApplyButton.IsEnabled = detectedClient != selectedClient;
            if (ProfileSelector.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content?.ToString() == "CleanOps T7")
            {
                CleanOpsWarning.Visibility = Visibility.Visible;
            }
            else
            {
                CleanOpsWarning.Visibility = Visibility.Collapsed;
            }
        }


        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ApplySelectedClient();
        }


        private void UpdateExeStatus()
        {
            if (string.IsNullOrWhiteSpace(config.GamePath))
            {
                ExeStatusText.Text = "No BO3 path set";
                ExeStatusText.Foreground = Brushes.Gray;
                return;
            }


            string exePath = Path.Combine(
                config.GamePath,
                "BlackOps3.exe"
            );


            string status = ExeChecker.CheckBO3(exePath);


            ExeStatusText.Text = status switch
            {
                "Old" => "Old BO3 executable",
                "New" => "New BO3 executable",
                "Unknown" => "Unknown executable",
                "Missing" => "Missing executable",
                _ => "Unknown"
            };


            ExeStatusText.Foreground =
                ExeStatusColors.TryGetValue(status, out Brush? brush)
                ? brush
                : Brushes.Gray;
        }
    }
}