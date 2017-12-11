using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ComputePower.Http.Models;

namespace ComputePower.Helpers
{
    public delegate Object ParralelDelegate(int a);

    internal class DllLoader
    {
        #region Externally implemented methods from kernel32.dll
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);
        #endregion
        
        private IntPtr _pDll;

        public DllLoader()
        {
            _pDll = IntPtr.Zero;
        }

        // Load a dll, get a function pointer to a named method and return it as an delegate
        public ParralelDelegate LoadDll(string pathToDll, string methodName)
        {
            if (_pDll != IntPtr.Zero)
            {
                FreeDll();
            }

            _pDll = LoadLibrary(pathToDll);
            if (_pDll == IntPtr.Zero)
                return null;

            IntPtr pAddressOfFunctionToCall = GetProcAddress(_pDll, methodName);
            if (pAddressOfFunctionToCall == IntPtr.Zero)
                return null;

            ParralelDelegate del = (ParralelDelegate)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(ParralelDelegate));
            return del;
        }

        protected void FreeDll()
        {
            if (_pDll != IntPtr.Zero)
            {
                FreeLibrary(_pDll);
            }
        }

        /// <summary>
        /// Using reflection, load an assembly, call an method with parameters and return the values from the method.
        /// </summary>
        /// <param name="assemblyPath">Path to the assembly</param>
        /// <param name="assemblyName">Name of the assembly (without .dll)</param>
        /// <param name="methodName">Name of the method to call</param>
        /// <param name="progressHandler">EventHandler to recieve progress updates</param>
        /// <returns></returns>
        public object CallMethod(string assemblyPath, string assemblyName, string methodName, EventHandler<EventArgs> progressHandler)
        {
            var filepath = assemblyPath + "\\" + assemblyName + ".dll";

            // Load the assembly by path (and name)
            Assembly computationAssembly = Assembly.LoadFile(filepath);
            
            // Get the type of the computation class where the method is located
            Type classType = computationAssembly.GetType(assemblyName + ".Computation.Computation");
            if(classType == null)
                throw new Exception();

            //// Define parameters type in the method to call
            //Type[] parametersType = new Type[]{ typeof(object[]) };

            // Retrieve the method
            var method = classType.GetMethod(methodName);

            if (method == null)
                throw new Exception();

            // Create an object of type "classType"
            var objectType = Activator.CreateInstance(classType);
            try
            {
                // Invoke (call) the method and await the result (method is async)
                var result = method.Invoke(objectType, new object[] { progressHandler });
                return result;
            }
            catch (Exception e)
            {
                ProgressEventArgs args = new ProgressEventArgs(0, "Exception occured loading DLL");
                args.Exception = e;
                progressHandler?.Invoke(this, args);
                return null;
            }

        }
    }
}
