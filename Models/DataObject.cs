using System;
using System.Collections.Generic;
using System.Linq;

namespace ParsingServices.Models
{
    public class DataObject : DataItem
    {     
        public IList<DataField> Fields { get; set; } = new List<DataField>();        
     
        public void AddField(DataField field)
        {
            Fields.Add(field);
        }

        public void AddField(string name, DataItem item)
        {
            DataField field = new DataField();
            field.Text = name;
            field.Value = item;

            Fields.Add(field);
        }

        public bool ContainsName(string name)
        {
            return Fields.Any(x => x.Text == name);
        }

        public DataField GetByName(string name)
        {
            return Fields.FirstOrDefault(x => x.Text == name);
        }
    }
}