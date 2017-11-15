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
    public class ComputePowerController
    {
        public async Task<bool> DownloadFile(string url, string fileName, EventHandler<EventArgs> handler)
        {
            var downloadManager = new DownloadManager();
            downloadManager.Progress += handler;
            string path = Directory.GetCurrentDirectory();
            return await downloadManager.DownloadAndSaveFile(url, path, fileName);
        }

        public ParralelDelegate LoadDllDelegate(string dllPath, string methodName)
        {
            DllLoader dllLoader = new DllLoader();
            return dllLoader.LoadDll(dllPath, methodName);
        }

        public void BeginComputation(string assemblyPath, string assemblyName, EventHandler<EventArgs> progressUpdateEventHandler)
        {
            object result = null;
            try
            {
                // Load the assembly and begin computation
                DllLoader dllLoader = new DllLoader();
                result = dllLoader.CallMethod(assemblyPath, assemblyName, "Execute", progressUpdateEventHandler);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            // Save results to a file
            FileSaver fileSaver = new FileSaver();
            fileSaver.SerializeAndSaveFile(result, Directory.GetCurrentDirectory(), assemblyName);
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
                // TODO error handling
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
            downloadManager.Progress += progressHandler;
            return await downloadManager.DownloadAndSaveFile(dllUrl, path, fileName);
        }

    }
}
