using System.ComponentModel.DataAnnotations;
using TesteBackendEnContact.Core.Domain.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;

namespace TesteBackendEnContact.Controllers.Models
{
    public class SaveContactRequest
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        [Required]
        public int ContactBookId { get; set; }

        public int? CompanyId { get; set; }

        public IContact ToContact(int Id) => new Contact(Id, Name, Phone, Email, Address, ContactBookId, CompanyId);
        public IContact ToContact() => new Contact(Name, Phone, Email, Address, ContactBookId, CompanyId);
    }
}
