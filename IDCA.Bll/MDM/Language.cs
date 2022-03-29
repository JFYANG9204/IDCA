using System.Collections.Generic;

namespace IDCA.Bll.MDM
{

    internal static class LanguageHelper
    {
        readonly static string[] _longCodes =
        {
            "af-za",
            "sq-al",
            "ar-dz",
            "ar-bh",
            "ar-eg",
            "ar-iq",
            "ar-jo",
            "ar-kw",
            "ar-lb",
            "ar-ly",
            "ar-ma",
            "ar-om",
            "ar-qa",
            "ar-sa",
            "ar-sy",
            "ar-tn",
            "ar-ae",
            "ar-ye",
            "hy-am",
            "eu-es",
            "be-by",
            "bg-bg",
            "ca-es",
            "zh-cn",
            "zh-hk",
            "zh-mo",
            "zh-sg",
            "zh-tw",
            "zh-chs",
            "zh-cht",
            "hr-hr",
            "cs-cz",
            "da-dk",
            "nl-be",
            "nl-nl",
            "en-au",
            "en-bz",
            "en-ca",
            "en-cb",
            "en-ie",
            "en-jm",
            "en-nz",
            "en-ph",
            "en-za",
            "en-tt",
            "en-gb",
            "en-us",
            "en-zw",
            "et-ee",
            "fo-fe",
            "fa-ir",
            "fi-fi",
            "fr-be",
            "fr-ca",
            "fr-fr",
            "fr-lu",
            "fr-mc",
            "fr-ch",
            "de-at",
            "de-de",
            "de-li",
            "de-lu",
            "de-ch",
            "el-gr",
            "he-il",
            "hi-in",
            "hu-hu",
            "is-is",
            "id-id",
            "it-it",
            "it-ch",
            "ja-jp",
            "kk-kz",
            "ko-kr",
            "lv-lv",
            "lt-lt",
            "ms-my",
            "nb-no",
            "nn-no",
            "pl-pl",
            "pt-br",
            "pt-pt",
            "ro-ro",
            "ru-ru",
            "sa-in",
            "cy-sr-sp",
            "lt-sr-sp",
            "sk-sk",
            "sl-si",
            "es-ar",
            "es-bo",
            "es-cl",
            "es-co",
            "es-cr",
            "es-do",
            "es-ec",
            "es-sv",
            "es-gt",
            "es-hn",
            "es-mx",
            "es-ni",
            "es-pa",
            "es-py",
            "es-pe",
            "es-pr",
            "es-es",
            "es-uy",
            "es-ve",
            "sv-fi",
            "sv-se",
            "syr-sy",
            "th-th",
            "tr-tr",
            "uk-ua",
            "ur-pk",
            "vi-vn",
        };

        readonly static string[] _shortCodes =
        {
            "AFK",
            "SQI",
            "ARG",
            "ARH",
            "ARE",
            "ARI",
            "ARJ",
            "ARK",
            "ARB",
            "ARL",
            "ARM",
            "ARO",
            "ARQ",
            "ARA",
            "ARS",
            "ART",
            "ARU",
            "ARY",
            "HYE",
            "EUQ",
            "BEL",
            "BGR",
            "CAT",
            "CHS",
            "ZHH",
            "ZHM",
            "ZHI",
            "CHT",
            "CHS",
            "CHT",
            "HRV",
            "CSY",
            "DAN",
            "NLB",
            "NLD",
            "ENA",
            "ENL",
            "ENC",
            "ENB",
            "ENI",
            "ENJ",
            "ENZ",
            "ENG",
            "ENS",
            "ENG",
            "ENG",
            "ENU",
            "ENW",
            "ETI",
            "FOS",
            "FAR",
            "FIN",
            "FRB",
            "FRC",
            "FRA",
            "FRL",
            "FRA",
            "FRS",
            "DEA",
            "DEU",
            "DEC",
            "DEL",
            "DES",
            "ELL",
            "HEB",
            "HIN",
            "HUN",
            "ISL",
            "IND",
            "ITA",
            "ITS",
            "JPN",
            "KKZ",
            "KOR",
            "LVI",
            "LTH",
            "MSL",
            "NOR",
            "NON",
            "PLK",
            "PTB",
            "PTG",
            "ROM",
            "RUS",
            "SAN",
            "SRB",
            "SRL",
            "SKY",
            "SLV",
            "ESS",
            "ESB",
            "ESL",
            "ESO",
            "ESC",
            "ESD",
            "ESF",
            "ESE",
            "ESG",
            "ESH",
            "ESM",
            "ESI",
            "ESA",
            "ESZ",
            "ESR",
            "ESU",
            "ESS",
            "ESY",
            "ESV",
            "SVF",
            "SVE",
            "SYR",
            "THA",
            "TRK",
            "URK",
            "URD",
            "VIT",
        };

