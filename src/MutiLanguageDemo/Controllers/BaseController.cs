using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiLanguage.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using MultiLanguage.Interfaces;
using System.IO;
using MultiLanguage.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MutiLanguageDemo.Controllers
{
    public class BaseController : Controller
    {
        protected ILanguageReceiveAdapter _languageReceiveAdapter;
        protected readonly IHostingEnvironment _hostingEnvironment;
        protected string _contentRootPath;
        protected LanguageManager _languageManager;
        protected string _currentLanguage;

        private static object _locker;
        public BaseController(IServiceProvider serviceProvider, IHostingEnvironment hostingEnvironment)
        {
            _locker = new object();
            _languageReceiveAdapter = serviceProvider.GetService<ILanguageReceiveAdapter>();

            _hostingEnvironment = hostingEnvironment;
            // 在这里，我默认语言文件为{语言标识符}.mo,如en-US.mo，依此类推
            _contentRootPath = _hostingEnvironment.WebRootPath + @"\mofile";
            _languageManager = _languageReceiveAdapter.GetLanguageManager();
            
            // 检测语言管理器是否初始化
            if (_languageManager == null)
            {
                lock (_locker)
                {
                    if (_languageManager == null)
                    {
                        List<SingleLanguage> singleLanguages = CreateSingleLanguageList(_contentRootPath);
                        _languageReceiveAdapter.InitLanguageManager(singleLanguages);

                        _languageManager = _languageReceiveAdapter.GetLanguageManager();
                        _languageManager.SetDefaultLanguage("zh-CN");
                    }
                }
            }

            _currentLanguage = _languageManager.DefaultLanguage;



        }

        /// <summary>
        /// 在_languageManager未完成初始化的时候，调用此方法创建语言对象，并在构造函数里初始化_languageManger
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<SingleLanguage> CreateSingleLanguageList(string path)
        {
            DirectoryInfo root = new DirectoryInfo(path);
            List<SingleLanguage> singleLanguages = new List<SingleLanguage>();

            foreach (FileInfo f in root.GetFiles())
            {
                if (f.Name.Substring(f.Name.Length - 3) == ".mo")
                {
                    var cultureCode = f.Name.Replace(".mo", "");
                    var moUrl = f.FullName;
                    SingleLanguage singleLanguage = new SingleLanguage(cultureCode,moUrl);
                    singleLanguages.Add(singleLanguage);
                }
                
            }

            return singleLanguages;



        }

        /// <summary>
        /// 获取名称为cultureCode全部翻译
        /// </summary>
        /// <param name="cultureCode">名称，如zh-CN</param>
        /// <returns></returns>
        public virtual JsonResult GetListTranslateResult(string cultureCode)
        {
            List<TranslateResult> resultObject = _languageManager.GetTranslateList(cultureCode);
            return Json(resultObject);
        }

        /// <summary>
        /// 获取名称为cultureCode且msgid为msgId的单条翻译
        /// </summary>
        /// <param name="cultureCode">名称，如zh-CN</param>
        /// <param name="msgId">单条翻译的唯一标识</param>
        /// <returns></returns>
        public virtual JsonResult GetSimpleTranslateResult(string cultureCode,string msgId)
        {
            TranslateResult resultObject = _languageManager.GetTranslate(cultureCode,msgId);
            return Json(resultObject);
        }

        /// <summary>
        /// 获取默认语言的全部翻译
        /// </summary>
        /// <returns></returns>
        public virtual JsonResult GetDefaultListTranslateResult()
        {
            List<TranslateResult> resultObject = _languageManager.GetTranslateList(_languageManager.DefaultLanguage);
            return Json(resultObject);
        }


        public virtual JsonResult GetAllListTranslateResult(bool IsUseAlias = false)
        {
            List<LanguageShowData> LanguageShowDatas = _languageManager.GetLanguageShowDatas(IsUseAlias);
           
            return Json(LanguageShowDatas);
        }

        public virtual JsonResult GetAllListTranslateWithCurrentLanguageResult(bool IsUseAlias = false)
        {
            List<LanguageShowData> LanguageShowDatas = _languageManager.GetLanguageShowDatas(IsUseAlias);

            for (int i = 0; i < LanguageShowDatas.Count; i++)
            {
                if (LanguageShowDatas[i].CultureCodeCountry.CultureCode == _currentLanguage)
                {
                    LanguageShowDatas[i].IsSystemDefault = true;

                }
                else
                {
                    LanguageShowDatas[i].IsSystemDefault = false;
                }
            }

            return Json(LanguageShowDatas);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            

            if (RouteData.Values.ContainsKey("lang"))
            {
                _currentLanguage = RouteData.Values["lang"].ToString().Trim();

                var countryCulture = _languageManager.GetAllCultureCodeCountries();

                if (null == countryCulture.Find(x => x.CultureCode == _currentLanguage))
                {
                    _currentLanguage = _languageManager.DefaultLanguage;
                }
            }
            else
            {

            }

            base.OnActionExecuting(context);
        }
    }
}