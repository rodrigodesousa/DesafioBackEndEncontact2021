﻿namespace TesteBackendEnContact.Core.Interface.ContactBook.Contact
{
    public interface IContact
    {
        int Id { get; }
        string Name { get; }
        string Phone { get; }
        string Email { get; }
        string Address { get; }
        int ContactBookId { get; }
        int? CompanyId { get; }
    }
}
