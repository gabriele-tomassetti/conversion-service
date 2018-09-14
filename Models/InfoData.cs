using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParsingServices.Libs;

namespace ParsingServices.Models
{   
    public enum ValueFormat
    {        
        Bool,
        Integer,
        Numeric,
        String        
    }

    public class InfoData
    {
        public Guid Id { get; set; } = Guid.Empty;
        public Format Format { get; set; } = Format.Undetermined;
        public string LocationFile { get; set; } = "";
        public DataItem Data { get; set; } = new DataItem();
        
        static public InfoData LoadFromDirectory(string path)
        {
            InfoData data = new InfoData();
                        
            var info = JObject.Parse(File.ReadAllText(path));
            data.Id = Guid.Parse(info["Id"].ToString());            
            data.LocationFile = info["LocationFile"].ToString();
            data.Format = Enum.Parse<Format>(info["Format"].ToString());            

            IConverter<DataItem> converter = null;    
            switch(data.Format)
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
            
            using(FileStream fs = new FileStream(path.Replace("data.json", $"file.{data.Format}"), FileMode.Open)) {
                data.Data = converter.FromFile(fs);
            }            
            
            return data;
        }
    }    
}