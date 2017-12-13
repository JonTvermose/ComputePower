using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ComputePower.Models;

namespace ComputePower
{
    public interface IComputePowerController
    {
        Task BeginComputation(int projectId, string assemblyPath, string assemblyName, EventHandler<EventArgs> progressUpdateEventHandler);
        Task<List<Project>> DownloadProjects(EventHandler<EventArgs> progressHandler);
        Task<bool> DownloadProjectDll(EventHandler<EventArgs> progressHandler, string dllUrl, string fileName);
    }
}
