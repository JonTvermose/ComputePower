using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ComputePower.Helpers
{
    public class FileLoader<T>
    {

        /// <summary>
        /// Load a file from the file system and deserialize to object of type T (provided)
        /// </summary>
        public void LoadFromFileSystem(string path, out T output)
        {
            string jsonText = File.ReadAllText(path);       
            output = JsonConvert.DeserializeObject<T>(jsonText);
        }
    }
}
