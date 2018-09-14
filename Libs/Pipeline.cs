using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using ParsingServices.Models;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace ParsingServices.Libs
{
    public class Pipeline
    {                
        private String Location = "Operations";
        
        public String Convert(DataItem data, Format format)
        {            
            IConverter<DataItem> converter = null;                
            String converted = String.Empty;                      

            if(data != null)
            {
                data = PerformOperations(data);
                
                switch(format)
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

                if(converter.IsValid(data))
                {
                    converted = converter.ToFile(data);
                }
            }            
            
            return converted;
        }

        public DataItem PerformOperations(DataItem data)
        {
            foreach(var file in Directory.EnumerateFiles($"{Location}{Path.DirectorySeparatorChar}"))
            {
                GlobalsScript item = new GlobalsScript { Data = data };
                
                var script = CSharpScript.EvaluateAsync<DataItem>(
                    System.IO.File.ReadAllText(file),
                    Microsoft.CodeAnalysis.Scripting.ScriptOptions.Default
                    .WithReferences(typeof(ParsingServices.Models.DataItem).Assembly)                    
                    .WithImports("System.Collections.Generic")
                    .WithImports("System.Linq"),
                    globalsType: item.GetType(), globals: item); 
                                                
                script.Wait();                    
                data = script.Result;   
            }            

            return data;
        }

        public void AddOperation(IFormFile file)
        {
            var id = Guid.NewGuid();
            using(FileStream fs = new FileStream($"{Location}{Path.DirectorySeparatorChar}{id}.scs", FileMode.OpenOrCreate)) {                
                file.CopyTo(fs);                
            }                                  
        }  
        public void DeleteOperation(Guid id)
        {
            Directory.Delete($"{Location}{Path.DirectorySeparatorChar}{id}", true);
        }
    }
}