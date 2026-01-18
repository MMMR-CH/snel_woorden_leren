using System.Collections.Generic;
using SWL.App.Ports;
using SWL.Core.Domain.Localization;

namespace SWL.Infrastructure.Localization
{
    /// <summary>
    /// Minimal localization service. Uses a small in-memory dictionary.
    /// Replace with a real localization package later.
    /// </summary>
    public sealed class StubLocalizationService : ILocalizationService
    {
        private readonly Dictionary<string, Dictionary<string, string>> _tables = new();

        public string CurrentLanguageCode { get; private set; } = LanguageCodes.EN;

        public IReadOnlyList<string> AvailableLanguageCodes { get; } = new[]
        {
            LanguageCodes.EN,
            LanguageCodes.NL,
            LanguageCodes.TR,
            LanguageCodes.AR,
            LanguageCodes.DE,
            LanguageCodes.ES
        };

        public StubLocalizationService()
        {
            // Very small sample keys
            _tables[LanguageCodes.EN] = new Dictionary<string, string>
            {
                { "play", "Play" },
                { "settings", "Settings" },
                { "shop", "Shop" },
                { "home", "Home" },
                { "league", "League" },
                { "words", "Words" }
            };

            _tables[LanguageCodes.NL] = new Dictionary<string, string>
            {
                { "play", "Spelen" },
                { "settings", "Instellingen" },
                { "shop", "Winkel" },
                { "home", "Home" },
                { "league", "Competitie" },
                { "words", "Woorden" }
            };
        }

        public void SetLanguage(string languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode)) return;
            CurrentLanguageCode = languageCode.ToUpperInvariant();
        }

        public string Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return "";

            if (_tables.TryGetValue(CurrentLanguageCode, out var table) && table.TryGetValue(key, out var value))
                return value;

            // Fallback to EN
            if (_tables.TryGetValue(LanguageCodes.EN, out var en) && en.TryGetValue(key, out var enValue))
                return enValue;

            return key;
        }
    }
}
