﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;

namespace TesteBackendEnContact.Repository.Interface
{
    public interface IContactRepository
    {
        Task<IContact> SaveAsync(IContact contactBook);
        Task<IContact> UpdateAsync(IContact contactBook);
        Task DeleteAsync(int id);
        Task<IEnumerable<IContact>> GetAllAsync();
        Task<IContact> GetAsync(int id);
    }
}
