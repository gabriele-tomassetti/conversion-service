using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using ParsingServices.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ParsingServices.Libs
{
    public class DataRepository : IDataRepository
    {
        private String Location = "Data";

        public InfoData Get(Guid id)
        {
            if(Directory.Exists($"{Location}{Path.DirectorySeparatorChar}{id}"))
            {
                return InfoData.LoadFromDirectory($"{Location}{Path.DirectorySeparatorChar}{id}{Path.DirectorySeparatorChar}data.json");
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<InfoData> All()
        {
            IEnumerable<string> directories = Directory.EnumerateDirectories(Location + Path.DirectorySeparatorChar);
            IList<InfoData> files = new List<InfoData>();
            
            foreach(var d in directories)
            {
                files.Add(InfoData.LoadFromDirectory($"{d}{Path.DirectorySeparatorChar}data.json"));            
            }   

            return files;         
        }

        public InfoData Save(InputData value)
        {
            var id = Guid.NewGuid();
            Directory.CreateDirectory($"{Location}{Path.DirectorySeparatorChar}{id}{Path.DirectorySeparatorChar}");            
            IConverter<DataItem> converter = null;                
            DataItem data = new DataItem();            

            switch(value.Format)
            {
                case Format.JSON:
                    converter = new ParsingServices.Models.JsonConverter();                                        
                break;
                case Format.CSV:
                    converter = new ParsingServices.Models.CSVConverter();                                        
                break;
                default:
                    break;
            }

            using(FileStream fs = new FileStream($"{Location}{Path.DirectorySeparatorChar}{id}{Path.DirectorySeparatorChar}file.{value.Format}", FileMode.OpenOrCreate)) {                
                value.File.CopyTo(fs);                
            }
            
            using(FileStream fs = new FileStream($"{Location}{Path.DirectorySeparatorChar}{id}{Path.DirectorySeparatorChar}file.{value.Format}", FileMode.Open)) {
                data = converter.FromFile(fs);
            }
            var infoData = new InfoData() {
                Id = id,
                Format = value.Format,
                LocationFile = $"{Location}{Path.DirectorySeparatorChar}{id}{Path.DirectorySeparatorChar}file.{value.Format}",
                Data = data
            };                                                            
                        
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;            

            System.IO.File.WriteAllText($"{Location}{Path.DirectorySeparatorChar}{id}{Path.DirectorySeparatorChar}data.json", JsonConvert.SerializeObject(infoData, settings));

            return infoData;
        }

        public void Delete(Guid id)
        {
            if(Directory.Exists($"{Location}{Path.DirectorySeparatorChar}{id}"))
                Directory.Delete($"{Location}{Path.DirectorySeparatorChar}{id}", true);
        }
    }
}
