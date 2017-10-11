using System;
using System.Threading.Tasks;
using ComputePower.Computation;
using ComputePower.Computation.Models;
using ComputePower.Helpers;
using ComputePower.Http;
using ComputePower.Http.Models;

namespace ComputePower
{
    public class ComputePowerController
    {
        private readonly string _path;
        private readonly string _fileName;

        public ComputePowerController()
        {
            _path = @"C:\Users\Public\ComputePower";
            _fileName = "data.json";
        }

        public void RunAutonomousProgram(string url, EventHandler<ProgressEventArgs> handler)
        {
            var downloadManager = new DownloadManager();
            downloadManager.Progress += handler;
            downloadManager.DownloadAndSaveFile(url, _path, _fileName);
        }

        public async Task Test(string url, string path, string fileName, EventHandler<ProgressEventArgs> handler)
        {
            //var downloadManager = new DownloadManager();
            //downloadManager.Progress += handler;
            //await downloadManager.DownloadAndSaveFile(url, path, fileName);

            IComputation c = new CpuComputation();
            c.ComputationProgress += ComputationProgressWriter;
            await c.ExecuteAsync(1e11);
            
            FileSaver fileSaver = new FileSaver();
            fileSaver.SerializeAndSaveFile(c.Result, path);
        }

        private void ComputationProgressWriter(object sender, ComputationProgressEventArgs args)
        {
            if (Math.Abs(args.Progress) < 0.1)
            {
                Console.WriteLine(args.Message);
            }
            else
            {
                Console.WriteLine("Thread {0} progress: {1}", args.ThreadId, args.Progress);
            }
        }
    }
}
