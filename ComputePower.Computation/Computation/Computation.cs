using System;
using System.Threading.Tasks;
using ComputePower.NBody.Computation.Models;
using RGiesecke.DllExport;
using System.Runtime.InteropServices;

namespace ComputePower.NBody.Computation
{
    public class Computation : IComputation
    {
        private event EventHandler<ComputationProgressEventArgs> ComputationProgress;

        private readonly Task[] _tasks;

        public Computation()
        {
            _tasks = new Task[Environment.ProcessorCount];
        }

        [DllExport("TestA", CallingConvention = CallingConvention.StdCall)]
        public static int TestA(int a, int b)
        {
            return a + b;
        }

        [DllExport("TestB", CallingConvention = CallingConvention.StdCall)]
        public int TestB(int a, int b)
        {
            return a + b;
        }

        public async Task<Object> ExecuteAsync(params object[] inputObjects)
        {
            DataModel dataModel;
            double deltaTime;
            // Check that DataModel has been set, else generate random data
            if (inputObjects == null)
            {
                dataModel = GenerateRandomData(1000); // !! N^2 complexity!
                deltaTime = 1e11;
            }
            else
            {
                dataModel = (DataModel) inputObjects[0];
                deltaTime = (double) inputObjects[1];
                ComputationProgress += (EventHandler<ComputationProgressEventArgs>) inputObjects[2];
            }
            // Setup timing
            var start = DateTime.Now;

            // How many objects should each task handle
            int chunkSize = (dataModel.Data.Length / _tasks.Length);

            // Create the tasks and begin
            ComputationProgress?.Invoke(this, new ComputationProgressEventArgs("Creating " + _tasks.Length + " threads, each processing " + chunkSize + " elements."));
            for (int i = 0; i < _tasks.Length; i++)
            {
                // Calculate offsets
                int startOffset = chunkSize * i;
                int endOffset = i == _tasks.Length ? dataModel.Data.Length : startOffset + chunkSize;

                // Intitialize and start the tasks
                _tasks[i] = new Task(() => ComputeData(dataModel, startOffset, endOffset, deltaTime));
                _tasks[i].Start();
                ComputationProgress?.Invoke(this, new ComputationProgressEventArgs(i + 1 + " threads started."));
            }

            // Await all threads completion
            foreach (var task in _tasks)
            {
                await task;
            }

            // End timer
            var end = DateTime.Now;

            // Signal the event handler when all threads are completed and data is collected
            var completed = new ComputationProgressEventArgs();
            completed.Progress = 100.0;
            ComputationProgress?.Invoke(this, completed);
            ComputationProgress?.Invoke(this, new ComputationProgressEventArgs("Computation finished in " + (end - start).TotalSeconds + " seconds.", true));
            return dataModel;
        }

        private void ComputeData(DataModel inputData, int startOffset, int endOffset, double deltaTime)
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

                if (startOffset == 0)
                {
                    // Calculate progress and invoke eventhandler
                    double temp = (double)(i - startOffset + 1) / (double)chunkSize * 100.0;
                    if (temp - progress > progressPercentStep)
                    {
                        progress = temp;
                        var args = new ComputationProgressEventArgs();
                        args.Progress = progress;
                        ComputationProgress?.Invoke(this, args);
                    }
                }
            }
        }

        /// <summary>
        /// Generate random test data.
        /// </summary>
        private DataModel GenerateRandomData(int bodies)
        {
            // Dont destroy the processor. N^2 complexity
            if (bodies > 50000)
                bodies = 50000;

            DataModel output = new DataModel();
            output.Data = new Body[bodies];

            ComputationProgress?.Invoke(this, new ComputationProgressEventArgs("Starting to generate random data: " + bodies));

            var solarMass = 1.98892e30;
            var earthMass = solarMass / 333000.0;
            var earthDist = 1.496e17;

            for (int i = 0; i < bodies; i++)
            {
                output.Data[i] = new Body(earthDist, solarMass);
            }
            ComputationProgress?.Invoke(this, new ComputationProgressEventArgs("Random data generated."));
            return output;
        }
    }
}
