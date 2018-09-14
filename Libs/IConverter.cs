using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Antlr4.Runtime.Tree;
using System.IO;
using ParsingServices.Models;

namespace ParsingServices.Libs
{
    public interface IConverter<T>
    {
        string ToFile(DataItem data);
        DataItem FromFile(Stream dataFile);
        bool IsValid(DataItem data);
    }
}