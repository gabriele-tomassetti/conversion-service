using System;
using System.Collections.Generic;
using System.Linq;

namespace ParsingServices.Models
{      
    public class DataArray : DataItem
    {
        public IList<DataItem> Values { get; set; } = new List<DataItem>();

        public void AddValue(string text)
        {
            Values.Add(new DataValue() { Text = text });
        }

        public void AddValue(DataItem item)
        {
            Values.Add(item);
        }
    }
}