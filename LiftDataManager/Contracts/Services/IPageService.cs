using System;

namespace LiftDataManager.Contracts.Services
{
    public interface IPageService
    {
        Type GetPageType(string key);
    }
}
