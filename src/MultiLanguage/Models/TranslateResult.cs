using System;
using System.Collections.Generic;
using System.Text;

namespace MultiLanguage.Models
{
    /// <summary>
    /// 返回翻译结果
    /// </summary>
    public class TranslateResult
    {
        /// <summary>
        /// 源文字
        /// </summary>
        public string Source { get; set; }



        /// <summary>
        /// 翻译结果
        /// </summary>
        public string Result { get; set; }


        public TranslateResult(string source,string result)
        {
            Source = source;
            Result = result;
        }
    }
}
