using ComputePower.NBody.Computation.Models;
using System;
using System.Threading.Tasks;

namespace ComputePower.NBody
{
    interface IComputation
    {
        /// <summary>
        /// Executes the parallel data computation
        /// </summary>
        Object Execute(EventHandler<EventArgs> progressHandler);
    }
}
