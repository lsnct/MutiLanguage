using MultiLanguage;
using MultiLanguage.Models;
using NGettext;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

 namespace MultiLanguage.Common
{

    /// <summary>
    /// 配合LanguageManager使用，是其中的列表中的单对象,参照https://msdn.microsoft.com/zh-cn/library/system.globalization.cultureinfo.aspx#CultureNames
    /// </summary>
    public class SingleLanguage:IDisposable
    {
        /// <summary>
        /// 区域性名称
        /// </summary>
        public string CultureName { get; internal set; }
        /// <summary>
        /// 区域性
        /// </summary>
        public string Culture { get; internal set; }

        /// <summary>
        /// 区域性名称的别名
        /// </summary>
        public List<string> AliasName { get; internal set; }


        /// <summary>
        /// MO文件所在路径
        /// </summary>
        public string MoFilePosition { get; internal set; }
        /// <summary>
        ///  国旗url
        /// </summary>
        public string FlagUrl { get; internal set; }

        /// <summary>
        /// MO文件语言缓存（待完成）
        /// </summary>
        private MemoryStream _singleLanguageMemory;

        /// <summary>
        /// NGettext形成的Catalog
        /// </summary>
        private Catalog _catalog;


        /// <summary>
        /// 是否完成初始化
        /// </summary>
        internal bool _isInit;

        /// <summary>
        /// .net 对语言支持的类
        /// </summary>
        private CultureInfo _cultureInfo;

        /// <summary>
        /// 初始化语言缓存委托
        /// </summary>
        private Action<string> InitMemoryDelegate;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cultureName">填写区域性名称，如en-US</param>
        /// <param name="moUrl">构造MO文件所在路径</param>
        /// <param name="culture">地区名称</param>
        /// <param name="flagUrl">国旗url</param>
        /// <param name="aliasName">初始化别名列表</param>
        public SingleLanguage(string cultureName, string moUrl, List<string> aliasName = null, string culture = "", string flagUrl = "")
        {
            _isInit = false;
            _isInit = InitCulture(cultureName, moUrl, culture, flagUrl, InitMemory, aliasName);
        }


        /// <summary>
        /// 初始化Culture
        /// </summary>
        /// <param name="cultureName">填写区域性名称，如en-US</param>
        /// <param name="moUrl">构造MO文件所在路径</param>
        /// <param name="culture">地区名称</param>
        /// <param name="initMemoryDelegate">读取内存的委托</param>
        /// <param name="aliasName">初始化别名列表</param>
        /// <returns></returns>
        private bool InitCulture(string cultureName, string moUrl, string culture, string flagUrl, Action<string> initMemoryDelegate, List<string> aliasName)
        {
            bool IsResult = true;

            if (string.IsNullOrEmpty(cultureName) || string.IsNullOrEmpty(moUrl))
            {
                //缺少必要的参数
                IsResult = false;

            }

            //初始化CultureName，Culture，_cultureInfo
            try
            {
                _cultureInfo = new CultureInfo(cultureName, false);
                if (string.IsNullOrEmpty(culture))
                {
                    this.Culture = _cultureInfo.NativeName;
                }
                else
                {
                    this.Culture = culture;
                }

                this.CultureName = _cultureInfo.Name;
            }
            catch (Exception ex)
            {
                //log......不支持的cultureName类型
                IsResult = false;

            }

            if (IsResult)
            {

                //初始化别名
                AliasName = new List<string>();

                if (aliasName != null && aliasName.Count > 0)
                {
                    for (int i = 0; i < aliasName.Count; i++)
                    {
                        AliasName.Add(new StringBuilder(aliasName[i]).ToString());
                    }
                }


                //if (!AliasName.Contains(cultureName.ToUpper()))
                //{
                //    AliasName.Add(cultureName.ToUpper());
                //}

                //if (!AliasName.Contains(cultureName.ToLower()))
                //{
                //    AliasName.Add(cultureName.ToLower());
                //}

                //初始化MO文件所在路径
                this.MoFilePosition = moUrl;
                InitMemoryDelegate = initMemoryDelegate;
                if (IsResult)
                {
                    IsResult = InitMemoryStreamWithPath();
                }

                this.FlagUrl = flagUrl;
            }



            return IsResult;
        }

