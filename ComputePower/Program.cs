using System;
using System.Threading.Tasks;
using ComputePower.Http.Models;

namespace ComputePower.UserInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Begin();
            Console.ReadLine(); // Await user input before exiting the program
        }

        public event EventHandler<ProgressEventArgs> ProgressHandler;


        public async Task Begin()
        {
            Console.WriteLine("Starting application...!");

            ProgressHandler += ProgressDownloadPrinter;
            ProgressHandler += ProgressCompletePrinter;

            var controller = new ComputePowerController();
            var url = "http://tvermose.it/ksp/Gigantor(110t_LKO).craft";
            string path = "";
            var fileName = "Gigantor.craft";
            await controller.Test(url, path, fileName, ProgressHandler);

            Console.WriteLine("Application ended. Press any key to close.");
        }

        private void ProgressDownloadPrinter(Object sender, ProgressEventArgs args)
        {
            Console.WriteLine("Total downloadet: {0:N2} kB", args.BytesRead);
        }

        private void ProgressCompletePrinter(Object sender, ProgressEventArgs args)
        {
            if (args.IsComplete)
            {
                Console.WriteLine("{0} {1} kB downloadet", args.Message, args.BytesRead);
            }
        }
        
    }
}
