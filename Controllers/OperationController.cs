using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ParsingServices.Libs;
using System.IO;

namespace ParsingServices.Controllers
{
    [Route("api/[controller]")]
    public class OperationController : Controller
    {
        private IDataRepository _dataRepository;
        private Pipeline _pipeline = new Pipeline();        
        public OperationController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }        
        
        [HttpPost]
        public IActionResult Post(IFormFile file)
        {                                   
            _pipeline.AddOperation(file);
                        
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _pipeline.DeleteOperation(id);

            return Ok();
        }
    }
}
