using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GitFolderStructureReplicator
{
    public class Serializer
    {
        public string Filename { get; set; } = "dirstruct.json";
        public string FilenameLogs { get; set; } = "logs.txt";

        public bool WriteToFile(DirectoryNode root, List<string> logs)
        {
            bool serializingSuccess = true;
            try
            {
                File.WriteAllText(Filename, JsonConvert.SerializeObject(root, Formatting.Indented, GetJsonSerializerSettings()));
            }
            catch (System.Exception ex)
            {
                logs.Add(ex.Message);
                serializingSuccess = false;
            }
            WriteLogs(logs);
            return serializingSuccess;
        }

        private JsonSerializerSettings GetJsonSerializerSettings()
        {
            return new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        }

        public void WriteLogs(List<string> logs)
        {
            if (logs.Count > 0)
            {
                File.WriteAllLines(FilenameLogs, logs);
            }
        }
    }
}
