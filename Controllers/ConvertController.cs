using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ParsingServices.Models;
using ParsingServices.Libs;

namespace ParsingServices.Controllers
{
    [Route("api/[controller]")]
    public class ConvertController : Controller
    {        
        private IDataRepository _dataRepository;
        public ConvertController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<InfoData> files =_dataRepository.All();

            return View(files);
        }

        [HttpGet("{id}/{format?}", Name = "GetData")]
        public IActionResult Get(Guid id, string format = "")
        {                        
            InfoData data = _dataRepository.Get(id);
            Pipeline pipeline = new Pipeline();            

            if(!String.IsNullOrEmpty(format))
                data.Format = Enum.Parse<Format>(format);
            
            string converted = pipeline.Convert(data.Data, data.Format);

            if(String.IsNullOrEmpty(converted))
                return NoContent();
            else
                return Content(converted);
        }
        
        [HttpPost]
        public IActionResult Post(InputData value)
        {            
            InfoData result = _dataRepository.Save(value);
                        
            return new JsonResult(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _dataRepository.Delete(id);

            return Ok();
        }
    }
}
