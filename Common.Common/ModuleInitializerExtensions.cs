using Commons.Commons;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Common
{
    public static class ModuleInitializerExtensions
    {
        /// <summary>
        /// 自动化服务注册的过程，通过扫描一组程序集来查找实现了IModuleInitializer接口的所有类，并调用它们的初始化方法
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public static IServiceCollection RunModuleInitializers(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                var moduleInitializerTypes = types.Where(t => !t.IsAbstract && typeof(IModuleInitializer).IsAssignableFrom(t));
                foreach (var type in moduleInitializerTypes)
                {
                    var initializer = (IModuleInitializer)Activator.CreateInstance(type);

                    if (initializer == null) 
                    {
                        throw new ApplicationException($"Cannot create ${type}");
                    }
                    initializer.Initialize(services);
                }
            }
            return services;
        }

        
    }
}
