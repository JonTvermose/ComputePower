using System;
using System.Net.Http;
using System.Threading.Tasks;
using ComputePower.Http;
using Xunit;

namespace ComputePower.Tests.Http.Tests
{
    public class DownloadManagerTest
    {
        [Fact]
        public async Task DownLoadAndSaveFile_succeeds()
        {
            var url = "http://tvermose.it/ksp/Gigantor(110t_LKO).craft";
            string path = "";
            var fileName = "Gigantor.craft";

            var dlManager = new DownloadManager();
            var result = await dlManager.DownloadAndSaveFile(url, path, fileName);

            Assert.True(result);
        }

        [Fact]
        public async Task DownLoadAndSaveFile_fails()
        {
            var url = "http://tvermose.it/ksp/Gigantor(110t_LKO).craft";
            string path = "";
            var fileName = "Gigantor.craft";
            var dlManager = new DownloadManager();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await dlManager.DownloadAndSaveFile("", path, fileName));
            Assert.IsType<InvalidOperationException>(ex);

            var ex2 = await Assert.ThrowsAsync<ArgumentException>(async () => await dlManager.DownloadAndSaveFile(url, "", ""));
            Assert.IsType<ArgumentException>(ex2);

            var ex3 = await Assert.ThrowsAsync<HttpRequestException>(async () => await dlManager.DownloadAndSaveFile("http://incorrectUrl.tvermose.it", path, fileName));
            Assert.IsType<HttpRequestException>(ex3);
        }
    }
}
