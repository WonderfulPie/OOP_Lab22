using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace Lab22
{
    public static class LocalizationManager
    {
        public static CultureInfo CurrentLanguage { get; set; } = new CultureInfo("en-US");

        private static readonly Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>
        {
            ["File"] = new Dictionary<string, string>
            {
                ["en-US"] = "File",
                ["uk-UA"] = "Файл"
            },
            ["New"] = new Dictionary<string, string>
            {
                ["en-US"] = "New",
                ["uk-UA"] = "Новий"
            },
            ["Open"] = new Dictionary<string, string>
            {
                ["en-US"] = "Open",
                ["uk-UA"] = "Відкрити"
            },
            ["Save"] = new Dictionary<string, string>
            {
                ["en-US"] = "Save",
                ["uk-UA"] = "Зберегти"
            },
            ["Save As"] = new Dictionary<string, string>
            {
                ["en-US"] = "Save As",
                ["uk-UA"] = "Зберегти як"
            },
            ["Settings"] = new Dictionary<string, string>
            {
                ["en-US"] = "Settings",
                ["uk-UA"] = "Налаштування"
            },
            ["Exit"] = new Dictionary<string, string>
            {
                ["en-US"] = "Exit",
                ["uk-UA"] = "Вихід"
            },
            ["Format"] = new Dictionary<string, string>
            {
                ["en-US"] = "Format",
                ["uk-UA"] = "Формат"
            },
            ["Font"] = new Dictionary<string, string>
            {
                ["en-US"] = "Font",
                ["uk-UA"] = "Шрифт"
            },
            ["Color"] = new Dictionary<string, string>
            {
                ["en-US"] = "Color",
                ["uk-UA"] = "Колір"
            },
            ["Help"] = new Dictionary<string, string>
            {
                ["en-US"] = "Help",
                ["uk-UA"] = "Допомога"
            },
            ["About"] = new Dictionary<string, string>
            {
                ["en-US"] = "About",
                ["uk-UA"] = "Про програму"
            },
            ["Language"] = new Dictionary<string, string>
            {
                ["en-US"] = "Language",
                ["uk-UA"] = "Мова"
            },
            ["Ready"] = new Dictionary<string, string>
            {
                ["en-US"] = "Ready",
                ["uk-UA"] = "Готово"
            }
        };

        public static string T(string key)
        {
            if (translations.TryGetValue(key, out var langDict) && langDict.TryGetValue(CurrentLanguage.Name, out var value))
            {
                return value;
            }
            return key;
        }

        public static ResourceDictionary GetResource()
        {
            var resource = new ResourceDictionary();
            foreach (var translation in translations)
            {
                resource.Add(translation.Key, T(translation.Key));
            }
            return resource;
        }

        public static void SetLanguage(CultureInfo culture)
        {
            CurrentLanguage = culture;
            UpdateResourceDictionary();
        }

        public static void UpdateResourceDictionary()
        {
            var dictionary = GetResource();
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dictionary);
        }
    }
}