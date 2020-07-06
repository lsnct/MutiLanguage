using MultiLanguage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiLanguage.Interfaces
{
    public interface ILanguageReceiveAdapter
    {
        /// <summary>
        /// 初始化多语言管理器
        /// </summary>
        /// <param name="managerLanguageList">单语言对象的list集合</param>
        void InitLanguageManager(List<SingleLanguage> managerLanguageList);

        /// <summary>
        /// 重新加载多语言管理器
        /// </summary>
        void ReLoadLanguage(List<SingleLanguage> managerLanguageList);

        /// <summary>
        /// 获取已加载多语言管理器
        /// </summary>
        /// <returns>当前多语言管理器</returns>
        LanguageManager GetLanguageManager();
    }
}
