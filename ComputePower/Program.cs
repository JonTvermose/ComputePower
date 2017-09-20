using System;

namespace ComputePower.UserInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting application...!");
            IComputePowerController controller = new ComputePowerController();

            Console.WriteLine("Application ended. Press any key to close.");
            Console.ReadLine(); // Await user input before exiting the program
        }
    }
}
