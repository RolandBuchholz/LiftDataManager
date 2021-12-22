using System.Threading.Tasks;

namespace LiftDataManager.Core.Contracts.Services
{
    public interface IVaultDataService
    {
        Task<int> GetFileAsync(string auftragsnummer);
        Task<int> SetFileAsync(string auftragsnummer);
    }
}
