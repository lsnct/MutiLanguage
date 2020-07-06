using Microsoft.Extensions.DependencyInjection;
using MultiLanguage.Common;
using MultiLanguage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 扩展方法，用于创建services
    /// </summary>
    public static class LanguageReceiveAdapterProviderServiceExtensions
    {
        /// <summary>
        /// 添加DI容器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddLanguageReceiveAdapter(this IServiceCollection services)
        {
            services.AddSingleton<ILanguageReceiveAdapter, LanguageReceiveAdapter>();



            return services;
        } 
    }
}
