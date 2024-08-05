using Entities;

namespace Contracts;

public interface ILocalizationService
{
    string GetLocalizedString(string key, LocaleResourcesEnum resourceType);
}
