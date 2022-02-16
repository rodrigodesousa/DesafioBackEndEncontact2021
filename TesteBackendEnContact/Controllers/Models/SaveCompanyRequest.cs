using System.ComponentModel.DataAnnotations;
using TesteBackendEnContact.Core.Domain.ContactBook.Company;
using TesteBackendEnContact.Core.Interface.ContactBook.Company;

namespace TesteBackendEnContact.Controllers.Models
{
    public class SaveCompanyRequest
    {
        [Required]
        public int ContactBookId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public ICompany ToCompany(int Id) => new Company(Id, ContactBookId, Name);
        public ICompany ToCompany() => new Company(ContactBookId, Name);
    }
}
