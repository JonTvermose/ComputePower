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

        public string Url { get; set; }

        public string FileName { get; set; }

        public async Task Begin()
        {

            Console.WriteLine("Starting application...!");
            Console.WriteLine("Enter URL: ");
            if ((Url = Console.ReadLine()).Length <= 1)
            {
                Url = "http://tvermose.it/ksp/Gigantor(110t_LKO).craft";
            }
            Console.WriteLine("Enter filename: ");
            if ((FileName = Console.ReadLine()).Length <= 1)
            {
                FileName = "Gigantor.craft";
            }
            
            ProgressHandler += ProgressDownloadPrinter;
            ProgressHandler += ProgressCompletePrinter;

            var controller = new ComputePowerController();

            string path = "";
            await controller.Test(Url, path, FileName, ProgressHandler);
            
            Console.WriteLine("Application ended. Press any key to close.");
        }

        private void ProgressDownloadPrinter(Object sender, ProgressEventArgs args)
        {
            if (args.Exception == null)
            {
                Console.WriteLine("Total downloadet: {0:N2} kB", args.BytesRead);
            }
        }

        private void ProgressCompletePrinter(Object sender, ProgressEventArgs args)
        {
            if (args.Exception != null)
            {
                Console.WriteLine(args.Message);
                Console.WriteLine(args.Exception.StackTrace);
                return;
            }
            if (args.IsComplete)
            {
                Console.WriteLine("{0} {1} kB downloadet", args.Message, args.BytesRead);
            }
            if (Math.Abs(args.BytesRead) < 0.001)
            {
                Console.WriteLine(args.Message);
            }
        }
    }
}
