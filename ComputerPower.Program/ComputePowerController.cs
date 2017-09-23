using System;
using System.IO;
using System.Threading.Tasks;
using ComputePower.Http;
using ComputePower.Http.Models;

namespace ComputePower
{
    public class ComputePowerController
    {
        public event EventHandler<ProgressEventArgs> ProgressHandler;

        private readonly string _path;
        private readonly string _fileName;

        public ComputePowerController()
        {
            _path = @"C:\Users\Public\ComputePower";
            _fileName = "data.json";
        }

        public ComputePowerController(EventHandler<ProgressEventArgs> progressHandler) : this()
        {
            ProgressHandler = progressHandler;
        }

        public void RunAutonomousProgram(string url)
        {
            var downloadManager = new DownloadManager(ProgressHandler);

            downloadManager.DownloadAndSaveFile(url, _path, _fileName);
        }

        public async Task Test(string url, string path, string fileName)
        {
            var dlManager = new DownloadManager(ProgressHandler);
            await dlManager.DownloadAndSaveFile(url, path, fileName);
        }
    }
}
