
using Microsoft.Extensions.DependencyInjection;
using MultiLanguage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiLanguage.Common
{
    //多语言构造器
    internal  class LanguageReceiveAdapter: ILanguageReceiveAdapter
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LanguageReceiveAdapter()
        {
            _lock = new object();
        }

        /// <summary>
        /// 多语言管理器
        /// </summary>
        private static LanguageManager _langManager;
        /// <summary>
        /// 实例化加锁
        /// </summary>
        private static object _lock;

        /// <summary>
        /// 初始化记录
        /// </summary>
        private List<SingleLanguage> _managerLanguageList;

        /// <summary>
        /// 初始化多语言管理器
        /// </summary>
        /// <param name="managerLanguageList">单语言对象的list集合</param>
        public void InitLanguageManager(List<SingleLanguage> managerLanguageList)
        {
            //当多个线程同时走到此模块时为防止出现意外，进行加锁处理
            
            if (_langManager == null)
            {
                lock (_lock)
                {
                    if (_langManager == null)
                    {
                        _langManager = new LanguageManager(managerLanguageList);
                        _managerLanguageList = managerLanguageList;
                    }
                }
            }
            else
            {
                return;
            }
            
        }

        /// <summary>
        /// 重新加载多语言管理器（未实装）
        /// </summary>
        /// <param name="managerLanguageList">单语言对象的list集合</param>
        public void ReLoadLanguage(List<SingleLanguage> managerLanguageList = null)
        {
            if (managerLanguageList == null)
            {
                _langManager.Dispose();
                _langManager = new LanguageManager(_managerLanguageList);
            }
            else
            {
                _langManager.Dispose();
                _langManager = new LanguageManager(_managerLanguageList);
            }
            
        }

        /// <summary>
        /// 获取langManager
        /// </summary>
        /// <returns>可以为null的LanguageManager实例化对象</returns>
        public LanguageManager GetLanguageManager()
        {
           
            return _langManager;
        }

        /// <summary>
        /// 向已经初始化完成的manager中添加一门新的语言
        /// </summary>
        /// <param name="singleLanguage"></param>
        /// <returns></returns>
        public bool AddSingleLanguage(SingleLanguage singleLanguage)
        {
            return false;
        }



    }

    
   

}
