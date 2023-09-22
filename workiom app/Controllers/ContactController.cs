using Microsoft.AspNetCore.Mvc;
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
    public class ContactController : Controller
    {
        private readonly ContactService _myDataService;

        public ContactController(ContactService myDataService)
        {
            _myDataService = myDataService;
        }

        [HttpGet]
        public ActionResult<List<Contact>> Get()
        {
            return _myDataService.Get();
        }

        [HttpPost]
        public IActionResult Post([FromBody] Contact contact)
        {
            _myDataService.InsertOne(contact);
            return Json(contact);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var contact = _myDataService.Get(id);

            if (contact == null)
            {
                return NotFound();
            }

            _myDataService.Delete(id);

            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] Contact contactIn)
        {
            var contact = _myDataService.Get(id);

            if (contact == null)
            {
                return NotFound();
            }

            _myDataService.Update(id, contactIn);

            return Json(contact);
        }

        [HttpGet("search")]
        public ActionResult<List<Contact>> Search(string term)
        {

            if (term == null || term.Equals(""))
            {
                return NotFound();
            }
            return _myDataService.Search(term);
        }


        [HttpGet("contactsForCompany/{companyId}")]
        public ActionResult<List<Contact>> GetContactsForCompany(string companyId)
        {
            return _myDataService.GetContactsForCompany(companyId);
        }


    }
}
