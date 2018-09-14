using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ParsingServices.Models
{
    public enum Format
    {
        CSV,
        JSON,
        Undetermined
    }
    
    public class InputData
    {
        public Format Format { get; set; }        
        public IFormFile File { get; set; }
    }    
}