        /// <summary>
        /// 根据文件路径初始化内存并保存路径
        /// </summary>
        /// <param name="path">文件路径</param>
        private bool InitMemoryStreamWithPath()
        {
            bool IsResult = true;
            if (string.IsNullOrEmpty(MoFilePosition))
            {
                IsResult = false;
            }

            if (IsResult)
            {
                try
                {
                    InitMemoryDelegate(MoFilePosition);
                }
                catch (Exception ex)
                {
                    _singleLanguageMemory = null;
                    IsResult = false;
                }

                if (_singleLanguageMemory == null)
                {
                     IsResult = false;
                }
            }

            return IsResult;
        }

        /// <summary>
        /// 生成全部索引
        /// </summary>
        internal List<LannguageIndex> GetAllIndexName()
        {
            LannguageIndex Lang = new LannguageIndex(CultureName, "CultureName",this);
            List<LannguageIndex> LangList = new List<LannguageIndex>();

            LangList.Add(Lang);

            foreach (var item in AliasName)
            {
                var TempLang = LangList.Find(x => x.Index == item);
                if (TempLang == null)
                {
                    LangList.Add(new LannguageIndex(item, "AliasName", this));
                }
            }

            return LangList;

        }
        /// <summary>
        /// 只考虑了本地，没有考虑远程（例如http协议，所以在这个地方我做成了委托）
        /// </summary>
        /// <param name="path"></param>
        private  void InitMemory(string path)
        {
            // 远程加载多语言模块
            if (path.Contains("Http:") || path.Contains("http:") || path.Contains("Https:") || path.Contains("https:"))
            {

            }
            else
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var ms = new MemoryStream())
                    {
                        fs.CopyTo(ms);
                        ms.Position = 0;

                        _catalog = new Catalog(ms, new CultureInfo(CultureName, false));
                        this._singleLanguageMemory = ms;
                    }
                }
            }
        }

        public void Dispose()
        {
            _singleLanguageMemory.Close();
            _singleLanguageMemory.Dispose();
        }


        internal string GetTranslate(string msgid)
        {
            string result = "";

            if (string.IsNullOrEmpty(msgid))
            {
                throw new Exception("无效的msgid");
            }

            if (_catalog != null)
            {
                var Translations = _catalog.Translations;

                foreach (var item in Translations)
                {
                    if (item.Key.Equals(msgid))
                    {
                        result = item.Value[0].ToString();
                        break;
                    }
                }

                return result;
            }
            else
            {
                throw new Exception("语言类型初始化失败！");
            }

            
            
        }

        internal List<TranslateResult> GetTranslatesList()
        {
            List<TranslateResult> result = new List<TranslateResult>();

           
            if (_catalog != null)
            {
                var Translations = _catalog.Translations;

                foreach (var item in Translations)
                {
                   
                    TranslateResult translateResult = new TranslateResult(item.Key,item.Value[0]);
                    result.Add(translateResult);
                }

                
            }
            else
            {
              
            }

            return result;
        }


        internal string GetTranslate(string msgid,List<string> parms)
        {
            string result = "";

            if (string.IsNullOrEmpty(msgid))
            {
                throw new Exception("无效的msgid");
            }

            if (_catalog != null)
            {
                var Translations = _catalog.Translations;

                foreach (var item in Translations)
                {
                    if (item.Key.Equals(msgid))
                    {
                        result = item.Value[0].ToString();
                    }
                }

                for (int i = 0; i < parms.Count; i++)
                {
                    var replaceString = "{" + i + "}";
                    if (result.Contains(replaceString))
                    {
                        result = result.Replace(replaceString, parms[i]);
                    }
                }

                return result;
            }
            else
            {
                throw new Exception("语言类型初始化失败！");
            }



        }

        /// <summary>
        /// 由调用者决定(但这样又会导致重新加载是失败的)
        /// </summary>
        /// <param name="memoryStream">内存流</param>
        public void InitMemoryByMemoryStream(MemoryStream memoryStream)
        {
            try
            {
                _catalog = new Catalog(memoryStream, new CultureInfo(CultureName, false));
                this._singleLanguageMemory = memoryStream;


            }
            catch(Exception ex)
            {
                throw new Exception("请检查内存流指针位置或内存流是否正确初始化。");
            }
        }

       
    }
    
}
