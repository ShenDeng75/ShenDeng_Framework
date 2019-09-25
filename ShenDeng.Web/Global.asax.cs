using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ShenDeng.Framework;

namespace ShenDeng.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            UnityIoC.RegisterComponents();     //依赖注入
            AreaRegistration.RegisterAllAreas();    //注册区域路由
            RouteConfig.RegisterRoutes(RouteTable.Routes);    //注册全局路由
        }
    }
}
