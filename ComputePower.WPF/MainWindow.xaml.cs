using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using ComputePower.Http.Models;
using ComputePower.WPF.Models;

namespace ComputePower.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ComputePowerController _computePowerController;
        private readonly MainViewModel _mainViewModel;
        private readonly ProgressBar _progressBar;
        private readonly Button _beginButton;
        private readonly Button _downloadDllButton;
        private readonly Label _computeLabel;
        private readonly ComboBox _projectsComboBox;

        private bool _isComputing;

        public MainWindow()
        {
            InitializeComponent();

            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;
            _computePowerController = new ComputePowerController();
            _progressBar = (ProgressBar) this.FindName("ProgressBar");
            _beginButton = (Button) this.FindName("BeginButton");
            _downloadDllButton = (Button) this.FindName("DllDownloadButton");
            _computeLabel = (Label) this.FindName("ComputeLabel");
            _projectsComboBox = (ComboBox) this.FindName("ProjectsComboBox");

            _progressBar.Visibility = Visibility.Hidden;
            _computeLabel.Visibility = Visibility.Hidden;
            _downloadDllButton.Visibility = Visibility.Hidden;
            _beginButton.IsEnabled = false;
            _projectsComboBox.IsEnabled = false;
            _isComputing = false;

            GetProjects();
        }

        public async Task GetProjects()
        {
            _mainViewModel.Projects = await _computePowerController.DownloadProjects(UpdateDownloadProgress);
        }

        public void IsComputing()
        {
            _beginButton.IsEnabled = _isComputing ? false : ((Project) _projectsComboBox.SelectedValue).IsDllDownloaded;
            _projectsComboBox.IsEnabled = !_isComputing;
        }

        private void ToggleComputeButton()
        {
            if (_isComputing)
            {
                _beginButton.Content = "Computing";
                _progressBar.Visibility = Visibility.Visible;
                _computeLabel.Visibility = Visibility.Hidden;
            }
            else
            {
                _beginButton.Content = "Begin Computing";
                _progressBar.Visibility = Visibility.Hidden;
                _progressBar.Value = 0.0;
                _computeLabel.Visibility = Visibility.Visible;
            }
        }

        private void BeginComputation(object sender, RoutedEventArgs routedEventArgs)
        {
            _isComputing = !_isComputing;
            IsComputing();
            ToggleComputeButton();
            var directory = AppDomain.CurrentDomain.BaseDirectory;
#if DEBUG
            // remove the \\bin\\debug part of directory if we're running in Visual Studio
            directory = directory.Substring(0, directory.Length - 11);
#endif
            var project = ((Project) _projectsComboBox.SelectedValue);
            if (project == null)
                return;
            var dllName = project.DllName;
            _mainViewModel.ResultList.Add(new TextHolder{Text = ""});
            _mainViewModel.ResultList.Add(new TextHolder { Text = "Starting Project: " + project.Name });
            var t = new Thread(() =>_computePowerController.BeginComputation(directory, dllName, UpdateProgress));
            t.Start();
        }

        public void UpdateProgress(object sender, EventArgs args)
        {
            // Use reflection to find the properties of he ProgressEventArgs
            double progress = args.GetType().GetProperty("Progress") != null ? (double)args.GetType().GetProperty("Progress").GetValue(args, null) : 0.0;
            string message = (string)args.GetType().GetProperty("Message")?.GetValue(args, null);

            // Update UI, must use dispatcher as we are not on main thread
            Dispatcher.Invoke(() =>
            {
                if (progress < 0.1 && message != null)
                {
                    _mainViewModel.ProgressText = message;
                    _mainViewModel.ResultList.Add(new TextHolder {Text = message});
                }
                else
                {
                    _mainViewModel.ProgressText = "Progress: " + progress.ToString(CultureInfo.CurrentCulture) + "%";
                    _mainViewModel.Progress = progress;
                    if (100 - progress < 0.01)
                    {
                        _isComputing = !_isComputing;
                        IsComputing();
                        ToggleComputeButton();
                    }
                }
            });
        }

        public void UpdateDownloadProgress(object sender, ProgressEventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                if (args.IsComplete)
                {
                    _projectsComboBox.IsEnabled = true;
                    var prgsBar = (ProgressBar) FindName("ProjectsProgressBar");
                    prgsBar.Visibility = Visibility.Hidden;
                    var prgsText = (Label) FindName("ProjectsDownloadLabel");
                    prgsText.Visibility = Visibility.Hidden;
                    _mainViewModel.ResultList.Add(new TextHolder { Text = args.Message });
                }
                else if (Math.Abs(args.BytesRead) > 0.00001)
                {
                    _mainViewModel.ProjectsProgress = args.BytesRead + "kb downloaded";
                    _mainViewModel.ResultList.Add(new TextHolder {Text = args.BytesRead + "kb downloaded"});
                }
                else
                {
                    _mainViewModel.ResultList.Add(new TextHolder { Text = args.Message });
                }
            });

        }

        private void ProjectsComboBox_OnDropDownClosed(object sender, EventArgs e)
        {
            _downloadDllButton.Visibility = Visibility.Hidden;

            var project = (Project) _projectsComboBox.SelectedValue;
            if (project != null)
            {
                _downloadDllButton.IsEnabled = !project.IsDllDownloaded;

                if (!project.IsDllDownloaded)
                {
                    _downloadDllButton.Visibility = Visibility.Visible;
                }
                _beginButton.IsEnabled = _isComputing ? false : project.IsDllDownloaded;
            }
        }

        private void DownloadDll(object sender, RoutedEventArgs routedEventArgs)
        {
            _downloadDllButton.IsEnabled = false;

            var prgsBar = (ProgressBar)FindName("ProjectsProgressBar");
            prgsBar.Visibility = Visibility.Visible;

            var project = (Project)_projectsComboBox.SelectedValue;
            _computePowerController.DownloadProjectDll(UpdateDllDownloadProgress, project.DllUrl, project.DllName);
        }

        public void UpdateDllDownloadProgress(object sender, ProgressEventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                if (args.IsComplete)
                {
                    _downloadDllButton.Visibility = Visibility.Hidden;
                    var prgsBar = (ProgressBar)FindName("ProjectsProgressBar");
                    prgsBar.Visibility = Visibility.Hidden;
                    _mainViewModel.ResultList.Add(new TextHolder { Text = args.Message });
                    var project = (Project)_projectsComboBox.SelectedValue;
                    project.IsDllDownloaded = true;
                    _beginButton.IsEnabled = true;
                }
                else if (Math.Abs(args.BytesRead) > 0.00001)
                {
                    _mainViewModel.ProjectsProgress = args.BytesRead + "kb downloaded";
                    _mainViewModel.ResultList.Add(new TextHolder { Text = args.BytesRead + "kb downloaded" }); // TODO this may not be needed
                }
                else
                {
                    _mainViewModel.ResultList.Add(new TextHolder { Text = args.Message });
                }
            });
        }
    }
    
}
