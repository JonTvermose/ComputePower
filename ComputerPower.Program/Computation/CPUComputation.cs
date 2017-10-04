using System;
using System.Threading.Tasks;
using ComputePower.Computation.Models;

namespace ComputePower.Computation
{
    public class CpuComputation : IComputation
    {
        public DataModel DataModel { get; set; }

        public event EventHandler<ComputationProgressEventArgs> ComputationProgress;


        private readonly Task[] _tasks;

        public CpuComputation()
        {
            _tasks = new Task[Environment.ProcessorCount];
        }

        public async Task ExecuteAsync(double deltaTime)
        {
            // Check that DataModel has been set, else generate random data
            if(DataModel == null)
                GenerateRandomData(20000); // !! N^2 complexity!

            var start = DateTime.Now;
            // How many objects should each task handle
            int chunkSize = (DataModel.Data.Length / _tasks.Length);

            ComputationProgress?.Invoke(this, new ComputationProgressEventArgs("Creating " + _tasks.Length + " threads, each processing " + chunkSize + " elements."));
            for (int i = 0; i < _tasks.Length; i++)
            {
                // Calculate offsets
                int startOffset = chunkSize * i;
                int endOffset = i == _tasks.Length ? DataModel.Data.Length : startOffset + chunkSize;

                // Intitialize and start the tasks
                _tasks[i] = new Task(() => ComputeData(DataModel, startOffset, endOffset, deltaTime));
                _tasks[i].Start();
                ComputationProgress?.Invoke(this, new ComputationProgressEventArgs(i + " threads started."));
            }

            // Await all threads completion
            foreach (var task in _tasks)
            {
                await task;
            }

            var end = DateTime.Now;

            // Signal the event handler when all threads are completed and data is collected
            ComputationProgress?.Invoke(this, new ComputationProgressEventArgs("Computation finished in " + (end - start).Milliseconds + " ms.", true));
        }

        private void ComputeData(DataModel inputData, int startOffset, int endOffset, double deltaTime)
        {
            ComputationProgress?.Invoke(this, new ComputationProgressEventArgs("Initiating computation from " + startOffset + " to " + endOffset));
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
            }
            ComputationProgress?.Invoke(this, new ComputationProgressEventArgs("Thread completed!"));
        }

        /// <summary>
        /// Generate random test data.
        /// </summary>
        private void GenerateRandomData(int bodies)
        {
            // Dont destroy the processor. N^2 complexity
            if (bodies > 50000)
                bodies = 50000;

            ComputationProgress?.Invoke(this, new ComputationProgressEventArgs("Starting to generate random data: " + bodies));

            DataModel = new DataModel();
            DataModel.Data = new Body[bodies];

            var solarMass = 1.98892e30;
            var earthMass = solarMass / 333000.0;
            var earthDist = 1.496e17;

            for (int i = 0; i < bodies; i++)
            {
                DataModel.Data[i] = new Body(earthDist, solarMass);
            }
            ComputationProgress?.Invoke(this, new ComputationProgressEventArgs("Random data generated."));
        }
    }
}
