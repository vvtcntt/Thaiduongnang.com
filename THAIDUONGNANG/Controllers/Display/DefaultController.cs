using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THAIDUONGNANG.Models;
namespace THAIDUONGNANG.Controllers.Display
{
    public class DefaultController : Controller
    {
        //
        // GET: /Default/
        THAIDUONGNANGContext db = new THAIDUONGNANGContext();
        public ActionResult Index()
        {
            tblConfig tblconfig = db.tblConfigs.First();
            ViewBag.Title = "<title>" + tblconfig.Title + "</title>";
            ViewBag.dcTitle = "<meta name=\"DC.title\" content=\"" + tblconfig.Title + "\" />";
            ViewBag.dcDescription = "<meta name=\"DC.description\" content=\"" + tblconfig.Description + "\" />";
            ViewBag.Description = "<meta name=\"description\" content=\"" + tblconfig.Description + "\"/>";
            ViewBag.Keyword = "<meta name=\"keywords\" content=\"" + tblconfig.Keywords + "\" /> ";
            ViewBag.h1="<h1 class=\"h1\">"+tblconfig.Title+"</h1>";
            return View();
        }
        public PartialViewResult NewsHomes()
        {
            var listNews = db.tblNews.Where(p => p.Active == true && p.ViewHomes == true).OrderByDescending(p => p.DateCreate).Take(3).ToList();
            string chuoi = "";
            for (int i = 0; i < listNews.Count; i++)
            {
                chuoi += "<div class=\"Tear_NewsHomes\">";
                chuoi += "<img src=\"" + listNews[i].ImageLinkThumb + "\" alt=\"" + listNews[i].Name + "\" />";
                chuoi += "<a href=\"/" + listNews[i].Tag + ".html\" title=\"" + listNews[i].Name + "\">" + listNews[i].Name + "</a>";
                chuoi += "<span class=\"Time\">Ngày cập nhật : Ngày " + DateTime.Parse(listNews[i].DateCreate.ToString()).Day + " tháng " + DateTime.Parse(listNews[i].DateCreate.ToString()).Month + " năm " + DateTime.Parse(listNews[i].DateCreate.ToString()).Year + "</span>";
                chuoi += "</div>";
            }
            ViewBag.chuoi = chuoi;
            string chuoiurl = "";
            for (int i = 1; i < 5; i++)
            {
                chuoiurl += " <li class=\"li1\">";
                if (i == 1)
                    chuoiurl += "<a href=\"#\" title=\"Bồn nước sơn hà\">Bồn nước sơn hà</a>";
                if (i == 2)
                    chuoiurl += "<a href=\"#\" title=\"Thái dương năng\">Thái dương năng</a>";
                if (i == 3)
                    chuoiurl += "<a href=\"#\" title=\"Máy lọc nước sơn hà\">Máy lọc nước Sơn Hà</a>";
                if (i == 4)
                    chuoiurl += "<a href=\"#\" title=\"Chậu rửa sơn hà\">Chậu rửa Sơn Hà</a>";
                chuoiurl += "<ul class=\"ul2\">";
                var listurrl = db.tblUrls.Where(p => p.Active == true && p.idCate == i).OrderBy(p => p.Ord).ToList();
                if (listurrl.Count > 0)
                {
                    for (int j = 0; j < listurrl.Count; j++)
                    {
                        chuoiurl += "  <li class=\"li2\">";
                        chuoiurl += " <a href=\"http://" + listurrl[j].Url + "\" title=\"" + listurrl[j].Name + "\" rel=\"nofollow\">" + listurrl[j].Name + "</a>";
                        chuoiurl += " </li>";
                    }
                }
                chuoiurl += "  </ul>";
                chuoiurl += "  </li>";
            }
            ViewBag.chuoiurl = chuoiurl;
            ViewBag.codevideo = db.tblVideos.First().Code;
            
            return PartialView();
        }
        public PartialViewResult MenuProductHomes()
        {
            string chuoi = "";
            var listMenu = db.tblGroupProducts.Where(p => p.Active == true && p.Level.Length == 5 && p.Index == true).OrderBy(p => p.Ord).ToList();
            for (int i = 0; i < listMenu.Count; i++)
            {
                chuoi += "<div class=\"Tear_Mn\">";
                string[] aray = listMenu[i].Name.Split(' ');
                int dodai = aray.Length;
                string name = "";
                for (int k = 0; k < dodai-1;k++ )
                {
                    name += aray[k] +" ";
                }
                chuoi += " <h2>" + name + " <span>" + aray[dodai - 1] + "</span></h2>";
                chuoi += " <div class=\"Content_Tear_Mn\">";
                chuoi += "<a href=\"/0/" + listMenu[i].Tag + "\" title=\"" + listMenu[i].Name + "\"><img src=\"" + listMenu[i].Images + "\" alt=\"" + listMenu[i].Name + "\" /></a>";
                chuoi += " </div>";
                chuoi += "<div class=\"Clear\"></div>";
                chuoi += " <div class=\"Infos\">";
                chuoi += " <div class=\"TopInfo\"></div>";
                chuoi += " <div class=\"Content_Info\">";
                chuoi += " <span class=\"ud\">Ưu điểm</span>";
                chuoi += " <div class=\"ct\">" + listMenu[i].Info + " </div>";
                chuoi += " </div>";
                chuoi += "</div>";
                chuoi += " </div>";
            }
            ViewBag.chuoi = chuoi;
            return PartialView();
        }
        public PartialViewResult AdwSale()
        {
            var listAdw = db.tblImages.Where(p => p.idMenu == 6 && p.Active==true).OrderByDescending(p => p.Ord).Take(1).ToList();
            string chuoi = "";
            chuoi += "<a href=\"" + listAdw[0].Url + "\" title=\"" + listAdw[0].Name + "\">";
            chuoi += "<img src=\"" + listAdw[0].Images + "\" alt=\"" + listAdw[0].Name + "\" title=\"" + listAdw[0].Name + "\"/>";
   chuoi+=" </a>";
   ViewBag.chuoi = chuoi;
            return PartialView();
        }
        public PartialViewResult TuvanPartial()
        {
          
            return PartialView();
        }
        public ActionResult Tuvan(FormCollection collection)
        {
            Session["txtOrd1"] = collection["txtOrd1"];
            Session["txtOrd2"] = collection["txtOrd2"];
            return Redirect("/Product/Tuvan");
        }
    }
}
