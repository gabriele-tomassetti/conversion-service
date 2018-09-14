using System;
using System.Collections.Generic;
using System.Linq;

namespace ParsingServices.Models
{
    public class DataValue : DataItem
    {        
        private string _text = "";
        public string Text
        {
            get {
                return _text;
            }
            set {
                Format = DataValue.DetermineFormat(value);
                
                _text = DataValue.PrepareValue(Format, value);                
            }
        }
        public ValueFormat Format { get; private set; } = ValueFormat.String;     

        private static ValueFormat DetermineFormat(string text)
        {
            ValueFormat format = ValueFormat.String;

            text = text.Trim().Trim('"');

            int intNum;
            
            bool isInt = int.TryParse(text, out intNum);            

            if(isInt)
                return ValueFormat.Integer;
            
            double realNum;
            
            bool isNum = double.TryParse(text, out realNum);

            if(isNum)
                return ValueFormat.Numeric;

            bool boolean;
            
            bool isBool = bool.TryParse(text, out boolean);

            if(isBool)
                return ValueFormat.Bool;
            
            return format;
        }

        private static String PrepareValue(ValueFormat format, string text)
        {
            text = text.Trim();

            return text;
        }
    }
}