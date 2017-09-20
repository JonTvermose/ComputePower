using System;
using System.Threading.Tasks;
using ComputePower.Http;
using ComputePower.Http.Models;

namespace ComputePower
{
    public class ComputePowerController : IComputePowerController
    {
        public event EventHandler<ProgressEventArgs> ProgressHandler;

        public ComputePowerController()
        {
        }

        public ComputePowerController(EventHandler<ProgressEventArgs> progressHandler)
        {
            ProgressHandler = progressHandler;
        }

        public async Task Test(string url, string path, string fileName)
        {
            var dlManager = new DownloadManager(ProgressHandler);
            await dlManager.DownloadAndSaveFile(url, path, fileName);
        }
    }
}
