using Microsoft.Extensions.DependencyInjection;
using MultiLanguage.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiLanguage.Common
{
    /// <summary>
    /// 多语言管理
    /// </summary>
    public  class LanguageManager:IDisposable
    {
        /// <summary>
        /// 单门语言类型的集合，存储着这些对象的地址
        /// </summary>
        private  List<SingleLanguage> _managerLanguageList;

        /// <summary>
        /// 存储ManagerLanguageList的索引
        /// </summary>
        private Dictionary<string, LannguageIndex> _managerLanguageListIndex;

        private Dictionary<string, CultureCodeCountry> _managerLanguageCountryList;

        public string DefaultLanguage { get;private set;}
        /// <summary>
        /// 构造函数初始化
        /// </summary>
        /// <param name="managerLanguageList">语言集合</param>
        public LanguageManager(List<SingleLanguage> managerLanguageList)
        {
            _managerLanguageList = new List<SingleLanguage>();

            for (int i = 0; i < managerLanguageList.Count; i++)
            {
                if (managerLanguageList[i] != null && managerLanguageList[i]._isInit)
                {
                    _managerLanguageList.Add(managerLanguageList[i]);
                }
                else
                {
                    continue;
                }
            }
            

            InitManagerLanguageListIndex();
        }      

        /// <summary>
        /// 通过语言索引类初始化字典
        /// </summary>
        private void InitManagerLanguageListIndex()
        {
            _managerLanguageListIndex = new Dictionary<string, LannguageIndex>();
            _managerLanguageCountryList = new Dictionary<string, CultureCodeCountry>();
            foreach (var item in _managerLanguageList)
            {
                //获取每种语言对应的全部索引
                var TempindexList = item.GetAllIndexName();
                for (int i = 0; i < TempindexList.Count; i++)
                {
                    // 加载语言索引
                    // 异常处理，当索引字典中已经包含对应字段时
                    if (_managerLanguageListIndex.ContainsKey(TempindexList[i].Index))
                    {
                        ////当该索引对象的类型为CultureName（标准索引），则替换之前的字典值
                        //if (TempindexList[i].IndexType == "CultureName")
                        //{
                        //    _managerLanguageListIndex[TempindexList[i].Index] = TempindexList[i];
                        //}
                        ////该索引为别名索引,则直接跳过，不予加载入字典表
                        //else if (TempindexList[i].IndexType == "AliasName")
                        //{

                        //}
                        ////默认处理
                        //else
                        //{
                        //    continue;
                        //}
                        throw new Exception("语言名和别名中有重复项。");
                    }
                    else
                    {
                        //将索引加入字典表
                        _managerLanguageListIndex[TempindexList[i].Index] = TempindexList[i];
                    }

                    // 加载语言国别
                    if (_managerLanguageCountryList.ContainsKey(TempindexList[i].Index))
                    {
                        continue;
                    }
                    else
                    {
                        string index = TempindexList[i].Index;
                        string country = TempindexList[i].SingleLanguage.Culture;
                        string cultureCode = TempindexList[i].SingleLanguage.CultureName;

                        CultureCodeCountry cultureCodeCountry = new CultureCodeCountry(cultureCode,country);

                        _managerLanguageCountryList.Add(index, cultureCodeCountry);
                    }
                }
                

            }
        }

        private static object _updateLock;
        /// <summary>
        /// 待实现
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取某一语言下的某一种msgid的翻译
        /// </summary>
        /// <param name="cultureCode">语言code,如zh-CN</param>
        /// <param name="msgid">某条语言的唯一标识</param>
        /// <returns></returns>
        public TranslateResult GetTranslate(string cultureCode,string msgid)
        {
            //获取关联索引
            var AttachLannguageIndex = _managerLanguageListIndex.ContainsKey(cultureCode)? _managerLanguageListIndex[cultureCode]:null;
            var AttachSingleLanguageModel = AttachLannguageIndex.SingleLanguage;

            var msgresult = AttachSingleLanguageModel.GetTranslate(msgid);

            TranslateResult result = new TranslateResult(msgid,msgresult);
            return result;

        }

        /// <summary>
        /// 获取某一语言下的某一种msgid的翻译（msgid中包含参数）
        /// </summary>
        /// <param name="cultureCode">语言code,如zh-CN</param>
        /// <param name="msgid">某条语言的唯一标识</param>
        /// <param name="parms">替换用参数</param>
        /// <returns></returns>
        public TranslateResult GetTranslate(string cultureCode, string msgid,List<string> parms)
        {
            TranslateResult result;
            //获取关联索引
            var AttachLannguageIndex = _managerLanguageListIndex.ContainsKey(cultureCode) ? _managerLanguageListIndex[cultureCode] : null;
            var AttachSingleLanguageModel = AttachLannguageIndex.SingleLanguage;
            if (parms != null && parms.Count > 0)
            {
                var msgresult = AttachSingleLanguageModel.GetTranslate(msgid, parms);
                result = new TranslateResult(msgid, msgresult);
            }
            else
            {
                var msgresult = AttachSingleLanguageModel.GetTranslate(msgid);
                result = new TranslateResult(msgid, msgresult);
            }

             // new TranslateResult(msgid, msgresult);
            return result;

        }

        /// <summary>
        /// 获取某一语言下的全部翻译
        /// </summary>
        /// <param name="cultureCode">语言code,如zh-CN</param>
        /// <returns></returns>
        public List<TranslateResult> GetTranslateList(string cultureCode)
        {
            List<TranslateResult> resultList;
            if (_managerLanguageListIndex.ContainsKey(cultureCode))
            {
                LannguageIndex lannguageIndex = _managerLanguageListIndex[cultureCode];
                SingleLanguage singleLanguage = lannguageIndex.SingleLanguage;

                var TranslatesList =  singleLanguage.GetTranslatesList();

                if (TranslatesList.Count > 0)
                {
                    resultList =  TranslatesList;
                }
                else
                {
                    resultList =  null;
                }
                
            }
            else
            {
                resultList = null;
            }


            return resultList;
        }

        /// <summary>
        ///  设置默认语言，请在构造函数执行完成后执行
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <returns></returns>
        public bool SetDefaultLanguage(string cultureCode)
        {
            var Isresult = false;
            if (_managerLanguageListIndex.ContainsKey(cultureCode))
            {
                DefaultLanguage = cultureCode;
                Isresult = true;
            }
            return Isresult;
        }

        /// <summary>
        ///  获取所有已加载的语言的语言-国家对应关系
        /// </summary>
        /// <returns></returns>
        public List<CultureCodeCountry> GetAllCultureCodeCountries()
        {
            List<CultureCodeCountry> CultureCodeCountries = new List<CultureCodeCountry>();

            foreach (var item in _managerLanguageCountryList)
            {
                CultureCodeCountries.Add(item.Value);
            }

            return CultureCodeCountries;
        }

        /// <summary>
        ///  获取已加载的全部语言的翻译内容
        /// </summary>
        /// <param name="isUseAlias">是否获取别名索引中的翻译内容（这里请先设置false）</param>
        /// <returns></returns>
        public List<LanguageShowData> GetLanguageShowDatas(bool isUseAlias)
        {
            List<LanguageShowData> languageShowDataList = new List<LanguageShowData>();
            foreach (var item in _managerLanguageListIndex)
            {
                if (!isUseAlias)
                {
                    if (item.Value.IndexType == "AliasName")
                    {
                        continue;
                    }
                }

                var TranslateList = GetTranslateList(item.Key);
                var CultureCodeCountry = new CultureCodeCountry(item.Key,item.Value.SingleLanguage.Culture);
               

                bool IsDefault;
                if (item.Value.SingleLanguage.CultureName == DefaultLanguage)
                {
                    IsDefault = true;
                }
                else
                {
                    IsDefault = false;
                }
                LanguageShowData languageShowData = new LanguageShowData(TranslateList, CultureCodeCountry, IsDefault);

                languageShowDataList.Add(languageShowData);
            }

            return languageShowDataList;
        }

        /// <summary>
        /// 获取语言在cultureCode中，msgid在Msgid中的翻译结果（获取自定义翻译结果）
        /// </summary>
        /// <param name="cultureCode">需要加载的语言标识</param>
        /// <param name="Msgid">语言的msgid</param>
        /// <param name="isUseAlias">是否使用别名索引，目前请设置false</param>
        /// <returns></returns>
        public List<LanguageShowData> GetLanguageShowDataParts(List<string> cultureCode, List<string> Msgid, bool isUseAlias)       
        {
            List<LanguageShowData> languageShowDatasList = null;
            if (cultureCode.Count < 1 || Msgid.Count < 1)
            {

            }
            else
            {
                languageShowDatasList = new List<LanguageShowData>();

                foreach (var item in _managerLanguageListIndex)
                {
                    if (!isUseAlias)
                    {
                        if (item.Value.IndexType == "AliasName")
                        {
                            continue;
                        }
                    }

                    if (null == cultureCode.Find(x => x == item.Value.Index))
                    {
                        continue;
                    }

                    

                    var TranslateList = GetTranslateList(item.Key);
                    var CultureCodeCountry = new CultureCodeCountry(item.Key, item.Value.SingleLanguage.Culture);
                   
                    bool IsDefault;
                    if (item.Value.SingleLanguage.CultureName == DefaultLanguage)
                    {
                        IsDefault = true;
                    }
                    else
                    {
                        IsDefault = false;
                    }

                    LanguageShowData languageShowData = new LanguageShowData(TranslateList, CultureCodeCountry, IsDefault);                   

                    for (int j = 0; j < languageShowData.TranslateList.Count; j++)
                    {
                        if (null == Msgid.Find(x => x == languageShowData.TranslateList[j].Source))
                        {
                            languageShowData.TranslateList.Remove(languageShowData.TranslateList[j]);
                            j--;
                        }
                        else
                        {
                            continue;

                        }

                    }

                

                languageShowDatasList.Add(languageShowData);

                }
            }

            return languageShowDatasList;

        }

        /// <summary>
        /// 添加一门语言
        /// </summary>
        /// <param name="singleLanguage"></param>
        /// <returns></returns>
        public bool AddSingleLanguage(SingleLanguage singleLanguage)
        {
            var result = false;
            if (_managerLanguageCountryList.ContainsKey(singleLanguage.CultureName))
            {
                return false;
            }
            _managerLanguageList.Add(singleLanguage);
            var TempindexList = singleLanguage.GetAllIndexName();
            for (int i = 0; i < TempindexList.Count; i++)
            {
                // 加载语言索引
                // 异常处理，当索引字典中已经包含对应字段时
                if (_managerLanguageListIndex.ContainsKey(TempindexList[i].Index))
                {
                    ////当该索引对象的类型为CultureName（标准索引），则替换之前的字典值
                    //if (TempindexList[i].IndexType == "CultureName")
                    //{
                    //    _managerLanguageListIndex[TempindexList[i].Index] = TempindexList[i];
                    //}
                    ////该索引为别名索引,则直接跳过，不予加载入字典表
                    //else if (TempindexList[i].IndexType == "AliasName")
                    //{

                    //}
                    ////默认处理
                    //else
                    //{
                    //    continue;
                    //}
                    return false;
                }
                else
                {
                    //将索引加入字典表
                    _managerLanguageListIndex[TempindexList[i].Index] = TempindexList[i];
                }

                // 加载语言国别
                if (_managerLanguageCountryList.ContainsKey(TempindexList[i].Index))
                {
                    continue;
                }
                else
                {
                    string index = TempindexList[i].Index;
                    string country = TempindexList[i].SingleLanguage.Culture;
                    string cultureCode = TempindexList[i].SingleLanguage.CultureName;

                    CultureCodeCountry cultureCodeCountry = new CultureCodeCountry(cultureCode, country);

                    _managerLanguageCountryList.Add(index, cultureCodeCountry);
                }
            }

            return true;
        }


        /// <summary>
        ///  更新多语言
        /// </summary>
        /// <param name="singleLanguages"></param>
        /// <param name="IsRenew"></param>
        /// <returns></returns>
        public void UpdateSingleLanguage(List<SingleLanguage> singleLanguages,bool IsRenew)
        {
            lock (_updateLock)
            {
                List<SingleLanguage> singleLanguageList = new List<SingleLanguage>();

                if (IsRenew)
                {
                    //以singleLanguages为基础重新构造
                    for (int i = 0; i < singleLanguages.Count; i++)
                    {
                        singleLanguageList.Add(singleLanguages[i]);
                    }
                }
                else
                {
                    //在原有数据基础上，进行singleLanguages改造

                    

                    // 进行深拷贝
                    for (int i = 0; i < _managerLanguageList.Count; i++)
                    {
                        var newLanguage = singleLanguages.Find(x => x.CultureName == _managerLanguageList[i].CultureName);
                        if (newLanguage != null)
                        {
                            // 新的类型里包含了之前的旧数据
                            singleLanguageList.Add(newLanguage);
                        }
                        else
                        {
                            var moUrl = _managerLanguageList[i].MoFilePosition;
                            var cultureName = _managerLanguageList[i].CultureName;
                            var aliasName = _managerLanguageList[i].AliasName;
                            var flagUrl = _managerLanguageList[i].FlagUrl;
                            var culture = _managerLanguageList[i].Culture;

                            var language = new SingleLanguage(cultureName, moUrl, aliasName, culture, flagUrl);
                            singleLanguageList.Add(language);
                        }
                       
                    }
                }


                // 生成好新的singleLanguageList，进行赋值
                _managerLanguageList = singleLanguageList;

            }
        }

        

       
    }


}
