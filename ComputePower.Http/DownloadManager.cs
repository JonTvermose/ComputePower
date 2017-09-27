using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ComputePower.Http.Models;

namespace ComputePower.Http
{
    public class DownloadManager
    {
        public event EventHandler<ProgressEventArgs> Progress;

        /// <summary>
        /// Download a file from a given URL and save said file to a given path and filename
        /// If the path does not exists it will be created
        /// </summary>
        /// <param name="url">URL where to retrieve the file</param>
        /// <param name="path">Path where to save the downloaded file</param>
        /// <param name="fileName">The downloaded file will be saved using this filename</param>
        /// <returns></returns>
        public async Task<bool> DownloadAndSaveFile(string url, string path, string fileName)
        {
            //Directory.CreateDirectory(path); // Exception in .NET Core 2.0

            var isMoreToRead = true;

            using (var client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result)
                {
                    response.EnsureSuccessStatusCode();
                    
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(), fileStream = new FileStream(path + fileName, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var totalRead = 0L;
                        var buffer = new byte[8192];
                        OnProgress(this, new ProgressEventArgs(0, "Download started.", false));
                        
                        do
                        {
                            var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                            if (read == 0)
                            {
                                isMoreToRead = false;
                                OnProgress(this, new ProgressEventArgs((double) totalRead/1000, "Download complete. File saved to: " + fileName, true));
                            }
                            else
                            {
                                await fileStream.WriteAsync(buffer, 0, read);

                                totalRead += read;
                                OnProgress(this, new ProgressEventArgs((double) totalRead/1000));
                            }
                        }
                        while (isMoreToRead);
                    }
                }
            }
            return !isMoreToRead;
        }

        protected virtual void OnProgress(object sender, ProgressEventArgs args)
        {
            Progress?.Invoke(sender, args);
        }
    }
}
