using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Controllers
{
    public class Importador : IImportador
    {
        private readonly IContactRepository contactRepository;
        private readonly IContactBookRepository contactBookRepository;
        private readonly ICompanyRepository companyRepository;


        public Importador(IContactRepository contactRepository, IContactBookRepository contactBookRepository, ICompanyRepository companyRepository)
        {
            this.contactRepository = contactRepository;
            this.contactBookRepository = contactBookRepository;
            this.companyRepository = companyRepository;
        }

        public async Task<string> CriarCsv()
        {
            var contacts = await contactRepository.GetAllAsync();

            var builder = new StringBuilder();
            builder.AppendLine("Id;Name;Phone;Email;Address;ContactBookId;CompanyId");

            foreach (Contact item in contacts)
            {
                builder.AppendLine($"{item.Id};{item.Name};{item.Phone};{item.Email};{item.Address};{item.CompanyId};{item.ContactBookId}");
            }

            return builder.ToString();
        }

        public async Task<IEnumerable<Contact>> CsvContact(IFormFile file)
        {
            var contacts = new List<Contact>();

            using (var sreader = new StreamReader(file.OpenReadStream()))
            {
                string[] headers = sreader.ReadLine().Split(';');

                while (!sreader.EndOfStream)
                {
                    Contact cont = StringToContact(sreader.ReadLine());

                    contacts.Add(cont);
                }
            }

            if (contacts.Count <= 0)
                return null;

            List<Contact> contactsSaveResult = new();

            foreach (var contact in contacts)
            {
                try
                {
                    if (contact.ContactBookId > 0)
                    {
                        var contactBookExist = await contactBookRepository.GetAsync(contact.ContactBookId);

                        if (contactBookExist != null)
                        {
                            if(contact.CompanyId != null)
                            {
                                var company = await companyRepository.GetAsync(contact.CompanyId.Value);

                                if(company != null)
                                {
                                    var responseAdd = await contactRepository.SaveAsync(contact);

                                    if (responseAdd.Id > 0)
                                        contactsSaveResult.Add(new Contact(contact.Id, contact.Name, contact.Phone, contact.Email, contact.Address, contact.ContactBookId, contact.CompanyId));

                                }
                            }
                            else
                            {
                                var responseAdd = await contactRepository.SaveAsync(contact);

                                if (responseAdd.Id > 0)
                                    contactsSaveResult.Add(new Contact(contact.Id, contact.Name, contact.Phone, contact.Email, contact.Address, contact.ContactBookId, contact.CompanyId));

                            }
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            return contactsSaveResult;
        }

        private Contact StringToContact(string contatoString)
        {

            var data = contatoString.Split(";");
            return new Contact(data[0], data[1], data[2], data[3], (data[4] == "" ? 0 : int.Parse(data[4])), (data[5] == "" ? null : int.Parse(data[5])));
        }
    }
}