        readonly static string[] _names =
        {
            "Afrikaans - South Africa",
            "Albanian - Albania",
            "Arabic - Algeria",
            "Arabic - Bahrain",
            "Arabic - Egypt",
            "Arabic - Iraq",
            "Arabic - Jordan",
            "Arabic - Kuwait",
            "Arabic - Lebanon",
            "Arabic - Libya",
            "Arabic - Morocco",
            "Arabic - Oman",
            "Arabic - Qatar",
            "Arabic - Saudi Arabia",
            "Arabic - Syria",
            "Arabic - Tunisia",
            "Arabic - United Arab Emirates",
            "Arabic - Yemen",
            "Armenian - Armenia",
            "Basque - Basque",
            "Belarusian - Belarus",
            "Bulgarian - Bulgaria",
            "Catalan - Catalan",
            "Chinese - China",
            "Chinese - Hong Kong SAR",
            "Chinese - Macau SAR",
            "Chinese - Singapore",
            "Chinese - Taiwan",
            "Chinese (Simplified)",
            "Chinese (Traditional)",
            "Croatian - Croatia",
            "Czech - Czech Republic",
            "Danish - Denmark",
            "Dutch - Belgium",
            "Dutch - The Netherlands",
            "English - Australia",
            "English - Belize",
            "English - Canada",
            "English - Caribbean",
            "English - Ireland",
            "English - Jamaica",
            "English - New Zealand",
            "English - Philippines",
            "English - South Africa",
            "English - Trinidad and Tobago",
            "English - United Kingdom",
            "English - United States",
            "English - Zimbabwe",
            "Estonian - Estonia",
            "Faroese - Faroe Islands",
            "Farsi - Iran",
            "Finnish - Finland",
            "French - Belgium",
            "French - Canada",
            "French - France",
            "French - Luxembourg",
            "French - Monaco",
            "French - Switzerland",
            "German - Austria",
            "German - Germany",
            "German - Liechtenstein",
            "German - Luxembourg",
            "German - Switzerland",
            "Greek - Greece",
            "Hebrew - Israel",
            "Hindi - India",
            "Hungarian - Hungary",
            "Icelandic - Iceland",
            "Indonesian - Indonesia",
            "Italian - Italy",
            "Italian - Switzerland",
            "Japanese - Japan",
            "Kazakh - Kazakhstan",
            "Korean - Korea",
            "Latvian - Latvia",
            "Lithuanian - Lithuania",
            "Malay - Malaysia",
            "Norwegian (BokmôÂ¥l) - Norway",
            "Norwegian (Nynorsk) - Norway",
            "Polish - Poland",
            "Portuguese - Brazil",
            "Portuguese - Portugal",
            "Romanian - Romania",
            "Russian - Russia",
            "Sanskrit - India",
            "Serbian (Cyrillic) - Serbia",
            "Serbian (Latin) - Serbia",
            "Slovak - Slovakia",
            "Slovenian - Slovenia",
            "Spanish - Argentina",
            "Spanish - Bolivia",
            "Spanish - Chile",
            "Spanish - Colombia",
            "Spanish - Costa Rica",
            "Spanish - Dominican Republic",
            "Spanish - Ecuador",
            "Spanish - El Salvador",
            "Spanish - Guatemala",
            "Spanish - Honduras",
            "Spanish - Mexico",
            "Spanish - Nicaragua",
            "Spanish - Panama",
            "Spanish - Paraguay",
            "Spanish - Peru",
            "Spanish - Puerto Rico",
            "Spanish - Spain",
            "Spanish - Uruguay",
            "Spanish - Venezuela",
            "Swedish - Finland",
            "Swedish - Sweden",
            "Syriac - Syria",
            "Thai - Thailand",
            "Turkish - Turkey",
            "Ukrainian - Ukraine",
            "Urdu - Pakistan",
            "Vietnamese - Vietnam",
        };

