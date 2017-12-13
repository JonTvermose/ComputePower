using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using ComputePower.Helpers;
using ComputePower.Http;
using ComputePower.Http.Models;
using ComputePower.Models;

namespace ComputePower
{
    public class ComputePowerController : IComputePowerController
    {
        private string _data;
        private int _currentProjectId;
        private int _cycles = 0;

        public async Task BeginComputation(int projectId, string assemblyPath, string assemblyName, EventHandler<EventArgs> progressUpdateEventHandler)
        {
            string result = null;
            if (string.IsNullOrWhiteSpace(_data) || _currentProjectId != projectId)
            {
                // Download the data needed for the calculation
                _data = await DownloadProjectData(progressUpdateEventHandler, projectId);
                _currentProjectId = projectId;
            }
            try
            {
                // Load the assembly and begin computation
                DllLoader dllLoader = new DllLoader();
                result = dllLoader.CallMethod(assemblyPath, assemblyName, ConfigurationManager.AppSettings.Get("MethodName"), progressUpdateEventHandler, _data);
                _cycles++;
            }
            catch (Exception e)
            {
                ProgressEventArgs args = new ProgressEventArgs(0, "Exception occured loading DLL");
                args.Exception = e;
                progressUpdateEventHandler?.Invoke(this, args);
            }

            // Save results to a file, should be posted to the server. TODO - Currently disabled to not fill up the harddrive
            //FileSaver fileSaver = new FileSaver();
            //fileSaver.SerializeAndSaveFile(result, Directory.GetCurrentDirectory(), assemblyName + _cycles);
            progressUpdateEventHandler?.Invoke(this, new ComputationProgressEventArgs("completed"));
        }

        // Download the list of projects and parse them to objects
        public async Task<List<Project>> DownloadProjects(EventHandler<EventArgs> progressHandler)
        {
            // Download file
            var downloadManager = new DownloadManager();
            string path = Directory.GetCurrentDirectory();
            var url = ConfigurationManager.AppSettings.Get("projectsUrl");
            var fileName = ConfigurationManager.AppSettings.Get("projectsName");
            downloadManager.Progress += progressHandler;
            var result = await downloadManager.DownloadAndSaveFile(url, path, fileName);

            if (!result)
            {
                ProgressEventArgs args = new ProgressEventArgs(0, "An error occured downloading project list");
                progressHandler?.Invoke(this, args);
                return null;
            }

            // Read and parse file
            var filePath = path + fileName;
            var fileLoader = new FileLoader<Project[]>();
            Project[] projects;
            fileLoader.LoadFromFileSystem(filePath, out projects);
            progressHandler?.Invoke(this, new ProgressEventArgs(0.0, "Files loaded into memory."));
            return new List<Project>(projects);
        }

        public async Task<bool> DownloadProjectDll(EventHandler<EventArgs> progressHandler, string dllUrl, string fileName)
        {
            var downloadManager = new DownloadManager();
            string path = Directory.GetCurrentDirectory();
#if DEBUG
            path = path.Substring(0, path.Length - 9); // Dont save in the /bin/debug folder.
#endif

            downloadManager.Progress += progressHandler;
            return await downloadManager.DownloadAndSaveFile(dllUrl, path, fileName + ".dll");
        }

        private async Task<string> DownloadProjectData(EventHandler<EventArgs> progressHandler, int projectId)
        {
            var downloadManager = new DownloadManager();
            string url = ConfigurationManager.AppSettings.Get("projectsUrl") + "/" + projectId + "/data";
            downloadManager.Progress += progressHandler;
            return await downloadManager.GetUrl(url);
        }

    }
}
