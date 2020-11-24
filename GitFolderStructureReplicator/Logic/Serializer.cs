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
        public string Filename { get; private set; } = "dirstruct.json";
        public string FolderPath { get; private set; } = "generated";
        public string FilenameLogs { get; set; } = "logs.txt";

        public Serializer(string folderPath)
        {
            if (!string.IsNullOrWhiteSpace(folderPath))
            {
                FolderPath = folderPath;
            }
            Directory.CreateDirectory(FolderPath);
        }

        public bool WriteToFile(DirectoryNode root, List<Log> logs)
        {
            bool serializingSuccess = true;
            try
            {
                File.WriteAllText(GetFilePath(Filename), JsonConvert.SerializeObject(root, GetJsonSerializerSettings()));
            }
            catch (Exception ex)
            {
                logs.Add(new Log { DateTime = DateTime.Now, Message = ex.Message });
                serializingSuccess = false;
            }
            WriteLogs(logs, true);
            return serializingSuccess;
        }

        private string GetFilePath(string filename)
        {
            return string.IsNullOrWhiteSpace(FolderPath) ? filename : FolderPath + "\\" + filename;
        }

        public DirectoryNode ReadFromFile(List<Log> logs)
        {
            DirectoryNode root = null;
            try
            {
                string text = File.ReadAllText(GetFilePath(Filename));
                root = JsonConvert.DeserializeObject<DirectoryNode>(text, GetJsonSerializerSettings());
            }
            catch (System.Exception ex)
            {
                logs.Add(new Log { DateTime = DateTime.Now, Message = ex.Message });
            }
            return root;
        }


        private JsonSerializerSettings GetJsonSerializerSettings()
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            return new JsonSerializerSettings { ContractResolver = contractResolver, Formatting = Formatting.Indented };
        }

        public void WriteLogs(List<Log> logs, bool extraction)
        {
            if (logs.Count > 0)
            {
                File.AppendAllText(GetFilePath(FilenameLogs), Log.GetText(logs));
            }
        }
    }
}
