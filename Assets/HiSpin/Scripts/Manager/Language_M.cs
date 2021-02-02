using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HiSpin
{
    public class Language_M
    {
        static Dictionary<LanguageCountryEnum, Dictionary<LanguageAreaEnum, string>> multi_language_differ_country;
        static Dictionary<LanguageAreaEnum, string> multi_language_differ_area;
        static readonly Dictionary<string, string> multi_language_differ_value = new Dictionary<string, string>();
        public static bool isJapanese = false;
        public Language_M()
        {
            MultiLanguageData data = Resources.Load<MultiLanguageData>("MultiLanguageData");
            if (data == null)
                throw new MissingReferenceException("MultiLanguage Data is not exist.");
            multi_language_differ_country = new Dictionary<LanguageCountryEnum, Dictionary<LanguageAreaEnum, string>>();
            int countryCount = data.languageUnits.Count;
            for (int i = 0; i < countryCount; i++)
            {
                MultiLanguageUnit multiLanguageUnit = data.languageUnits[i];
                if (!multi_language_differ_country.ContainsKey(multiLanguageUnit.country))
                {
                    Dictionary<LanguageAreaEnum, string> newCountryLanguage = new Dictionary<LanguageAreaEnum, string>();
                    multi_language_differ_country.Add(multiLanguageUnit.country, newCountryLanguage);
                }
                Dictionary<LanguageAreaEnum, string> multi_language_unitCountry = multi_language_differ_country[multiLanguageUnit.country];
                var values = multiLanguageUnit.value_allAreas;
                int valueCount = values.Count;
                for (int valueIndex = 0; valueIndex < valueCount; valueIndex++)
                {
                    if (multi_language_unitCountry.ContainsKey(values[valueIndex].area))
                    {
                        Debug.LogError("MultiLanguage Data own the two or more values as the same area.");
                        continue;
                    }
                    else
                        multi_language_unitCountry.Add(values[valueIndex].area, values[valueIndex].value);
                }
            }
            SystemLanguage language = Application.systemLanguage;
            LanguageCountryEnum languageCountry;
            switch (language)
            {
                case SystemLanguage.Japanese:
                    languageCountry = LanguageCountryEnum.日文;
                    isJapanese = true;
                    break;
                case SystemLanguage.Russian:
                    languageCountry = LanguageCountryEnum.俄文;
                    break;
                case SystemLanguage.Korean:
                    languageCountry = LanguageCountryEnum.韩文;
                    break;
                case SystemLanguage.German:
                    languageCountry = LanguageCountryEnum.德文;
                    break;
                default:
                    languageCountry = LanguageCountryEnum.英文;
                    break;
            }
            ChangeLanguageCountry(languageCountry);
        }
        private static void OnChangeLanguageCountry()
        {
            multi_language_differ_value.Clear();
            var firstLanguageDic = multi_language_differ_country[0];
            foreach (var keyPairs in multi_language_differ_area)
            {
                if (string.IsNullOrEmpty(firstLanguageDic[keyPairs.Key]))
                    continue;
                multi_language_differ_value.Add(firstLanguageDic[keyPairs.Key], keyPairs.Value);
            }
        }
        public static void ChangeLanguageCountry(LanguageCountryEnum languageCountry)
        {
            if (multi_language_differ_country.ContainsKey(languageCountry))
            {
                multi_language_differ_area = multi_language_differ_country[languageCountry];
                OnChangeLanguageCountry();
            }
            else
                Debug.LogError("Change Language Country Error : no this country.");
        }
        public static void ChangeLanguageCountry(int countryIndex)
        {
            ChangeLanguageCountry((LanguageCountryEnum)countryIndex);
        }
        public static void ChangeLanguageCountry(string countryAbbLower)
        {
            if (System.Enum.TryParse(countryAbbLower, out LanguageCountryEnum languageCountry))
            {
                ChangeLanguageCountry(languageCountry);
            }
            else
                Debug.LogError("Change Language Country Error : country abb is no correct.");
        }
        public static string GetMultiLanguageByEnglish(string enValue)
        {
            int areaCount = multi_language_differ_value.Count;
            string lowerValue = enValue.ToLower();
            foreach (var key in multi_language_differ_value.Keys)
            {
                if (lowerValue.Equals(key.ToLower()))
                    if (!Save.data.isPackB)
                        return multi_language_differ_value[key].Replace("$", "");
                    else
                        return multi_language_differ_value[key];
            }
            return "";
        }
        public static string GetMultiLanguageByArea(LanguageAreaEnum languageArea)
        {
            if (multi_language_differ_area.ContainsKey(languageArea))
            {
                if (!Save.data.isPackB)
                    return multi_language_differ_area[languageArea].Replace("$", "");
                else
                    return multi_language_differ_area[languageArea];
            }
            else
                Debug.LogError("Get " + languageArea + " Language Error : no this area.");
            return "";
        }
    }
}
