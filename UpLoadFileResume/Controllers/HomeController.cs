using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;

namespace Hs_UpLoad.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            if (Session["user"] == null)
            { 
                //去地址检测
                
                //检测后依旧未登录
            }
            return View();
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            //
            string HostURL = Request.Url.ToString();
            Regex regex = new Regex("/" + RouteData.Route.GetRouteData(this.HttpContext).Values["controller"].ToString() + "/" + RouteData.Route.GetRouteData(this.HttpContext).Values["action"].ToString());//分割字符串
            HostURL = regex.Split(HostURL)[0];

            ViewBag.URLHost = HostURL;

           //string c1 = RouteData.Route.GetVirtualPath().ToString();

            ViewBag.url = Request.Url.Host;
            ViewBag.url1 = Request.Url.ToString();
            string SSOURL = System.Configuration.ConfigurationManager.AppSettings["SSOUrl"].ToString();
            string a = Request.Url.PathAndQuery + SSOURL;
            return View();
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <returns></returns> 
        public JsonResult plupload(string name)
        {
            string[] dd = Request.Headers.AllKeys;
            string msg = string.Empty;
            int chunk = Convert.ToInt32(Request["chunk"]); //当前分块
            int chunks = Convert.ToInt32(Request["chunks"]);//总的分块数量
            long hcouns = 0;
            foreach (string upload in Request.Files)
            {
                if (upload != null && upload.Trim() != "")
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "Temp\\";
                    if (!Directory.Exists(path))    //判断给定的路径上是否存在该目录
                    {
                        Directory.CreateDirectory(path);    //不存在则创建该目录
                    }
                    System.Web.HttpPostedFileBase postedFile = Request.Files[upload];   //获取客户端上载文件的集合
                    string filename1 = Path.GetFileName(postedFile.FileName);   //获取客户端上传文件的名称及后缀
                    string filename = name; //

                    string newFileName = filename;
                    if (chunks > 1)
                    {
                        newFileName = chunk + "_" + filename;   //按文件块重命名块文件
                    }
                    string fileNamePath = path + newFileName;   //将块文件和临时文件夹路径绑定

                    if (chunks > 0)
                    {
                        for (int i = 0; i < chunks; i++)
                        {
                            //检测已存在磁盘的文件区块
                            if (!System.IO.File.Exists(path+i.ToString() + "_" + filename) && i != chunk)
                            {
                                hcouns = i * postedFile.ContentLength;
                                break;  
                            }
                        }
                    }
                    postedFile.SaveAs(fileNamePath);    //保存上载文件内容

                    if (chunks > 1 && chunk + 1 == chunks)    //判断块总数大于1 并且当前分块+1==块总数(指示是否为最后一个分块)
                    {
                        using (FileStream fsw = new FileStream(path + filename, FileMode.Create, FileAccess.Write))
                        {
                            BinaryWriter bw = new BinaryWriter(fsw);
                            // 遍历文件合并 
                            for (int i = 0; i < chunks; i++)
                            {
                                bw.Write(System.IO.File.ReadAllBytes(path + i.ToString() + "_" + filename));    //打开一个文件读取流信息，将其写入新文件
                                System.IO.File.Delete(path + i.ToString() + "_" + filename);        //删除指定文件信息
                                bw.Flush(); //清理缓冲区
                            }
                        }

                    }

                }
            }
            return Json(new { jsonrpc = "2.0", result = "", id = "id", hcount = "" + hcouns.ToString() + "" });

        }
        [HttpPost]
        public JsonResult checkplupload()
        {
            string fileName = Request["fileName"].ToString();
            //文件名称
            long Size = Request["size"] == null ? 0 : Convert.ToInt64(Request["size"]);
            //文件的分块大小
            int fileCount = Request["maxFileCount"] == null ? 0 : Convert.ToInt32(Request["maxFileCount"]);
            //文件一共的分块数量。比如1G以20M分块，则有50块。

            //上面变量通过加载文件的时候通过ajax方式提交过来
            long hcouns = 0;
            string path = AppDomain.CurrentDomain.BaseDirectory + "Temp\\";
            if (fileCount > 0)
            {
                for (int i = 0; i < fileCount; i++)
                {
                    //检测已存在磁盘的文件区块
                    if (!System.IO.File.Exists(path + i.ToString() + "_" + fileName))
                    {
                        //你懂的，如果服务器上不存在如 i_文件名这个文件，那证明客户端应该从这个字节开始往服务器上传。
                        hcouns = i * Size;
                        //服务器已存在区块的总字节数
                        break;
                    }
                }
            }
            //返回到客户端
            return Json(new { result = hcouns });
        }
    }
}
