using System;
using ComputePower.NBody.Computation.Models;
using System.Threading;

namespace ComputePower.NBody.Computation
{
    public class Computation : IComputation
    {
        private readonly Thread[] _threads;

        public Computation()
        {
            _threads = new Thread[Environment.ProcessorCount];
        }
        public Object Execute(EventHandler<EventArgs> progressHandler)
        {
            DataModel dataModel;
            double deltaTime;
            // Check that DataModel has been set, else generate random data
            dataModel = GenerateRandomData(1000, progressHandler); // !! N^2 complexity!
            deltaTime = 1e11;

            // Setup timing
            var start = DateTime.Now;

            // How many objects should each task handle
            int chunkSize = (dataModel.Data.Length / _threads.Length);

            // Create the tasks and begin
            progressHandler?.Invoke(this, new ComputationProgressEventArgs("Creating " + _threads.Length + " threads, each processing " + chunkSize + " elements."));
            for (int i = 0; i < _threads.Length; i++)
            {
                // Calculate offsets
                int startOffset = chunkSize * i;
                int endOffset = i == _threads.Length ? dataModel.Data.Length : startOffset + chunkSize;

                // Intitialize and start the tasks
                var i1 = i;
                _threads[i] = new Thread(() => ComputeData(dataModel, startOffset, endOffset, deltaTime, i1 == 0 ? progressHandler : null));
                _threads[i].Start();
                progressHandler?.Invoke(this, new ComputationProgressEventArgs(i + 1 + " threads started."));
            }

            // Await all threads completion
            foreach (var thread in _threads)
            {
                thread.Join();
            }

            // End timer
            var end = DateTime.Now;

            // Signal the event handler when all threads are completed and data is collected
            var completed = new ComputationProgressEventArgs();
            completed.Progress = 100.0;
            progressHandler?.Invoke(this, completed);
            progressHandler?.Invoke(this, new ComputationProgressEventArgs("Computation finished in " + (end - start).TotalSeconds + " seconds."));
            return dataModel;
        }

        private void ComputeData(DataModel inputData, int startOffset, int endOffset, double deltaTime, EventHandler<EventArgs> progressHandler)
        {
            progressHandler?.Invoke(this, new ComputationProgressEventArgs("Initiating computation from " + startOffset + " to " + endOffset));
            int chunkSize = endOffset - startOffset;
            double progress = 0.0;
            double progressPercentStep = 5.0; // How often should the eventhandler be invoked with progress updates?

            // Run calculations on subset of the data
            for (int i = startOffset; i < endOffset; i++)
            {
                // Reset Force for each iteration
                inputData.Data[i].ResetForce();

                // Add force for all elements in the system
                for (int j = 0; j < inputData.Data.Length; j++)
                {
                    if (i != j)
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
                        progressHandler?.Invoke(this, new ComputationProgressEventArgs(progress));
                    }
                }
            }
        }

        /// <summary>
        /// Generate random test data.
        /// </summary>
        private DataModel GenerateRandomData(int bodies, EventHandler<EventArgs> progressHandler)
        {
            // Dont destroy the processor. N^2 complexity
            if (bodies > 50000)
                bodies = 50000;

            DataModel output = new DataModel();
            output.Data = new Body[bodies];

            progressHandler?.Invoke(this, new ComputationProgressEventArgs("Starting to generate random data: " + bodies));

            var solarMass = 1.98892e30;
            var earthMass = solarMass / 333000.0;
            var earthDist = 1.496e17;

            for (int i = 0; i < bodies; i++)
            {
                output.Data[i] = new Body(earthDist, solarMass);
            }
            progressHandler?.Invoke(this, new ComputationProgressEventArgs("Random data generated."));
            return output;
        }
    }
}
