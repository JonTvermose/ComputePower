using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ComputePower.Http.Models;

namespace ComputePower.Http
{
    public class DownloadManager
    {
        public event EventHandler<ProgressEventArgs> ProgressHandler;

        public DownloadManager()
        {
        }

        public DownloadManager(EventHandler<ProgressEventArgs> handler)
        {
            ProgressHandler = handler;
        }

        public async Task<bool> DownloadAndSaveFile(string url, string path, string fileName)
        {
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
                        ProgressHandler?.Invoke(this, new ProgressEventArgs(0, "Download started.", false));

                        do
                        {
                            var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                            if (read == 0)
                            {
                                isMoreToRead = false;
                                ProgressHandler?.Invoke(this, new ProgressEventArgs((double) totalRead/1000, "Download complete.", true));
                            }
                            else
                            {
                                await fileStream.WriteAsync(buffer, 0, read);

                                totalRead += read;
                                ProgressHandler?.Invoke(this, new ProgressEventArgs((double) totalRead/1000));
                            }
                        }
                        while (isMoreToRead);
                    }
                }
            }
            return !isMoreToRead;
        }
    }
}
