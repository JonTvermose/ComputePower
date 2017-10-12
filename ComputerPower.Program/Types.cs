using System;
using ComputePower.Computation.Models;

namespace ComputePower
{
    public delegate bool ParralelDelegate(EventHandler<ComputationProgressEventArgs> progressHandler, params object[] inputObjects);

}
