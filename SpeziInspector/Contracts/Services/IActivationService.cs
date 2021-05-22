using System.Threading.Tasks;

namespace SpeziInspector.Contracts.Services
{
    public interface IActivationService
    {
        Task ActivateAsync(object activationArgs);
    }
}
