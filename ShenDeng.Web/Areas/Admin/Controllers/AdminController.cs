using System.Web.Mvc;
using ShenDeng.Framework.Application;
using ShenDeng.Framework.Base;
using ShenDeng.Framework.Domain;
using ShenDeng.Framework.Handle;
using ShenDeng.Framework.Tools;
using System.IO;
using System.Collections.Generic;
using ShenDeng.Web.Help;
using System;
using System.Collections;
using ShenDeng.Framework;
using System.Linq;

namespace ShenDeng.Web.Areas.Admin.Controllers
{
    [FilterAuthority(Role.Admin)]
    public class AdminController : Controller
    {
        public readonly IAccountService accountService;
        public AdminController(IAccountService accountService)
        {
            this.accountService = accountService;
        }
        #region *账户管理*
        //显示账户
        public ActionResult ManageAccount()
        {
            var accounts = accountService.GetAllAccount();
            return View(accounts);
        }
        // 添加账户
        public JsonResult CreateAccount()
        {
            try
            {
                var datas = tool.Deserialize<Dictionary<string, object>>(Request.InputStream);
                accountService.CreateAccount(datas[key.username].ToString(), datas[key.jobnumber].ToString())
                    .SetPassWord("1")
                    .SetCanDelete(true);
                Account account = accountService.GetOneAccount(AccountIdentifier.of(datas[key.username].ToString()));
                List<string> roles = new List<string>((string[])((ArrayList)datas["roles"]).ToArray(typeof(string)));
                for (var i = 0; i < roles.Count; i++)
                {
                    int role = (int)Enum.Parse(typeof(Role), roles[i]);
                    account.AddRole(role);
                }
                accountService.Commit(); // 一个方法只能Commit一次

                var result = new{ Result = "成功" };
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch(Exception err)
            {
                var result = new{ Result = err.Message };
                return Json(result, JsonRequestBehavior.DenyGet);
            }
        }
        //删除账户
        public ActionResult Delete_Account(string id)
        {
            accountService.Delete(AccountIdentifier.of(id));
            return RedirectToAction("ManageAccount");
        }
        // 重置密码
        public JsonResult ResetPassword(string username)
        {
            try
            {
                accountService.ResetPassword(username);
                var result = new { Result = "成功" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch(Exception err)
            {
                var result = new { Result = err.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}