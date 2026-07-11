using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTranslate;
using GTranslate.Translators;
using Services.Abstraction;


namespace Services.Translation
{
    public class GTranslateTranslationService : ITranslationServices
    {
        private readonly GoogleTranslator _translator = new GoogleTranslator();

        public async Task<string> TranslateToArabicAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            try
            {
                var result = await _translator.TranslateAsync(text, "ar");
                return result.Translation;
            }
            catch
            {
                return text; 
            }
        }
    }
}
