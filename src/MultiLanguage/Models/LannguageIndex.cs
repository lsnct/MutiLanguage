using MultiLanguage;
using MultiLanguage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiLanguage.Models
{
    /// <summary>
    /// 语言索引类
    /// </summary>
    internal class LannguageIndex
    {
        public string Index { get; private set; }

        public string IndexType { get; private set; }
        public SingleLanguage SingleLanguage { get; private set; }

       

        public LannguageIndex(string index,string indexType, SingleLanguage singleLanguage)
        {
            Index = index;
            IndexType = indexType;
            SingleLanguage = singleLanguage;
        }


    }
}
