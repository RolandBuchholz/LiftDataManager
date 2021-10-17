using System.Threading.Tasks;

namespace LiftDataManager.Activation
{
    public interface IActivationHandler
    {
        bool CanHandle(object args);

        Task HandleAsync(object args);
    }
}
