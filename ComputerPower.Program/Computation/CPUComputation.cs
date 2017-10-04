using System;
using System.Threading.Tasks;
using ComputePower.Computation.Models;

namespace ComputePower.Computation
{
    public class CpuComputation : IComputation
    {
        public DataModel DataModel { get; set; }

        public event EventHandler ComputationCompleted;

        private readonly Task[] _tasks;
        private object[] _results;

        public CpuComputation()
        {
            _tasks = new Task[Environment.ProcessorCount - 1];
            _results = new object[_tasks.Length];
        }

        public async Task ExecuteAsync()
        {
            // Check that DataModel has been set
            if(DataModel == null)
                throw new ArgumentNullException(nameof(DataModel));

            // How many objects should each task handle
            int chunkSize = (DataModel.Data.Length / _tasks.Length);

            for (int i = 0; i < _tasks.Length; i++)
            {
                // Calculate offsets
                int startOffset = chunkSize * i;
                int endOffset = i == _tasks.Length ? DataModel.Data.Length : startOffset + chunkSize;

                // Intitialize and start the tasks
                _tasks[i] = new Task(() => ComputeData(DataModel, startOffset, endOffset, i));
                _tasks[i].Start();
            }

            // Await all threads completion
            foreach (var task in _tasks)
            {
                await task;
            }

            // Collect data into a single object
            CollectData();

            // Signal the event handler when all threads are completed and data is collected
            ComputationCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void ComputeData(DataModel inputData, int startOffset, int endOffset, int i)
        {
            // Run calculations

            // Save computed data in _results[i]
        }

        private void CollectData()
        {
            DataModel = new DataModel();
        }
    }
}
