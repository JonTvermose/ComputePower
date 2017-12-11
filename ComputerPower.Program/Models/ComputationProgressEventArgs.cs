using System;

namespace ComputePower.Models
{
    public class ComputationProgressEventArgs : EventArgs
    {
        public string Message { get; set; }
        public bool ComputationCompleted { get; set; }
        public double Progress { get; set; }

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
