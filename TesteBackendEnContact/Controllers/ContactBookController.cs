using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Controllers.Models;
using TesteBackendEnContact.Core.Domain.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactBookController : ControllerBase
    {
        private readonly ILogger<ContactBookController> _logger;

        public ContactBookController(ILogger<ContactBookController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<IContactBook>> Post(SaveContactBookRequest contactBook, [FromServices] IContactBookRepository contactBookRepository)
        {
            return Ok(await contactBookRepository.SaveAsync(contactBook.ToContactBook()));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IContactBook>> Put(int id, SaveContactBookRequest contactBook, [FromServices] IContactBookRepository contactBookRepository)
        {
            var contactBookToEdit = await contactBookRepository.GetAsync(id);
            if(contactBookToEdit != null)
                return Ok(await contactBookRepository.UpdateAsync(contactBook.ToContactBook(id)));

            return NotFound("Agenda não encontrada");
        }

        [HttpDelete]
        public async Task Delete(int id, [FromServices] IContactBookRepository contactBookRepository)
        {
            await contactBookRepository.DeleteAsync(id);
        }

        [HttpGet]
        public async Task<IEnumerable<IContactBook>> Get([FromServices] IContactBookRepository contactBookRepository)
        {
            return await contactBookRepository.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<IContactBook> Get(int id, [FromServices] IContactBookRepository contactBookRepository)
        {
            return await contactBookRepository.GetAsync(id);
        }
    }
}
