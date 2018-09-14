using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ParsingServices.Models;
using ParsingServices.Libs;

namespace ParsingServices.Controllers
{
    public class HomeController : Controller
    {        
        private IDataRepository _dataRepository;
        public HomeController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }
        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<InfoData> files =_dataRepository.All();
            return View(files);
        }

        [HttpGet("{id}/{format?}")]
        public IActionResult Get(Guid id, string format = "")
        {                        
            InfoData data = _dataRepository.Get(id);
            Pipeline pipeline = new Pipeline();
            
            ViewData["Id"] = id;

            if(!String.IsNullOrEmpty(format))
                data.Format = Enum.Parse<Format>(format);
            
            string converted = pipeline.Convert(data.Data, data.Format);

            if(String.IsNullOrEmpty(converted))
            {
                ViewData["File"] = "";
                ViewData["Status"] = false;
            }
            else
            {
                ViewData["File"] = converted;
                ViewData["Status"] = true;
            } 
            
            return View();
        }
        
        [HttpGet("add")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpGet("addOperation")]
        public IActionResult AddOperation()
        {
            return View();
        }

        [HttpPost("addOperation")]
        public IActionResult UploadOperation(IFormFile file)
        {
            var operation = new OperationController(_dataRepository);
            operation.Post(file);

            return Redirect("/");
        }

        [HttpPost("add")]
        public IActionResult Post(InputData value)
        {            
            var convert = new ConvertController(_dataRepository);
            convert.Post(value);

            return Redirect("/");
        }

        [HttpGet("/delete/{id}")]
        public IActionResult Delete(Guid id)
        {
            ViewData["Id"] = id;

            _dataRepository.Delete(id);
            
            return View();
        }
    }
}
