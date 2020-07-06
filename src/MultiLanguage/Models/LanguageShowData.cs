using System;
using System.Collections.Generic;
using System.Text;

namespace MultiLanguage.Models
{
    public class LanguageShowData
    {
        public List<TranslateResult> TranslateList { get; set; }

        public CultureCodeCountry CultureCodeCountry { get; set; }

        public bool IsSystemDefault { get; set; }

        public LanguageShowData(List<TranslateResult> translateList, CultureCodeCountry cultureCodeCountry, bool isSystemDefault)
        {
            TranslateList = translateList;
            CultureCodeCountry = cultureCodeCountry;
            IsSystemDefault = isSystemDefault;
        }
    }
}
