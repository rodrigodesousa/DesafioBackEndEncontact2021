using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook;

namespace TesteBackendEnContact.Controllers
{
    public interface IImportador
    {
        Task<string> CriarCsv();
        Task<IEnumerable<Contact>> CsvContact(IFormFile file);
    }
}
