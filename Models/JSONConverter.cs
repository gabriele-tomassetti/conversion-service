using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Antlr4.Runtime.Tree;
using ParsingServices.Libs;
using ParsingServices.Parsers;
using Antlr4.Runtime;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

namespace ParsingServices.Models
{
    public class JsonConverter : IConverter<DataItem>
    {
        private JArray ConvertToJsonArray(DataArray data)
        {
            JArray arr = new JArray();                       

            foreach(var value in data.Values)
            {
                if(value is DataArray == false && value is DataObject == false)
                {                    
                    arr.Add(GetToken(value as DataValue));
                }
                else if(value is DataArray)
                {
                    arr.Add(ConvertToJsonArray(value as DataArray));                    
                }
                else if(value is DataObject)
                {
                    arr.Add(ConvertToJsonObject(value as DataObject));                    
                }
            }
            
            return arr;
        }

        private JObject ConvertToJsonObject(DataObject data)
        {
            JObject obj = new JObject();
            JProperty prop = null;

            foreach(var field in data.Fields)
            {
                if(field.Value is DataArray == false && field.Value is DataObject == false)
                {
                    prop = GetProperty(field.Text, field.Value as DataValue);
                }
                else if(field.Value is DataArray)
                {
                    prop = new JProperty(field.Text, ConvertToJsonArray(field.Value as DataArray));
                }
                else if(field.Value is DataObject)
                {
                    prop = new JProperty(field.Text, ConvertToJsonObject(field.Value as DataObject));
                }

                obj.Add(prop);
            }
            
            return obj;
        }

        private JToken GetToken(DataValue value)
        {
            JToken token;
            
            switch(value.Format)
            {
                case ValueFormat.Integer:
                    token = Convert.ToInt32(value.Text);
                    break;
                case ValueFormat.Numeric:
                    token = Convert.ToDecimal(value.Text);
                    break;
                case ValueFormat.Bool:
                    token = Convert.ToBoolean(value.Text);
                    break;
                default:
                    token = value.Text;
                    break;
            }

            return token;
        }

        private JProperty GetProperty(String name, DataValue value)
        {
            return new JProperty(name, GetToken(value));
        }

        public string ToFile(DataItem data)
        {            
            JContainer container = null;
            if(data is DataArray)
            {
                container = ConvertToJsonArray(data as DataArray);
            }
            else if(data is DataObject)            
            {
                container = ConvertToJsonObject(data as DataObject);
            }
                        
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;                     

            return JsonConvert.SerializeObject(container, settings);            
        }  

        public DataItem FromFile(Stream dataFile)
        {
            ICharStream chars = CharStreams.fromStream(dataFile);
            JSONLexer jsonLexer = new JSONLexer(chars);            
            CommonTokenStream commonTokenStream = new CommonTokenStream(jsonLexer);
            JSONParser jsonParser = new JSONParser(commonTokenStream);
            JSONConverterVisitor visitor = new JSONConverterVisitor();

            return visitor.VisitJson(jsonParser.json());
        }

        public bool IsValid(DataItem data)
        {
            return true;
        }   
    }
}