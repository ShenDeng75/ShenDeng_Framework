using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using ShenDeng.Framework.Application;
using ShenDeng.Framework.Application.Imp;
using ShenDeng.Framework.App_Start;
using Unity;
using Unity.Mvc5;

namespace ShenDeng.Framework
{
    public static class UnityIoC
    {
        private static IUnityContainer container { get; set; }
        public static void RegisterComponents()
        {
			container = new UnityContainer();   //实例化容器
            var assembly = new List<string>();    //加载程序集
            assembly.Add("ShenDeng.Framework");
           // assembly.Add("ShenDeng");
            foreach (var ass in assembly)   //根据特性进行自动注册
            {
                RegisterType(ass);
            }
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));  //?  他自己加的
        }

        public static void RegisterType(string assembly)
        {
            var types = Assembly.Load(assembly).GetTypes();   //得到所有类型
            foreach (var type in types)
            {
                var attributes = type.GetCustomAttributes();   //得到所有自定义特性
                foreach (var attribute in attributes)
                {
                    if (attribute is RegisterToContainerAttribute)
                    {
                        var Itypes = type.GetInterfaces();    //得到所有接口
                        var Itype = Itypes.Single();   //获得唯一接口，如果不是唯一的就会报错
                        container.RegisterType(Itype, type);   //注册类型
                    }
                }
            }
        }
        //直接通过接口类型获取实例
        public static T  Get<T>()
        {
            try
            {
                return container.Resolve<T>();
            }
            catch (Exception e)
            {
                throw new Exception(typeof(T)+"注册有误!\n"+e.Message);
            }
            
        }
    }
}