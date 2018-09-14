using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using ParsingServices.Parsers;
using ParsingServices.Models;
using Newtonsoft.Json.Linq;

namespace ParsingServices.Libs
{      
    public class JSONConverterVisitor
    {        
        public DataItem VisitJson(JSONParser.JsonContext context)
        {            
            return VisitComplex_value(context.complex_value());
        }

        public DataItem VisitComplex_value(JSONParser.Complex_valueContext context)
        {
            if(context.obj() != null)
                return VisitObj(context.obj());
            else if(context.array() != null)
                return VisitArray(context.array());
            else
                return new DataItem();     
        }
	            
        public DataObject VisitObj(JSONParser.ObjContext context)
        {
            if(context.pair() != null && context.pair().Length > 0)
            {                
                DataObject obj = new DataObject();

                foreach(var p in context.pair())
                {
                    if(p.value() != null)
                        obj.AddField(VisitPair(p));
                    else
                        obj.AddField(p.STRING().GetText().Trim('"'), VisitComplex_value(p.complex_value()));
                }

                return obj;            
            }
            else
            {
                return new DataObject();
            }            
        }
	            
        public DataField VisitPair(JSONParser.PairContext context)
        {            
            return new DataField(){ Text = context.STRING().GetText().Trim('"'),
                                    Value = new DataValue() { Text = JToken.Parse(context.value().GetText()).ToString() } };
        }
	            
        public DataArray VisitArray(JSONParser.ArrayContext context)
        {
            if(context.composite_value() != null && context.composite_value().Length > 0)
            {                                
                DataArray array = new DataArray();

                foreach(var p in context.composite_value())
                {
                    if(p.value() != null)
                    {                        
                        array.AddValue(JToken.Parse(p.value().GetText()).ToString());
                    }
                    else
                    {                        
                        array.AddValue(VisitComplex_value(p.complex_value()));
                    }
                }

                return array;            
            }
            else
            {                
                return new DataArray();
            }
        }    
    }
}