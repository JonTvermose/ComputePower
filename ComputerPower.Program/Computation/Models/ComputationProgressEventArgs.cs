using System;
using System.Collections.Generic;
using System.Text;

namespace ComputePower.Computation.Models
{
    public class ComputationProgressEventArgs : EventArgs
    {
        public string Message { get; }
        public bool ComputationCompleted { get; }

        public ComputationProgressEventArgs(string message)
        {
            Message = message;
        }

        public ComputationProgressEventArgs(string message, bool computationCompleted)
        {
            Message = message;
            ComputationCompleted = computationCompleted;
        }

    }
}
