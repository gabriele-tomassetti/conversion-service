using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using ParsingServices.Parsers;
using ParsingServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParsingServices.Libs
{
    public class CSVConverterVisitor
    {                        
        public DataArray VisitCsv(CSVParser.CsvContext context)
        {            
            DataArray arr = new DataArray();

            List<string> names = new List<string>();

            foreach(var n in context.hdr().row().field())
            {
                names.Add(n.GetText());
            }

            foreach(var r in context.row())
            {
               arr.AddValue(VisitRow(names, r));
            }

            return arr;
        }

        public DataItem VisitRow(List<string> names, CSVParser.RowContext context)
        {
            DataObject item = new DataObject();
            
            if(names.Count != context.field().Length)
                throw new Exception("Wrong number of elements");

            for(int a = 0; a < names.Count; a++)
            {
                item.AddField(names[a], new DataValue(){ Text = CleanField(context.field()[a].GetText())});
            }

            return item;
        } 

        private String CleanField(string text)          
        {            
            // we trim any enclosing quotes
            text = text.Trim('\"');
            
            // we remove escaping from double quotes
            if(text.Contains("\"\""))
                return text.Replace("\"\"","\"");
            else
                return text;            
        }
    }
}