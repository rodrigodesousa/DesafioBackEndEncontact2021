using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook;
using TesteBackendEnContact.Core.Domain.ContactBook.Company;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;
using TesteBackendEnContact.Database;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Repository
{
    public class ContactRepository : IContactRepository
    {
        private readonly DatabaseConfig databaseConfig;

        public ContactRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }
        public async Task DeleteAsync(int id)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var sql = new StringBuilder();
            sql.AppendLine("DELETE FROM Contact WHERE Id = @id;");

            await connection.ExecuteAsync(sql.ToString(), new { id });
        }

        public async Task<IEnumerable<IContact>> GetAllAsync()
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = "SELECT * FROM Contact";
            var result = await connection.QueryAsync<ContactDao>(query);

            return result?.Select(item => item.Export());
        }

        public async Task<IContact> GetAsync(int id)
        {
            var list = await GetAllAsync();

            return list.ToList().Where(item => item.Id == id).FirstOrDefault();
        }

        public async Task<IContact> SaveAsync(IContact contact)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            var dao = new ContactDao(contact);

            dao.Id = await connection.InsertAsync(dao);

            return dao.Export();
        }
        public async Task<IEnumerable<IContact>> Busca(int pagina, int qtdRegistrosPorPagina, string pesquisa)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var queryCountRegistros = "SELECT count(*) FROM Contact";
            var qtdRegistros = (await connection.QueryAsync<int>(queryCountRegistros));

            int totalPaginas = (int)Math.Ceiling(Convert.ToDecimal(qtdRegistros.First()) / Convert.ToDecimal(qtdRegistrosPorPagina));

            var skip = qtdRegistrosPorPagina * pagina;
            pesquisa = "%" + pesquisa + "%";
            var query = @"SELECT * from Contact as cont where" +
                " cont.Name LIKE @pesquisa OR cont.Email LIKE @pesquisa OR cont.Phone LIKE @pesquisa OR cont.Address LIKE @pesquisa" +
                " limit @qtdRegistrosPorPagina OFFSET @skip;";
            var result = await connection.QueryAsync<ContactDao>(query, new { pesquisa, skip, qtdRegistrosPorPagina });

            var queryCompany = @"SELECT * from Company inner join Contact on Contact.CompanyId = Company.id where Company.name LIKE @pesquisa" +
                " limit @qtdRegistrosPorPagina OFFSET @skip;";
            var resultCompany = await connection.QueryAsync<ContactDao>(queryCompany, new { pesquisa, skip, qtdRegistrosPorPagina });
            
            var list = result.ToList().Concat(resultCompany.ToList());

            return list?.GroupBy(x => x.Id).Select(x => x.First().Export());
        }
        public async Task<IEnumerable<IContact>> BuscaContatosEmpresa(int companyId, int pagina, int qtdRegistrosPorPagina)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var queryCountRegistros = "SELECT count(*) FROM Contact";
            var qtdRegistros = (await connection.QueryAsync<int>(queryCountRegistros));

            int totalPaginas = (int)Math.Ceiling(Convert.ToDecimal(qtdRegistros.First()) / Convert.ToDecimal(qtdRegistrosPorPagina));

            var skip = qtdRegistrosPorPagina * pagina - 1;  

            var query = @"SELECT * from Contact cont where cont.CompanyId = @companyId" +
                " limit @qtdRegistrosPorPagina OFFSET @skip;";
            var result = await connection.QueryAsync<ContactDao>(query, new { companyId, skip, qtdRegistrosPorPagina });

            return result?.Select(item => item.Export());
        }
        public async Task<IContact> UpdateAsync(IContact contact)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            var dao = new ContactDao(contact);

            await connection.UpdateAsync(dao);

            return dao.Export();
        }
    }
    public class Pagination<TEntity> where TEntity : class
    {
        public Pagination(double countRows, double numberOfPages, IEnumerable<TEntity> listResult)
        {
            CountRows = countRows;
            NumberOfPages = numberOfPages;
            ListResult = listResult;
        }

        public double CountRows { get; private set; }
        public double NumberOfPages { get; private set; }
        public IEnumerable<TEntity> ListResult { get; private set; }
    }
    [Table("Contact")]
    public class ContactDao : IContact
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public int ContactBookId { get; set; }

        public int? CompanyId { get; set; }
        public ContactDao()
        {
        }

        public ContactDao(IContact contact)
        {
            Id = contact.Id;
            Name = contact.Name;
            Phone = contact.Phone;
            Email = contact.Email;
            Address = contact.Address;
            ContactBookId = contact.ContactBookId;
            CompanyId = contact.CompanyId;
        }

        public IContact Export() => new Contact(Id, Name, Phone, Email, Address, ContactBookId, CompanyId);
    }
}
