using System;

namespace SpeziInspector.Contracts.Services
{
    public interface IPageService
    {
        Type GetPageType(string key);
    }
}
