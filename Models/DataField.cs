using System;
using System.Collections.Generic;
using System.Linq;
namespace ParsingServices.Models
{    
    public class DataField : DataItem
    {
        public string Text { get; set; } = ""; 
        public DataItem Value { get; set; } = null;
    }    
}