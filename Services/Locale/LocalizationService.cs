using Contracts;
using Entities;
using Microsoft.Extensions.Localization;

namespace Services.Locale;

public class LocalizationService : ILocalizationService
{
    private readonly IStringLocalizerFactory _localizerFactory;

    private readonly Dictionary<LocaleResourcesEnum, Type> _resourceTypes = new()
    {
        {LocaleResourcesEnum.Account, typeof(Resources.Account)},
        {LocaleResourcesEnum.MDM, typeof(Resources.MDM)},
    };

    public LocalizationService(IStringLocalizerFactory factory)
    {
        _localizerFactory = factory;
    }

    public string GetLocalizedString(string key, LocaleResourcesEnum resourceType)
    {
        var type = _resourceTypes[resourceType];

        var localizer = _localizerFactory.Create(type);

        return localizer[key];
    }
}
