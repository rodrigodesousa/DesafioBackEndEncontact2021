using System.Collections.Generic;
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
        Task<IEnumerable<IContact>> Busca(int pagina, int qtdRegistrosPorPagina, string pesquisa);
        Task<IEnumerable<IContact>> BuscaContatosEmpresa(int companyId, int pagina, int qtdRegistrosPorPagina);
    }
}
