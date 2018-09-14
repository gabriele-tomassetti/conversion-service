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
using System.Text.RegularExpressions;

namespace ParsingServices.Models
{
    public class CSVConverter : IConverter<DataItem>
    {
        private string PrepareForCSV(string text)
        {                        
            // escape any double quotes        
            if(text.Contains("\""))                
                text = text.Replace("\"","\"\"");
            
            // we add enclosing double quotes if necessary
            if(Regex.IsMatch(text, @"(,|""|^[ \t]|[ \t]$)"))
                text = $"\"{text}\"";        
            
            return text;
        }
        public string ToFile(DataItem data)
        {                        
            if(IsValidObjects(data))
                return FromObjectsToFile(data);
        
            if(IsValidValues(data))
                return FromValuesToFile(data);
                        
            return String.Empty;
        }  
        
        public String FromObjectsToFile(DataItem data)
        {
            List<List<string>> rows = new List<List<string>>();            
            
            StringBuilder text = new StringBuilder();
            
            List<string> headRow = new List<string>();                  
            
            if(data is DataArray)
            {
                foreach(var dataRow in (data as DataArray).Values)
                {
                    if(dataRow is DataObject)
                    {
                        foreach(var item in (dataRow as DataObject).Fields)
                        {
                            if(!headRow.Contains(item.Text))
                                headRow.Add(PrepareForCSV(item.Text));
                        }                                    
                    }                   
                }

                rows.Add(headRow);

                foreach(var dataRow in (data as DataArray).Values)
                {
                    List<string> row = new List<string>();

                    foreach(var head in headRow)
                    {
                        if((dataRow as DataObject).ContainsName(head))
                        {
                            var item = (dataRow as DataObject).GetByName(head);
                            if(item.Value is DataValue)
                                row.Add(PrepareForCSV((item.Value as DataValue).Text));
                        }
                        else
                            row.Add(String.Empty);
                    }

                    rows.Add(row);
                }

                foreach(var row in rows)
                {
                    text.AppendJoin(",", row.ToArray());
                    text.AppendLine();
                }
            }

            return text.ToString();
        }

        public String FromValuesToFile(DataItem data)
        {
            List<List<string>> rows = new List<List<string>>();            
            
            StringBuilder text = new StringBuilder();
            
            List<string> headRow = new List<string>();                  
            
            if(data is DataArray)
            {
                // we add the arbitrary value "item" as header
                headRow.Add(PrepareForCSV("item"));
                
                rows.Add(headRow);

                foreach(var dataRow in (data as DataArray).Values)
                {
                    List<string> row = new List<string>();

                    if(dataRow is DataValue)
                        row.Add(PrepareForCSV((dataRow as DataValue).Text));
                    else
                        row.Add(String.Empty);                    

                    rows.Add(row);
                }

                foreach(var row in rows)
                {
                    text.AppendJoin(",", row.ToArray());
                    text.AppendLine();
                }
            }

            return text.ToString();
        }

        public DataItem FromFile(Stream dataFile)
        {
            ICharStream chars = CharStreams.fromStream(dataFile);
            CSVLexer csvLexer = new CSVLexer(chars);            
            CommonTokenStream commonTokenStream = new CommonTokenStream(csvLexer);
            CSVParser csvParser = new CSVParser(commonTokenStream);
            CSVConverterVisitor visitor = new CSVConverterVisitor();

            try {
                return visitor.VisitCsv(csvParser.csv());
            }
            catch {
                return new DataItem();
            }
        }

        public bool IsValid(DataItem data)
        {
            bool valid = false; 
            bool validValues = true;
            bool validObjects = true;

            if(data is DataArray)
            {
                valid = true;                
                
                foreach(var dataRow in (data as DataArray).Values)
                {                    
                    if(!(dataRow is DataObject))
                    {  
                        validObjects = false;                                                       
                    }

                    if(!(dataRow is DataValue))
                    {  
                        validValues = false;                                                       
                    }
                }               
            }
            
            return valid && (validValues != validObjects);
        }

        public bool IsValidValues(DataItem data)
        {
            bool validValues = true;

            if(data is DataArray)
            {
                validValues = true;                
                
                foreach(var dataRow in (data as DataArray).Values)
                {                    
                    if(!(dataRow is DataValue))
                    {  
                        validValues = false;                                                       
                    }
                }               
            }
            
            return validValues;
        }

        public bool IsValidObjects(DataItem data)
        {
            bool validObjects = false;

            if(data is DataArray)
            {
                validObjects = true;                
                
                foreach(var dataRow in (data as DataArray).Values)
                {                    
                    if(!(dataRow is DataObject))
                    {  
                        validObjects = false;                                                       
                    }
                }               
            }
            
            return validObjects;
        }
    } 
}