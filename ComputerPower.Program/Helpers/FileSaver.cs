using System.IO;
using Newtonsoft.Json;

namespace ComputePower.Helpers
{
    internal class FileSaver
    {
        public void SerializeAndSaveFile(object inputObjects, string filePath)
        {
            // Append '\' to path if last char is not a '\'
            if (!string.IsNullOrWhiteSpace(filePath) && filePath[filePath.Length] != '\\')
            {
                filePath += "\\";
            }

            // Serialize data to JSON and save it to results.json with the given filePath
            using (StreamWriter file = File.CreateText(filePath + "results.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, inputObjects);
            }
        }
    }
}
