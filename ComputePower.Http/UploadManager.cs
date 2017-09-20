using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ComputePower.Http
{
    public class UploadManager
    {
        public async Task OpenAndUploadFile(string filePath, string url)
        {
            var httpClient = new HttpClient();
            var content = new MultipartFormDataContent();

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                content.Add(new StreamContent(fileStream));
                content.Headers.Add("txt", "1");

                await httpClient.PostAsync(url, content);
            }
        }
    }
}
