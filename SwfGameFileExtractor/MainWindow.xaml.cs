using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ookii.Dialogs.Wpf;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace SwfGameFileExtractor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string DefaultPath;
        private bool IsDownloading
        {
            get => _isDownloading;
            set
            {
                _isDownloading = value;
                if (_isDownloading)
                {
                    ButtonDownload.Background.Opacity = 0.25;
                    ButtonBrowsePath.Background.Opacity = 0.25;
                    ButtonDownload.IsHitTestVisible = false;
                    ButtonBrowsePath.IsHitTestVisible = false;
                    TextBoxUrl.IsReadOnly = true;
                }
                else
                {
                    ButtonDownload.Background.Opacity = 1;
                    ButtonBrowsePath.Background.Opacity = 1;
                    ButtonDownload.IsHitTestVisible = true;
                    ButtonBrowsePath.IsHitTestVisible = true;
                    TextBoxUrl.IsReadOnly = false;
                }
            }
        }
        private bool _isDownloading;

        public MainWindow()
        {
            InitializeComponent();

            // Top UI portion (_, [], x)
            SourceInitialized += (s, e) => this.FixTheGapIssue();
            ButtonClose.Click += (s, e) => Close();
            ButtonMaximize.Click += (s, e) => WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
            ButtonMinimize.Click += (s, e) => WindowState = WindowState.Minimized;

            DefaultPath = TextBoxPath.Text;
            ProgressBarDownload.Value = 0;
            TextBlockProgressLabel.Text = string.Empty;
        }

        // URL examples:
        // https://games.inbox.lv/mini/game/gold-strike/
        private async void TryDownloadAsync()
        {
            try
            {
                if (TextBoxPath.Text == DefaultPath)
                {
                    throw new Exception("Invalid directory!");
                }

                TextBlockProgressLabel.Text = "Searching for the game file";

                SwfGame game = null;

                // Stuff needed for the download
                if (TextBoxUrl.Text.Contains("games.inbox.lv/mini/game/"))
                    game = await InboxSwfFactory.CreateInstanceFrom(TextBoxUrl.Text);
                else
                    throw new Exception("Invalid input!");

                if (game == null)
                {
                    throw new Exception("Game unavailable!");
                }

                Download(game.SwfFileUrl, @$"{TextBoxPath.Text}\{game.Title}.swf");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                TextBlockProgressLabel.Text = "Error";
            }
        }

        private void Download(string urlToDownloadFrom, string pathToSaveAt)
        {
            using WebClient webClient = new WebClient();

            // Subscribe before downloading
            webClient.DownloadProgressChanged += OnDownloadProgressChanged;
            IsDownloading = true;

            // Unsubscribe after downloading
            webClient.DownloadFileCompleted += (s, e) =>
            {
                webClient.DownloadProgressChanged -= OnDownloadProgressChanged;
                TextBlockProgressLabel.Text = $"Finished";
                IsDownloading = false;
            };

            // Downloading
            webClient.DownloadFileAsync(
                new Uri(urlToDownloadFrom),
                pathToSaveAt);
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressBarDownload.Value = e.ProgressPercentage;
            TextBlockProgressLabel.Text = $"Downloading {e.ProgressPercentage}%";
        }

        private void TextBoxUrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxUrl.Text))
            {
                TextBlockPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                TextBlockPlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e)
        {
            if (IsDownloading)
            {
                return;
            }

            var directoryDialog = new FolderBrowserDialog();
            directoryDialog.RootFolder = Environment.SpecialFolder.Desktop;

            if (directoryDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextBoxPath.Text = directoryDialog.SelectedPath;
            }
        }

        private void ButtonDownload_Click(object sender, RoutedEventArgs e)
        {
            if (IsDownloading)
            {
                return;
            }

            TryDownloadAsync();
        }
    }
}
