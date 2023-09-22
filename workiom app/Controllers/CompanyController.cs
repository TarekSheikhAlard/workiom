using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workiom_app.Models;
using workiom_app.Services;

namespace workiom_app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : Controller
    {
        private readonly CompanyService _myDataService;

        public CompanyController(CompanyService myDataService)
        {
            _myDataService = myDataService;
        }

        [HttpGet]
        public ActionResult<List<Company>> Get()
        {
            return _myDataService.Get();
        }


        [HttpPost]
        public IActionResult Post([FromBody] Company company)
        {
            _myDataService.InsertOne(company);
            return Json(company);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var company = _myDataService.Get(id);

            if (company == null)
            {
                return NotFound();
            }

            _myDataService.Delete(id);

            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] Company companyIn)
        {
            var company = _myDataService.Get(id);

            if (company == null)
            {
                return NotFound();
            }

            _myDataService.Update(id, companyIn);

            return Json(company);
        }

        [HttpGet("search")]
        public ActionResult<List<Company>> Search(string term)
        {
            if (term == null || term.Equals(""))
            {
                return NotFound();
            }
            return _myDataService.Search(term);
        }


        [HttpGet("contacts")]
        public ActionResult<List<BsonDocument>> GetContacts()
        {
            return  _myDataService.GetCompaniesWithContacts();
        }
    }
}

