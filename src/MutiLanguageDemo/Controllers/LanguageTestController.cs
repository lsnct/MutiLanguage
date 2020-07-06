using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace MutiLanguageDemo.Controllers
{
    public class LanguageTestController : BaseController
    {

        public LanguageTestController(IServiceProvider serviceProvider, IHostingEnvironment hostingEnvironment) : base(serviceProvider, hostingEnvironment)
        {

        }

        /// <summary>
        /// 本页面主要测试加载全部语言到客户端并形成翻译（利用了GetAllListTranslateResult方法和客户端cookies）
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }


    }
}