using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ComputePower.Computation.Models;

namespace ComputePower.Computation
{
    interface IComputation
    {
        DataModel DataModel { get; set; }

        event EventHandler<ComputationProgressEventArgs> ComputationProgress;

        /// <summary>
        /// Executes the parallel data computation
        /// </summary>
        Task ExecuteAsync(double deltaTime);
    }
}
