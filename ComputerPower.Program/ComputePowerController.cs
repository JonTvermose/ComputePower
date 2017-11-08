﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ComputePower.Computation.Models;
using ComputePower.Helpers;
using ComputePower.Http;
using ComputePower.Http.Models;
using Newtonsoft.Json;

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

        public async Task TestB(string url, string path, string fileName, EventHandler<ProgressEventArgs> handler)
        {
            //FileLoader<DataModel> loader = new FileLoader<DataModel>();
            //DataModel output = new DataModel();
            //var filepath = Directory.GetCurrentDirectory() + "\\results.json";
            //loader.LoadFromFileSystem(filepath, out output);

            //IComputation c = new CpuComputation();
            //c.ComputationProgress += ComputationProgressWriter;
            //await c.ExecuteAsync(output, 1e11);
            
            //FileSaver fileSaver = new FileSaver();
            //fileSaver.SerializeAndSaveFile(c.Result, path);
        }

        public async Task<bool> DownloadFile(string url, string fileName, EventHandler<ProgressEventArgs> handler)
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
        public ObservableCollection<Project> DownloadProjects(EventHandler<ProgressEventArgs> progressHandler)
        {
            // Download & return list - TODO

            progressHandler.Invoke(this, new ProgressEventArgs(616));
            var list = new List<Project>()
            {
                new Project
                {
                    Name = "Comet search",
                    Description = "A seach for comets in our solar system",
                    DllUrl = "",
                    WebsiteUrl = "http://nasa.org",
                    DllName = "ComputePower.CPUComputation"
                },
                new Project
                {
                    Name = "Asteroid search",
                    Description = "A seach for asteroids in our solar system",
                    DllUrl = "",
                    WebsiteUrl = "http://nasa.org",
                    DllName = "ComputePower.CPUComputation"
                }
            };
            progressHandler.Invoke(this, new ProgressEventArgs(100, "Download complete", true));
            return new ObservableCollection<Project>(list);
        }
    }
}
