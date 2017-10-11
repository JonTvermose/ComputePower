using System;
using System.Runtime.InteropServices;
using ComputePower.Computation.Models;

namespace ComputePower
{
    /// <summary>
    /// Original source: https://blogs.msdn.microsoft.com/jonathanswift/2006/10/03/dynamically-calling-an-unmanaged-dll-from-net-c/
    /// Some modifications has been made to clean the code and make it usefull.
    /// </summary>
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
        
        protected delegate bool ParralelDelegate(EventHandler<ComputationProgressEventArgs> progressHandler, params object[] inputObjects);

        private IntPtr _pDll;

        public DllLoader()
        {
            _pDll = IntPtr.Zero;
        }

        protected ParralelDelegate LoadDll(string pathToDll, string methodName)
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
    }
}
