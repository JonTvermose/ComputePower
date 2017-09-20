using System;

namespace ComputePower.Http.Models
{
    public class ProgressEventArgs : EventArgs
    {
        public double BytesRead { get; }

        public string Message { get; }

        /// <summary>
        /// True = file is done downloading.
        /// </summary>
        public bool IsComplete { get; }

        public ProgressEventArgs(double value)
        {
            BytesRead = value;
        }

        public ProgressEventArgs(double value, string message) : this(value)
        {
            Message = message;
        }

        public ProgressEventArgs(double value, string message, bool completed) : this(value, message)
        {
            IsComplete = completed;
        }
    }
}
