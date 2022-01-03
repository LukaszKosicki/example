using api.Models.Entities.Other;
using api.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ContactFormController : Controller
    {
        private IAsyncRepository<ContactForm> contactFormRepository;


        public ContactFormController(IAsyncRepository<ContactForm> contactFormRepo)
        {
            contactFormRepository = contactFormRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ContactForm model)
        {
            await contactFormRepository.Add(model);
            return Ok();
        }
    }
}
