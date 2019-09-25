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
			container = new UnityContainer();   //ʵ��������
            var assembly = new List<string>();    //���س���
            assembly.Add("ShenDeng.Framework");
           // assembly.Add("ShenDeng");
            foreach (var ass in assembly)   //�������Խ����Զ�ע��
            {
                RegisterType(ass);
            }
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));  //?  ���Լ��ӵ�
        }

        public static void RegisterType(string assembly)
        {
            var types = Assembly.Load(assembly).GetTypes();   //�õ���������
            foreach (var type in types)
            {
                var attributes = type.GetCustomAttributes();   //�õ������Զ�������
                foreach (var attribute in attributes)
                {
                    if (attribute is RegisterToContainerAttribute)
                    {
                        var Itypes = type.GetInterfaces();    //�õ����нӿ�
                        var Itype = Itypes.Single();   //���Ψһ�ӿڣ��������Ψһ�ľͻᱨ��
                        container.RegisterType(Itype, type);   //ע������
                    }
                }
            }
        }
        //ֱ��ͨ���ӿ����ͻ�ȡʵ��
        public static T  Get<T>()
        {
            try
            {
                return container.Resolve<T>();
            }
            catch (Exception e)
            {
                throw new Exception(typeof(T)+"ע������!\n"+e.Message);
            }
            
        }
    }
}