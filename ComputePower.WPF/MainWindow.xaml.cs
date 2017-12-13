using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AutoMapper.QueryableExtensions;
using ComputePower.WPF.Models;

namespace ComputePower.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Declarations
        private readonly IComputePowerController _computePowerController;
        private readonly MainViewModel _mainViewModel;
        private readonly ProgressBar _progressBar;
        private readonly Button _beginButton;
        private readonly Button _downloadDllButton;
        private readonly Label _computeLabel;
        private readonly ComboBox _projectsComboBox;
        private readonly Button _stopButton;

        private bool _isComputing;
        private bool _stopComputing;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            ComputeMapper.Initialize(); // Init automapper

            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;
            _computePowerController = new ComputePowerController();
            _progressBar = (ProgressBar) this.FindName("ProgressBar");
            _beginButton = (Button) this.FindName("BeginButton");
            _downloadDllButton = (Button) this.FindName("DllDownloadButton");
            _computeLabel = (Label) this.FindName("ComputeLabel");
            _projectsComboBox = (ComboBox) this.FindName("ProjectsComboBox");
            _stopButton = (Button) this.FindName("StopButton");

            _progressBar.Visibility = Visibility.Hidden;
            _computeLabel.Visibility = Visibility.Hidden;
            _downloadDllButton.Visibility = Visibility.Hidden;
            _beginButton.IsEnabled = false;
            _projectsComboBox.IsEnabled = false;
            _isComputing = false;
            _stopButton.Visibility = Visibility.Hidden;
            
            GetProjects();
        }

        #region HelperMethods

        private void ToggleComputeButton()
        {
            if (_isComputing)
            {
                _beginButton.Content = "Computing";
                _progressBar.Visibility = Visibility.Visible;
                _computeLabel.Visibility = Visibility.Hidden;
                _stopButton.Visibility = Visibility.Visible;
            }
            else
            {
                _beginButton.Content = "Begin Computing";
                _progressBar.Visibility = Visibility.Hidden;
                _progressBar.Value = 0.0;
                _computeLabel.Visibility = Visibility.Visible;
                _stopButton.Visibility = Visibility.Hidden;
            }
        }

        public void IsComputing()
        {
            _beginButton.IsEnabled = _isComputing ? false : ((ProjectViewModel)_projectsComboBox.SelectedValue).IsDllDownloaded;
            _projectsComboBox.IsEnabled = !_isComputing;
        }

        public async Task GetProjects()
        {
            var projects = (await _computePowerController.DownloadProjects(UpdateDownloadProgress))?.AsQueryable().ProjectTo<ProjectViewModel>();
            if (projects != null)
            {
                _mainViewModel.Projects = new ObservableCollection<ProjectViewModel>(projects);
            }
        }

        #endregion
        
        #region ClickHandlers

        private void BeginComputation(object sender, RoutedEventArgs routedEventArgs)
        {
            _stopComputing = false;
            _isComputing = !_isComputing;
            IsComputing();
            ToggleComputeButton();
            var directory = AppDomain.CurrentDomain.BaseDirectory;
#if DEBUG
            // remove the \\bin\\debug part of directory if we're running in Visual Studio
            directory = directory.Substring(0, directory.Length - 11);
#endif
            var project = ((ProjectViewModel)_projectsComboBox.SelectedValue);
            if (project == null)
                return;
            var dllName = project.DllName;
            _mainViewModel.ResultList.Add(new TextHolder { Text = "" });
            _mainViewModel.ResultList.Add(new TextHolder { Text = "Starting Project: " + project.Name });
            var t = new Thread(() => _computePowerController.BeginComputation(project.Id, directory, dllName, UpdateProgress));
            t.Start();
        }

        private void StopComputation(object sender, RoutedEventArgs routedEventArgs)
        {
            _stopComputing = true;
            _stopButton.IsEnabled = false;
            _stopButton.Content = "Stopping...";
        }

        private void ProjectsComboBox_OnDropDownClosed(object sender, EventArgs e)
        {
            _downloadDllButton.Visibility = Visibility.Hidden;

            var project = (ProjectViewModel)_projectsComboBox.SelectedValue;
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


        #endregion

        #region ProgressHandlers

        public void UpdateProgress(object sender, EventArgs args)
        {
            // Use reflection to find the properties of he ProgressEventArgs
            double progress = args.GetType().GetProperty("Progress") != null
                ? (double) args.GetType().GetProperty("Progress").GetValue(args, null)
                : 0.0;
            string message = (string) args.GetType().GetProperty("Message")?.GetValue(args, null);

            // Update UI, must use dispatcher as we are not on main thread
            Dispatcher.Invoke(() =>
            {
                if (progress < 0.1 && message != null && message != "completed")
                {
                    _mainViewModel.ProgressText = message;
                    _mainViewModel.ResultList.Add(new TextHolder { Text = message });
                }
                else
                {
                    _mainViewModel.ProgressText = "Progress: " + progress.ToString(CultureInfo.CurrentCulture) + "%";
                    _mainViewModel.Progress = progress;
                    if (!string.IsNullOrWhiteSpace(message) && message == "completed")
                    {
                        //var synthesizer = new SpeechSynthesizer();
                        //synthesizer.Rate = 2;
                        //synthesizer.SpeakAsync("Cycle complete");
                        _isComputing = !_isComputing;
                        IsComputing();
                        ToggleComputeButton();
                        if (!_stopComputing)
                        {
                            BeginComputation(null, null);
                        }
                        else
                        {
                            _stopButton.IsEnabled = true;
                            _stopButton.Content = "Stop Computation";
                            // TODO - upload data here
                        }
                    }
                }
            });
        }

        public void UpdateDownloadProgress(object sender, EventArgs args)
        {
            // Use reflection to find the properties of he ProgressEventArgs
            double bytesRead = args.GetType().GetProperty("BytesRead") != null ? (double)args.GetType().GetProperty("BytesRead").GetValue(args, null) : 0.0;
            string message = (string)args.GetType().GetProperty("Message")?.GetValue(args, null);
            bool isComplete = (bool)args.GetType().GetProperty("IsComplete")?.GetValue(args, null);

            Dispatcher.Invoke(() =>
            {
                if (isComplete)
                {
                    _projectsComboBox.IsEnabled = true;
                    var prgsBar = (ProgressBar)FindName("ProjectsProgressBar");
                    prgsBar.Visibility = Visibility.Hidden;
                    var prgsText = (Label)FindName("ProjectsDownloadLabel");
                    prgsText.Visibility = Visibility.Hidden;
                    _mainViewModel.ResultList.Add(new TextHolder { Text = message });
                }
                else if (Math.Abs(bytesRead) > 0.00001)
                {
                    _mainViewModel.ProjectsProgress = bytesRead + "kb downloaded";
                    _mainViewModel.ResultList.Add(new TextHolder { Text = bytesRead + "kb downloaded" });
                }
                else
                {
                    _mainViewModel.ResultList.Add(new TextHolder { Text = message });
                }
            });
        }

        public void UpdateDllDownloadProgress(object sender, EventArgs args)
        {
            // Use reflection to find the properties of he ProgressEventArgs
            double bytesRead = args.GetType().GetProperty("BytesRead") != null ? (double)args.GetType().GetProperty("BytesRead").GetValue(args, null) : 0.0;
            string message = (string)args.GetType().GetProperty("Message")?.GetValue(args, null);
            bool isComplete = (bool)args.GetType().GetProperty("IsComplete")?.GetValue(args, null);
            Exception exception = (Exception)args.GetType().GetProperty("Exception")?.GetValue(args, null);

            Dispatcher.Invoke(() =>
            {
                if (exception != null)
                {
                    var prgsBar = (ProgressBar)FindName("ProjectsProgressBar");
                    prgsBar.Visibility = Visibility.Hidden;
                    _mainViewModel.ResultList.Add(new TextHolder { Text = "ERROR: " + exception.Message });
                    _downloadDllButton.IsEnabled = true;
                }
                else if (isComplete)
                {
                    _downloadDllButton.Visibility = Visibility.Hidden;
                    var prgsBar = (ProgressBar)FindName("ProjectsProgressBar");
                    prgsBar.Visibility = Visibility.Hidden;
                    _mainViewModel.ResultList.Add(new TextHolder { Text = message });
                    var project = (ProjectViewModel)_projectsComboBox.SelectedValue;
                    project.IsDllDownloaded = true;
                    _beginButton.IsEnabled = true;
                }
                else if (Math.Abs(bytesRead) > 0.00001)
                {
                    _mainViewModel.ProjectsProgress = bytesRead + "kb downloaded";
                    _mainViewModel.ResultList.Add(new TextHolder { Text = bytesRead + "kb downloaded" }); // TODO this may not be needed
                }
                else
                {
                    _mainViewModel.ResultList.Add(new TextHolder { Text = message });
                }
            });
        }

        private void DownloadDll(object sender, RoutedEventArgs routedEventArgs)
        {
            _downloadDllButton.IsEnabled = false;

            Dispatcher.Invoke(() =>
            {
                var prgsBar = (ProgressBar)FindName("ProjectsProgressBar");
                prgsBar.Visibility = Visibility.Visible;

                var project = (ProjectViewModel)_projectsComboBox.SelectedValue;
                _computePowerController.DownloadProjectDll(UpdateDllDownloadProgress, project.DllUrl, project.DllName);
            });
        }

        #endregion
        
    }
}