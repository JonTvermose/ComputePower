﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ComputePower.WPF.Models
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _progressText;
        private double _progress;

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
            _progressText = "Placeholder text here";
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}