﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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

        public async Task<bool> BeginComputation(string assemblyPath, string assemblyName, params object[] dataObjects)
        {
            dataObjects = new object[2];
            dataObjects[0] = 1;
            dataObjects[1] = 3;

            var test = TestB(1, 3);
            object result = null;
            try
            {
                // Load the assembly and begin computation
                DllLoader dllLoader = new DllLoader();
                result = await dllLoader.CallMethod(assemblyPath, assemblyName, "Test", dataObjects);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);                
            }


            // Save results to a file
            FileSaver fileSaver = new FileSaver();
            fileSaver.SerializeAndSaveFile(result, Directory.GetCurrentDirectory(), assemblyName);

            return true;
        }

        [DllImport("ComputePower.NBody.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int TestA(int a, int b);

        [DllImport("ComputePower.NBody.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int TestB(int a, int b);

        private void ComputationProgressWriter(object sender, ComputationProgressEventArgs args)
        {
            if (Math.Abs(args.Progress) < 0.1)
            {
                Console.WriteLine(args.Message);
            }
            else
            {
                Console.WriteLine("Thread progress: {1}", args.Progress);
            }
        }
    }
}
