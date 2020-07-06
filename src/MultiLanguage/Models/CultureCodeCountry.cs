using System;
using System.Collections.Generic;
using System.Text;

namespace MultiLanguage.Models
{
    public class CultureCodeCountry
    {
        /// <summary>
        /// 语言code 如zh-CN
        /// </summary>
        public string CultureCode { get; set; }

        /// <summary>
        /// 语言主要归属国别
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cultureCode">语言code 如zh-CN</param>
        /// <param name="country">国别</param>
        public CultureCodeCountry(string cultureCode, string country)
        {
            CultureCode = cultureCode;
            Country = country;
        }
    }
}
