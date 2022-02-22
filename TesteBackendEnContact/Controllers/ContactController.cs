using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
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
        private readonly IImportador importador;

        public ContactController(ILogger<ContactController> logger, IImportador importador)
        {
            _logger = logger;
            this.importador = importador;
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
        [HttpPut("{id}")]
        public async Task<ActionResult<IContact>> Put(int id, SaveContactRequest contact, [FromServices] IContactRepository contactRepository, [FromServices] ICompanyRepository companyRepository, [FromServices] IContactBookRepository contactBookRepository)
        {
            if (contact.CompanyId != null)
            {
                var company = await companyRepository.GetAsync(contact.CompanyId.Value);

                if (company == null)
                    return NotFound("Empresa não econtrada");
            }

            var contactBook = await contactBookRepository.GetAsync(contact.ContactBookId);

            if (contactBook == null)
                return NotFound("Agenda não econtrada");

            return Ok(await contactRepository.UpdateAsync(contact.ToContact(id)));
        }

        [HttpDelete]
        public async Task Delete(int id, [FromServices] IContactRepository contactRepository)
        {
            await contactRepository.DeleteAsync(id);
        }
        [HttpGet("{id}")]
        public async Task<IContact> Get(int id, [FromServices] IContactRepository contactRepository)
        {
            return await contactRepository.GetAsync(id);
        }
        [HttpGet("busca")]
        public async Task<IEnumerable<IContact>> Busca(int pagina, int qtdRegistrosPorPagina, string pesquisa, [FromServices] IContactRepository contactRepository)
        {
            qtdRegistrosPorPagina = qtdRegistrosPorPagina > 0 ? qtdRegistrosPorPagina : 5;
            return await contactRepository.Busca(pagina, qtdRegistrosPorPagina, pesquisa);
        }
        [HttpGet("empresa/{companyId}")]
        public async Task<IEnumerable<IContact>> BuscaContatosEmpresa(int companyId, int pagina, int qtdRegistrosPorPagina, [FromServices] IContactRepository contactRepository)
        {
            qtdRegistrosPorPagina = qtdRegistrosPorPagina > 0 ? qtdRegistrosPorPagina : 5;
            return await contactRepository.BuscaContatosEmpresa(companyId, pagina, qtdRegistrosPorPagina);
        }
        [HttpGet]
        [Route("criar-csv")]
        public async Task<ActionResult> CriarCsvContact()
        {
            try
            {
                var csvFile = await importador.CriarCsv();

                if (string.IsNullOrEmpty(csvFile))
                    return NoContent();

                var file = File(Encoding.UTF8.GetBytes(csvFile), "text/csv", "contacts.csv");

                return file;
            }
            catch (Exception ex)
            {
                return BadRequest("Desculpe! Ocorreu um erro! - " + ex.Message);
            }
        }
        [HttpPost]
        [Route("csv-contact")]
        public async Task<ActionResult<IEnumerable<SaveContactRequest>>> UploadContactCsvFileAsync(IFormFile file)
        {
            var incio = DateTime.Now;

            if (file == null)
                return BadRequest("O arquivo não foi anexado.");

            if (!file.FileName.EndsWith(".csv"))
                return BadRequest("O arquivo carregado não é do tipo .csv");

            try
            {
                var contactsSaveResult = await importador.CsvContact(file);

                if (contactsSaveResult == null)
                    return UnprocessableEntity("Nem todos contatos foram salvos.");

                return Ok(contactsSaveResult);
            }
            catch (Exception ex)
            {
                return BadRequest("Desculpe! Ocorreu um erro! - " + ex.Message);
            }
            finally
            {
                var final = DateTime.Now;

                var diff = final.Subtract(incio);

                _logger.LogInformation($"======= Tempo decorrido: {diff}");
            }
        }
    }
}
