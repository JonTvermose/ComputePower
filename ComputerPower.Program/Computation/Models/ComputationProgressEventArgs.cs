using System;
using System.Collections.Generic;
using System.Text;

namespace ComputePower.Computation.Models
{
    public class ComputationProgressEventArgs : EventArgs
    {
        public string Message { get; set; }
        public bool ComputationCompleted { get; set; }
        public double Progress { get; set; }
        public int ThreadId { get; set; }

        public ComputationProgressEventArgs()
        {
        }

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
