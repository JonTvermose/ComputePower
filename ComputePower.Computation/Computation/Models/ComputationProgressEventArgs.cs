using System;

namespace ComputePower.NBody.Computation.Models
{
    public class ComputationProgressEventArgs : EventArgs
    {
        public string Message { get; set; }
        public double Progress { get; set; }

        public ComputationProgressEventArgs()
        {
        }

        public ComputationProgressEventArgs(string message)
        {
            Message = message;
        }

        public ComputationProgressEventArgs(string message, double progress)
        {
            Message = message;
            Progress = progress;
        }

        public ComputationProgressEventArgs(double progress)
        {
            Progress = progress;
            Message = progress + "% done.";
        }

    }
}
