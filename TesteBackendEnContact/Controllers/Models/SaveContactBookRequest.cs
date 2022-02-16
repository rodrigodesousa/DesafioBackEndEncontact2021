using System.ComponentModel.DataAnnotations;
using TesteBackendEnContact.Core.Domain.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook;

namespace TesteBackendEnContact.Controllers.Models
{
    public class SaveContactBookRequest
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public IContactBook ToContactBook(int Id) => new ContactBook(Id, Name);
        public IContactBook ToContactBook() => new ContactBook(Name);
    }
}
