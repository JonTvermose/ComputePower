using System;
using System.Collections.Generic;
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

        public MainWindow()
        {
            InitializeComponent();

            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;
            _computePowerController = new ComputePowerController();
            _progressBar = (ProgressBar) this.FindName("ProgressBar");
            _beginButton = (Button) this.FindName("BeginButton");
            _progressBar.Visibility = Visibility.Hidden;
            _computeLabel = (Label) this.FindName("ComputeLabel");
            _computeLabel.Visibility = Visibility.Hidden;
            _projectsComboBox = (ComboBox) this.FindName("ProjectsComboBox");
            // _projectsComboBox.ItemsSource = ?? ; // TODO - list of projects
        }

        private void ToggleComputeButton()
        {
            if (_beginButton.IsEnabled)
            {
                _beginButton.Content = "Computing";
                _beginButton.IsEnabled = false;
                _progressBar.Visibility = Visibility.Visible;
                _computeLabel.Visibility = Visibility.Hidden;
            }
            else
            {
                _beginButton.Content = "Begin Computing";
                _beginButton.IsEnabled = true;
                _progressBar.Visibility = Visibility.Hidden;
                _progressBar.Value = 0.0;
                _computeLabel.Visibility = Visibility.Visible;
            }
        }

        public void BeginComputation(object sender, RoutedEventArgs routedEventArgs)
        {
            ToggleComputeButton();
            var directory = AppDomain.CurrentDomain.BaseDirectory;
#if DEBUG
            // remove the \\bin\\debug part of directory
            directory = directory.Substring(0, directory.Length - 11);
#endif

            var t = new Thread(() =>_computePowerController.BeginComputation(directory, "ComputePower.CPUComputation", UpdateProgress));
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
                }
                else
                {
                    _mainViewModel.ProgressText = "Progress: " + progress.ToString(CultureInfo.CurrentCulture) + "%";
                    _mainViewModel.Progress = progress;
                    //_progressBar.Value = progress;
                    if (100 - progress < 0.01)
                        ToggleComputeButton();
                }
            });
        }
    }
    
}
