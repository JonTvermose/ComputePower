using System;
using System.Threading;
using System.Threading.Tasks;
using ComputePower.Computation.Models;

namespace ComputePower.Computation
{
    public class CpuComputation : IComputation
    {
        public Object Result { get; set; }

        public event EventHandler<ComputationProgressEventArgs> ComputationProgress;

        private readonly Task[] _tasks;

        public CpuComputation()
        {
            //_tasks = new Task[Environment.ProcessorCount];
            _tasks = new Task[Environment.ProcessorCount];

        }

        public async Task ExecuteAsync(params object[] inputObjects)
        {
            double deltaTime = 1.0;
            // Check that DataModel has been set, else generate random data
            if(inputObjects == null || inputObjects.Length <= 1)
                inputObjects = GenerateRandomData(1000); // !! N^2 complexity!

            var start = DateTime.Now;
            // How many objects should each task handle
            int chunkSize = (((DataModel) inputObjects[1]).Data.Length / _tasks.Length);

            ComputationProgress?.Invoke(this, new ComputationProgressEventArgs("Creating " + _tasks.Length + " threads, each processing " + chunkSize + " elements."));
            for (int i = 0; i < _tasks.Length; i++)
            {
                // Calculate offsets
                int startOffset = chunkSize * i;
                int endOffset = i == _tasks.Length ? ((DataModel)inputObjects[1]).Data.Length : startOffset + chunkSize;

                // Intitialize and start the tasks
                _tasks[i] = new Task(() => ComputeData(((DataModel)inputObjects[1]), startOffset, endOffset, deltaTime, i));
                _tasks[i].Start();
                ComputationProgress?.Invoke(this, new ComputationProgressEventArgs(i + 1 + " threads started."));
            }

            // Await all threads completion
            foreach (var task in _tasks)
            {
                await task;
            }

            var end = DateTime.Now;

            // Signal the event handler when all threads are completed and data is collected
            ComputationProgress?.Invoke(this, new ComputationProgressEventArgs("Computation finished in " + (end - start).TotalSeconds + " seconds.", true));
            Result = inputObjects[1];
        }

        private void ComputeData(DataModel inputData, int startOffset, int endOffset, double deltaTime, int threadId)
        {
            ComputationProgress?.Invoke(this, new ComputationProgressEventArgs("Initiating computation from " + startOffset + " to " + endOffset));
            int chunkSize = endOffset - startOffset;
            double progress = 0.0;
            double progressPercentStep = 5.0; // How often should the eventhandler be invoked with progress updates?

            // Run calculations on subset of the data
            for (int i = startOffset; i < endOffset; i++)
            {
                // Reset Force for each iteration
                inputData.Data[i].ResetForce();

                // Add force for all elements in the system
                for (int j = 0; j<inputData.Data.Length; j++)
                {
                    if(i != j)
                        inputData.Data[i].AddForce(inputData.Data[j]);
                }
                // Update position & velocity
                inputData.Data[i].Update(deltaTime);

                // Calculate progress and invoke eventhandler
                double temp = (double) (i - startOffset + 1) / (double) chunkSize * 100.0;
                if (temp - progress > progressPercentStep)
                {
                    progress = temp;
                    var args = new ComputationProgressEventArgs();
                    args.Progress = progress;
                    args.ThreadId = threadId;
                    ComputationProgress?.Invoke(this, args);
                }
            }
            var completed = new ComputationProgressEventArgs();
            completed.Progress = 100.0;
            completed.ThreadId = threadId;
            ComputationProgress?.Invoke(this, completed);
        }

        /// <summary>
        /// Generate random test data.
        /// </summary>
        private object[] GenerateRandomData(int bodies)
        {
            object[] output = new object[2];
            // Dont destroy the processor. N^2 complexity
            if (bodies > 50000)
                bodies = 50000;

            ComputationProgress?.Invoke(this, new ComputationProgressEventArgs("Starting to generate random data: " + bodies));

            output[1] = new DataModel();
            ((DataModel)output[1]).Data = new Body[bodies];

            var solarMass = 1.98892e30;
            var earthMass = solarMass / 333000.0;
            var earthDist = 1.496e17;

            for (int i = 0; i < bodies; i++)
            {
                ((DataModel)output[1]).Data[i] = new Body(earthDist, solarMass);
            }
            ComputationProgress?.Invoke(this, new ComputationProgressEventArgs("Random data generated."));
            return output;
        }
    }
}
