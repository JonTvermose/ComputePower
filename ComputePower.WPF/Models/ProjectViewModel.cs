using System.ComponentModel;
using System.Runtime.CompilerServices;
using ComputePower.Http.Models;
using ComputePower.WPF.Annotations;

namespace ComputePower.WPF.Models
{
    public class ProjectViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;
        private string _description;
        private string _dllUrl;
        private string _dllName;
        private string _websiteUrl;
        private bool _isDllDownloaded;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DllUrl
        {
            get { return _dllUrl; }
            set
            {
                if (value != _dllUrl)
                {
                    _dllUrl = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DllName
        {
            get { return _dllName; }
            set
            {
                if (value != _dllName)
                {
                    _dllName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string WebsiteUrl
        {
            get { return _websiteUrl; }
            set
            {
                if (value != _websiteUrl)
                {
                    _websiteUrl = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsDllDownloaded
        {
            get { return _isDllDownloaded; }
            set
            {
                if (value != _isDllDownloaded)
                {
                    _isDllDownloaded = value;
                    OnPropertyChanged();
                }
            }
        }

        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Usage: var projectViewModel = (ProjectViewModel) project;
        public static explicit operator ProjectViewModel(Project project)
        {
            ProjectViewModel pvm = new ProjectViewModel();
            pvm.IsDllDownloaded = project.IsDllDownloaded;
            pvm.Description = project.Description;
            pvm.DllName = project.DllName;
            pvm.DllUrl = project.DllUrl;
            pvm.Name = project.Name;
            pvm.WebsiteUrl = project.WebsiteUrl;
            return pvm;
        }
    }
}
