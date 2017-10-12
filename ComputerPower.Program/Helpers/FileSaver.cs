using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ComputePower.Helpers
{
    internal class FileSaver
    {
        public void SerializeAndSaveFile(object inputObjects, string filePath, string fileName)
        {
            // Append '\' to path if last char is not a '\'
            if (!string.IsNullOrWhiteSpace(filePath) && filePath.Last() != '\\')
            {
                filePath += "\\";
            }

            var name = fileName.Split('.').Last();

            // Serialize data to JSON and save it to results.json with the given filePath
            using (StreamWriter file = File.CreateText(filePath + name + ".json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, inputObjects);
            }
        }
    }
}
