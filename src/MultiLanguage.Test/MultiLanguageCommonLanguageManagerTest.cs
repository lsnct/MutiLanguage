using Microsoft.Extensions.DependencyInjection;
using MultiLanguage;
using MultiLanguage.Common;
using MultiLanguage.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace MultiLanguage.Test
{
    public class MultiLanguageCommonLanguageManagerTest
    {

        private ILanguageReceiveAdapter _adpter;
        private readonly string _hostingEnvironment;

        public MultiLanguageCommonLanguageManagerTest()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddLanguageReceiveAdapter();

            var provider = services.BuildServiceProvider();

            _adpter = provider.GetService<ILanguageReceiveAdapter>();

            DirectoryInfo rootDir = Directory.GetParent(Environment.CurrentDirectory);
            _hostingEnvironment =  rootDir.FullName;
        }
        /// <summary>
        /// 多语言测试，用户传入一个msgid和一个culture code,给出对应翻译
        /// </summary>
        [Fact]
        public void MultiLanguageTest()
        {   
            string webRootPath = _hostingEnvironment;
           
            var langUSFilePath = webRootPath + @"\en-US.mo";
            var langCNFilePath = webRootPath + @"\zh-CN.mo";

            string cultureCode = "en-US";
            string msgid = "仅自选时用户地址不能为空";

            List<string> parms = new List<string>();

            parms.Add("test1");
            parms.Add("test2");

            SingleLanguage langEn = new SingleLanguage("en-US", langUSFilePath, null);
            SingleLanguage langZh = new SingleLanguage("zh-CN", langCNFilePath, null);

            List<SingleLanguage> langs = new List<SingleLanguage>();

            langs.Add(langEn);
            langs.Add(langZh);

            _adpter.InitLanguageManager(langs);

            var manager = _adpter.GetLanguageManager();
            var result = manager.GetTranslate(cultureCode, msgid);

            msgid = "paramsErrorResult";


            result = manager.GetTranslate(cultureCode, msgid, parms);

            

            Assert.Equal(result.Result, "Incorrect Parameter 'test1'.");

            List<string> cultureCodes = new List<string>();
            List<string> Msgid = new List<string>();
            cultureCodes.Add("zh-CN");
            cultureCodes.Add("en-US");

            Msgid.Add("propertyManageName");
            Msgid.Add("propertyManageArr1");
            Msgid.Add("propertyManageArr2");


            var resultPartLoad = manager.GetLanguageShowDataParts(cultureCodes, Msgid, false);

            Assert.Equal(resultPartLoad.Count.ToString(), "2");
        }

        /// <summary>
        /// 在culturecode中使用自定义的别名
        /// </summary>
        [Fact]
        public void MultiLanguageAliasNameTest()
        {
            string webRootPath = _hostingEnvironment;

            var langUSFilePath = webRootPath + @"\en-US.mo";
            var langCNFilePath = webRootPath + @"\zh-CN.mo";

            string cultureCode = "en-US";
            string msgid = "仅自选时用户地址不能为空";

            

            List<string> aliasName = new List<string>();
            aliasName.Add("测试英语");
            aliasName.Add("我的英语");

            SingleLanguage langEn = new SingleLanguage("en-US", langUSFilePath, aliasName);
            SingleLanguage langZh = new SingleLanguage("zh-CN", langCNFilePath, null);

            List<SingleLanguage> langs = new List<SingleLanguage>();

            langs.Add(langEn);
            langs.Add(langZh);

            _adpter.InitLanguageManager(langs);

            string cultureCodeAlias = "我的英语";

            var manager = _adpter.GetLanguageManager();
            var result = manager.GetTranslate(cultureCodeAlias, msgid);

            Assert.Equal(result.Result, "Parameter 'userAddress' can not be null or whitespace when 'onlyUserMarked' is true.");
        }

        /// <summary>
        /// 测试，当起别名时，不小心将别名与之前的本名相同，
        /// 那么将会导致冲突的别名无法加入映射的字典表，而本名不受影响
        /// </summary>
        [Fact]
        public void MultiLanguageAliasNameIsRepeatTest()
        {
            string webRootPath = _hostingEnvironment;

            var langUSFilePath = webRootPath + @"\en-US.mo";
            var langCNFilePath = webRootPath + @"\zh-CN.mo";

            string cultureCode = "en-US";
            string msgid = "仅自选时用户地址不能为空";

          

            List<string> aliasName = new List<string>();
            aliasName.Add("测试英语");
            aliasName.Add("en-US");

            SingleLanguage langEn = new SingleLanguage("en-US", langUSFilePath, null);
            SingleLanguage langZh = new SingleLanguage("zh-CN", langCNFilePath, aliasName);

            List<SingleLanguage> langs = new List<SingleLanguage>();

            langs.Add(langEn);
            langs.Add(langZh);

            _adpter.InitLanguageManager(langs);

            string cultureCodeAlias = "en-US";

            var manager = _adpter.GetLanguageManager();
            var result = manager.GetTranslate(cultureCodeAlias, msgid);

            Assert.Equal(result.Result, "Parameter 'userAddress' can not be null or whitespace when 'onlyUserMarked' is true.");
        }
    }
}
