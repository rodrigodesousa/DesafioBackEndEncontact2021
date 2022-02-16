using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Controllers.Models;
using TesteBackendEnContact.Core.Interface.ContactBook.Company;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ILogger<CompanyController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ICompany>> Post(SaveCompanyRequest company, [FromServices] ICompanyRepository companyRepository, [FromServices] IContactBookRepository contactBookRepository)
        {
            var contactBook = await contactBookRepository.GetAsync(company.ContactBookId);

            if(contactBook != null)
                return Ok(await companyRepository.SaveAsync(company.ToCompany()));
            
            return NotFound("Agenda não econtrada");
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<ICompany>> Put(int id, SaveCompanyRequest company, [FromServices] ICompanyRepository companyRepository, [FromServices] IContactBookRepository contactBookRepository)
        {
            var companyToEdit = await companyRepository.GetAsync(id);

            if (companyToEdit == null)
                return NotFound("Empresa não econtrada");

            var contactBook = await contactBookRepository.GetAsync(company.ContactBookId);
            
            if (contactBook == null)
                return NotFound("Agenda não econtrada");

            return Ok(await companyRepository.UpdateAsync(company.ToCompany(id)));            
        }

        [HttpDelete]
        public async Task Delete(int id, [FromServices] ICompanyRepository companyRepository)
        {
            await companyRepository.DeleteAsync(id);
        }

        [HttpGet]
        public async Task<IEnumerable<ICompany>> Get([FromServices] ICompanyRepository companyRepository)
        {
            return await companyRepository.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ICompany> Get(int id, [FromServices] ICompanyRepository companyRepository)
        {
            return await companyRepository.GetAsync(id);
        }
    }
}
