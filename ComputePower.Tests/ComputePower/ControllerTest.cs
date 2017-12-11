using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ComputePower.Http;
using Xunit;

namespace ComputePower.Tests.ComputePower
{
    public class ControllerTest
    {
        [Fact]
        public async Task DownProjectsList_fails()
        {
            var controller = new ComputePowerController();
            var result = await controller.DownloadProjects(null);
            Assert.Null(result);
        }
    }
}
