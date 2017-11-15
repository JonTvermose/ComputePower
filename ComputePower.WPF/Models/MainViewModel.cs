using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ComputePower.WPF.Models
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _progressText;
        private double _progress;
        private string _projectsProgress;
        private ObservableCollection<ProjectViewModel> _projects;
        private ObservableCollection<TextHolder> _resultList;

        public ObservableCollection<TextHolder> ResultList
        {
            get { return _resultList; }
            set
            {
                if (value != _resultList)
                {
                    _resultList = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ProjectViewModel> Projects
        {
            get { return _projects; }
            set
            {
                if (value != _projects)
                {
                    _projects = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ProgressText
        {
            get { return _progressText; }
            set
            {
                if (value != _progressText)
                {
                    _progressText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ProjectsProgress
        {
            get { return _projectsProgress; }
            set
            {
                if (value != _projectsProgress)
                {
                    _projectsProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Progress
        {
            get { return _progress; }
            set
            {
                if (Math.Abs(value - _progress) > 0.01)
                {
                    _progress = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainViewModel()
        {
            _progress = 0;
            _progressText = "";
            _projects = new ObservableCollection<ProjectViewModel>();
            _resultList = new ObservableCollection<TextHolder>();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
