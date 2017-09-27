using System;
using System.Threading.Tasks;
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
            downloadManager.DownloadAndSaveFile(url, _path, _fileName, handler);
        }

        public async Task Test(string url, string path, string fileName, EventHandler<ProgressEventArgs> handler)
        {
            var downloadManager = new DownloadManager();
            await downloadManager.DownloadAndSaveFile(url, path, fileName, handler);
        }
    }
}
