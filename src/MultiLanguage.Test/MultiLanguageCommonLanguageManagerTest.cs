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
        /// �����Բ��ԣ��û�����һ��msgid��һ��culture code,������Ӧ����
        /// </summary>
        [Fact]
        public void MultiLanguageTest()
        {   
            string webRootPath = _hostingEnvironment;
           
            var langUSFilePath = webRootPath + @"\en-US.mo";
            var langCNFilePath = webRootPath + @"\zh-CN.mo";

            string cultureCode = "en-US";
            string msgid = "����ѡʱ�û���ַ����Ϊ��";

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
        /// ��culturecode��ʹ���Զ���ı���
        /// </summary>
        [Fact]
        public void MultiLanguageAliasNameTest()
        {
            string webRootPath = _hostingEnvironment;

            var langUSFilePath = webRootPath + @"\en-US.mo";
            var langCNFilePath = webRootPath + @"\zh-CN.mo";

            string cultureCode = "en-US";
            string msgid = "����ѡʱ�û���ַ����Ϊ��";

            

            List<string> aliasName = new List<string>();
            aliasName.Add("����Ӣ��");
            aliasName.Add("�ҵ�Ӣ��");

            SingleLanguage langEn = new SingleLanguage("en-US", langUSFilePath, aliasName);
            SingleLanguage langZh = new SingleLanguage("zh-CN", langCNFilePath, null);

            List<SingleLanguage> langs = new List<SingleLanguage>();

            langs.Add(langEn);
            langs.Add(langZh);

            _adpter.InitLanguageManager(langs);

            string cultureCodeAlias = "�ҵ�Ӣ��";

            var manager = _adpter.GetLanguageManager();
            var result = manager.GetTranslate(cultureCodeAlias, msgid);

            Assert.Equal(result.Result, "Parameter 'userAddress' can not be null or whitespace when 'onlyUserMarked' is true.");
        }

        /// <summary>
        /// ���ԣ��������ʱ����С�Ľ�������֮ǰ�ı�����ͬ��
        /// ��ô���ᵼ�³�ͻ�ı����޷�����ӳ����ֵ������������Ӱ��
        /// </summary>
        [Fact]
        public void MultiLanguageAliasNameIsRepeatTest()
        {
            string webRootPath = _hostingEnvironment;

            var langUSFilePath = webRootPath + @"\en-US.mo";
            var langCNFilePath = webRootPath + @"\zh-CN.mo";

            string cultureCode = "en-US";
            string msgid = "����ѡʱ�û���ַ����Ϊ��";

          

            List<string> aliasName = new List<string>();
            aliasName.Add("����Ӣ��");
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
