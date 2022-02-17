using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Controllers.Models;
using TesteBackendEnContact.Core.Domain.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly ILogger<ContactController> _logger;

        public ContactController(ILogger<ContactController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<IContact>> Get([FromServices] IContactRepository contactRepository)
        {
            return await contactRepository.GetAllAsync();
        }
        [HttpPost]
        public async Task<ActionResult<IContact>> Post(SaveContactRequest contact, [FromServices] IContactRepository contactRepository, [FromServices] ICompanyRepository companyRepository, [FromServices] IContactBookRepository contactBookRepository)
        {
            if(contact.CompanyId != null)
            {
                var company = await companyRepository.GetAsync(contact.CompanyId.Value);

                if (company == null)
                    return NotFound("Empresa não econtrada");
            }

            var contactBook = await contactBookRepository.GetAsync(contact.ContactBookId);

            if (contactBook == null)
                return NotFound("Agenda não econtrada");

            return Ok(await contactRepository.SaveAsync(contact.ToContact()));
        }

        [HttpDelete]
        public async Task Delete(int id, [FromServices] IContactRepository contactRepository)
        {
            await contactRepository.DeleteAsync(id);
        }
        [HttpGet("busca")]
        public async Task<IEnumerable<IContact>> Busca(int pagina, int qtdRegistrosPorPagina, string pesquisa, [FromServices] IContactRepository contactRepository)
        {
            qtdRegistrosPorPagina = qtdRegistrosPorPagina > 0 ? qtdRegistrosPorPagina : 5;
            return await contactRepository.Busca(pagina, qtdRegistrosPorPagina, pesquisa);
        }
    }
}
