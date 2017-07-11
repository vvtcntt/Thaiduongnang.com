using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THAIDUONGNANG.Models;
namespace THAIDUONGNANG.Controllers.Display.Footer
{
    public class FooterController : Controller
    {
        //
        // GET: /Footer/
        THAIDUONGNANGContext db = new THAIDUONGNANGContext();
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult FooterPartial()
        {
            var tblmap = db.tblMaps.First();
            ViewBag.maps = tblmap.Content;
            var listhotline = db.tblHotlines.Where(p => p.Active == true).OrderBy(p => p.Ord).ToList();
            string chuoi = "";
            for(int i = 0; i < listhotline.Count; i++)
            {
                chuoi += "<div class=\"Tear_ft_1\">";
                chuoi += "<div class=\"nVar_Tear_ft_1\">";
                if(listhotline[i].Type==1)
                { 
                chuoi += "<span class=\"name_ft\">"+listhotline[i].Name+" <span>(Chi nhánh)</span></span>";
                }
                else
                    chuoi += "<span class=\"name_ft\">" + listhotline[i].Name + " <span>(Trụ sở chính)</span></span>";
                chuoi += " </div>";
                chuoi += " <div class=\"Content_Tear_ft_1\">";
                chuoi += "  <span class=\"ct_ft\">Địa chỉ : <span>" + listhotline[i].Address + " </span></span>";
                chuoi += " <span class=\"ct_ft\">Điện thoại : <span>" + listhotline[i].Mobile + " </span>    </span>";
                chuoi += "  <span class=\"ct_ft hotline\">Hotline :<span>" + listhotline[i].Hotline + " </span></span>";
                chuoi += " <span class=\"ct_ft\">Email: <span>" + listhotline[i].Email + " </span></span>";
                chuoi += " <span class=\"Map_ft\"><span class=\"icon\"></span> Bản đồ đường đi</span>";
                chuoi += " </div>";
                chuoi += "  </div>";

            }
            ViewBag.chuoi = chuoi;
            string chuoipd = "";
            var listGroup = db.tblGroupProducts.Where(p => p.Index == true && p.Active == true).OrderBy(p => p.Ord).ToList();
            for (int i = 0; i < listGroup.Count;i++ )
            {
                chuoipd += "<a href=\"/0/" + listGroup[i].Tag + "\" title=\"" + listGroup[i].Name + "\">" + listGroup[i].Name + "</a>";
            }
            ViewBag.chuoipd = chuoipd;

            //load báo giá
            string baogia = "";
            var listbaogia = db.tblGroupProducts.Where(p => p.Active == true && p.Baogia == true).OrderBy(p => p.Ord).Take(5).ToList();
            for (int i = 0; i < listbaogia.Count;i++ )
            {
                baogia += "<a href=\"/"+listbaogia[i].Tag+"-bao-gia.html\" title=\"Báo giá " + listbaogia[i].Name + "\">Báo giá " + listbaogia[i].Name + "</a>";
            }
            ViewBag.baogia = baogia;
            var ListUrl = db.tblUrls.Where(p => p.Active == true).OrderBy(p => p.Ord).ToList();
            string chuoiurl = "";
            for (int i = 0; i < ListUrl.Count;i++ )
            {
                chuoiurl += "<a href=\"" + ListUrl[i].Url + "\" title=\"" + ListUrl[i].Name + "\">" + ListUrl[i].Name + "</a>";
            }
            ViewBag.chuoiurl = chuoiurl;
                return PartialView(db.tblConfigs.First());
        }
    }
}