        static readonly Dictionary<string, string> _longCodeToShortCode = new();
        static readonly Dictionary<string, string> _longCodeToName = new();

        static void LoadInformations()
        {
            _longCodeToShortCode.Clear();
            _longCodeToName.Clear();

            for (int i = 0; i < _longCodes.Length; i++)
            {
                _longCodeToShortCode.Add(_longCodes[i], _shortCodes[i]);
                _longCodeToName.Add(_longCodes[i], _names[i]);
            }
        }

        internal static string GetShortCodeFromLongCode(string longCode)
        {
            if (_longCodeToShortCode.Count == 0)
            {
                LoadInformations();
            }
            
            string key = longCode.ToLower();
            return _longCodeToShortCode.ContainsKey(key) ? _longCodeToShortCode[key] : string.Empty;
        }

        internal static string GetNameFromLongCode(string longCode)
        {
            if (_longCodeToName.Count == 0)
            {
                LoadInformations();
            }

            string key = longCode.ToLower();
            return _longCodeToName.ContainsKey(key) ? _longCodeToName[key] : string.Empty;
        }

    }


    public class Language : MDMNamedObject, ILanguage
    {
        internal Language(IMDMObject parent) : base(parent.Document, parent)
        {
            _longCode = "";
            _shortCode = "";
            _name = "";
            _objectType = MDMObjectType.Language;
        }

        internal Language(string longCode, IMDMObject parent) : base(parent.Document, parent)
        {
            _longCode = longCode;
            _name = LanguageHelper.GetNameFromLongCode(longCode);
            _shortCode = LanguageHelper.GetShortCodeFromLongCode(longCode);
            _objectType = MDMObjectType.Language;
        }

        string _shortCode;
        string _longCode;

        public string ShortCode => _shortCode;
        public string LongCode => _longCode;
        new public MDMObjectType ObjectType => _objectType;

        public void SetLongCode(string longCode)
        {
            _longCode = longCode;
            _name = LanguageHelper.GetNameFromLongCode(_longCode);
            _shortCode= LanguageHelper.GetShortCodeFromLongCode(_longCode);
        }

        public bool IsDefault => string.IsNullOrEmpty(_longCode);
    }

    public class Languages : MDMNamedCollection<Language>, ILanguages<Language>
    {

        internal Languages(IMDMDocument document, string @base) : base(document, document, collection => new Language(collection))
        {
            _base = @base;
            _default = new Language(this);
            _objectType = MDMObjectType.Languages;
        }

        string _base;
        readonly ILanguage _default;

        public string Base { get => _base; internal set => _base = value; }
        public ILanguage Default => _default;
        new public MDMObjectType ObjectType => _objectType;

        public override void Add(Language item)
        {
            string lowerName = item.LongCode.ToLower();
            if (!string.IsNullOrEmpty(lowerName) && !_cache.ContainsKey(lowerName))
            {
                _items.Add(item);
                _cache.Add(lowerName, item);
                string lowerId = item.Id.ToLower();
                if (!string.IsNullOrEmpty(lowerId) && !_idCache.ContainsKey(lowerId))
                {
                    _idCache.Add(lowerId, item);
                }
            }
        }

    }
}
