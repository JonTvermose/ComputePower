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
            _computeLabel = (Label) this.FindName("ComputeLabel");
            _projectsComboBox = (ComboBox) this.FindName("ProjectsComboBox");

            _progressBar.Visibility = Visibility.Hidden;
            _computeLabel.Visibility = Visibility.Hidden;
            _beginButton.IsEnabled = false;
            _projectsComboBox.IsEnabled = false;
            _isComputing = false;

            _mainViewModel.Projects = _computePowerController.DownloadProjects(UpdateDownloadProgress);
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
            // remove the \\bin\\debug part of directory
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
                    _projectsComboBox.IsEnabled = true;
                }
                else
                {
                    _mainViewModel.ProjectsProgress = args.BytesRead + "kb downloaded";
                }
            });

        }

        private void ProjectsComboBox_OnDropDownClosed(object sender, EventArgs e)
        {
            var project = (Project) _projectsComboBox.SelectedValue;
            if (project != null)
            {
                project.IsDllDownloaded = true;
                _beginButton.IsEnabled = _isComputing ? false : project.IsDllDownloaded;
            }
        }
    }
    
}
