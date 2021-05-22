using System.Threading.Tasks;

namespace SpeziInspector.Activation
{
    public interface IActivationHandler
    {
        bool CanHandle(object args);

        Task HandleAsync(object args);
    }
}
