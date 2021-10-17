using System.Threading.Tasks;

namespace LiftDataManager.Contracts.Services
{
    public interface IActivationService
    {
        Task ActivateAsync(object activationArgs);
    }
}
