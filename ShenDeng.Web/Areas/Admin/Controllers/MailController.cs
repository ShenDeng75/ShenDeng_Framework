using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShenDeng.Framework.Application;
using ShenDeng.Framework.Handle;
using ShenDeng.Framework.Domain;
using ShenDeng.Framework.Tools;
using System.IO;
using ShenDeng.Web.Help;

namespace ShenDeng.Web.Areas.Admin.Controllers
{
    // *邮箱管理
    [FilterAuthority(Role.Admin)]
    public class MailController : Controller
    {
        private MailServer mailServer;
        public MailController()
        {
            mailServer = new MailServer();
        }
        // 显示所有
        public ActionResult Index()
        {
            var mails = mailServer.GetAll();
            return View(mails);
        }
        // 添加
        public JsonResult CreateMail()
        {
            try
            {
                var datas = tool.Deserialize<Dictionary<string, string>>(Request.InputStream);

                mailServer.CreateMail(datas[key.name], datas[key.mailaddre], datas[key.duty], datas[key.mailpwd]);
                var result = new { Result = "成功" };
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch(Exception err)
            {
                var result = new { Result = err.Message };
                return Json(result, JsonRequestBehavior.DenyGet);
            }
        }
        // 删除
        public ActionResult Delete(string dbid)
        {
            mailServer.DeleteMail(dbid);
            return RedirectToAction("Index");
        }
    }
}