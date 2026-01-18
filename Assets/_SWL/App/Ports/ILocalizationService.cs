using System.Collections.Generic;

namespace SWL.App.Ports
{
    public interface ILocalizationService
    {
        string CurrentLanguageCode { get; }
        IReadOnlyList<string> AvailableLanguageCodes { get; }

        void SetLanguage(string languageCode);

        /// <summary>Returns the localized string for a key. Fallback is key itself.</summary>
        string Get(string key);
    }
}
