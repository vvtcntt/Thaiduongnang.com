using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THAIDUONGNANG.Models;
namespace THAIDUONGNANG.Controllers.Display.Header
{
    public class HeaderController : Controller
    {
        //
        // GET: /Header/
        THAIDUONGNANGContext db = new THAIDUONGNANGContext();
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult HeaderPartial()
        {
            string menu = "";
            var listMenu = db.tblGroupProducts.Where(p => p.Active == true && p.Level.Length == 5).OrderBy(p => p.Ord).ToList();

            for (int i = 0; i < listMenu.Count; i++)
            {
                menu += "<li>";
                menu += "<a href=\"/0/" + listMenu[i].Tag + "\" title=\"" + listMenu[i].Name + "\">" + listMenu[i].Name + "</a>";
                string level = listMenu[i].Level;
                var MenuChild = db.tblGroupProducts.Where(p => p.Active == true && p.Level.Substring(0, 5) == level && p.Level.Length == 10).ToList();
                if (MenuChild.Count > 0)
                {
                    menu += "<ul class=\"ul_3\">";
                    for (int j = 0; j < MenuChild.Count; j++)
                    {
                        menu += "<li><a href=\"/0/" + MenuChild[j].Tag + "\" title=\"" + MenuChild[j].Name + "\">" + MenuChild[j].Name + "</a></li>";
                    }
                    menu += "</ul>";
                }
                menu += "</li>";

            }
            ViewBag.menu = menu;
            return PartialView(db.tblConfigs.First());
        }
        public PartialViewResult SlidePartial()
        {
            var listSplide = db.tblImages.Where(p => p.Active == true && p.idMenu == 1).OrderByDescending(p => p.Ord).ToList();
            string chuoi1 = "";
            string chuoi2 = "";
            for (int i = 0; i < listSplide.Count; i++)
            {
                if (i == 0)
                {
                    chuoi2 += " <li data-target=\"#myCarousel\" data-slide-to=\"" + i + "\" class=\"active\"></li>";
                    chuoi1 += "<div class=\"item active\">";
                }
                else
                {
                    chuoi2 += " <li data-target=\"#myCarousel\" data-slide-to=\"" + i + "\"></li>";
                    chuoi1 += "<div class=\"item\">";
                }
                chuoi1 += "<div class=\"fill\" style=\" background:url(" + listSplide[i].Images + ");background-size:100%\"></div>";
                chuoi1 += "<div class=\"carousel-caption\">";
                 chuoi1 += "</div>";
                chuoi1 += "</div>";

            }
            ViewBag.chuoi1 = chuoi1;
            ViewBag.chuoi2 = chuoi2;
            return PartialView();
        }
    }
}
