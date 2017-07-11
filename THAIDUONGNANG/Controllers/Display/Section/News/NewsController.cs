﻿using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THAIDUONGNANG.Models;
namespace THAIDUONGNANG.Controllers.Display.Section.News
{
    public class NewsController : Controller
    {
        //
        // GET: /News/
        THAIDUONGNANGContext db = new THAIDUONGNANGContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NewsDetail(string tag)
        {
            var tblnews = db.tblNews.First(p => p.Tag == tag);
            int id = int.Parse(tblnews.id.ToString());
            ViewBag.Title = "<title>" + tblnews.Title + "</title>";
            ViewBag.Description = "<meta name=\"description\" content=\"" + tblnews.Description + "\"/>";
            ViewBag.Keyword = "<meta name=\"keywords\" content=\"" + tblnews.Keyword + "\" /> ";
            ViewBag.dcTitle = "<meta name=\"DC.title\" content=\"" + tblnews.Title + "\" />";
            ViewBag.dcDescription = "<meta name=\"DC.description\" content=\"" + tblnews.Description + "\" />";
            int idCates = int.Parse(tblnews.idCate.ToString());

            string Urlgroup = db.tblGroupNews.Find(idCates).Tag;
            string chuoitag = "";
            if (tblnews.Tabs != null)
            {
                string Chuoi = tblnews.Tabs;
                string[] Mang = Chuoi.Split(',');

                List<int> araylist = new List<int>();
                for (int i = 0; i < Mang.Length; i++)
                {
                    chuoitag += "<a href=\"/TagNews/" + Mang[i] + "\" title=\"" + Mang[i] + "\">" + Mang[i] + "</a>";
                    string tabs = Mang[i].ToString();
                    var listnew = db.tblNews.Where(p => p.Tabs.Contains(tabs) && p.id != id && p.Active == true).ToList();
                    for (int j = 0; j < listnew.Count; j++)
                    {
                        araylist.Add(listnew[j].id);
                    }

                }
                ViewBag.chuoitag = chuoitag;

                var listnewlienquan = db.tblNews.Where(p => araylist.Contains(p.id) && p.Active == true && p.id != id).OrderByDescending(p => p.Ord).Take(3).ToList();
                string chuoinew = "";
                if (listnewlienquan.Count > 0)
                {

                    chuoinew += " <div class=\"Tintuclienquan\">";
                    for (int i = 0; i > listnewlienquan.Count; i++)
                    {
                        chuoinew += "<a href=\"/" + listnewlienquan[i].Tag + ".html\" title=\"" + listnewlienquan[i].Name + "\">› " + listnewlienquan[i].Name + "</a>";
                    }
                    chuoinew += "</div>";
                }
                ViewBag.chuoinew = chuoinew;


                //Load tin mới

            }
            int iduser = int.Parse(tblnews.idUser.ToString());
            var User = db.tblUsers.Find(iduser);
            ViewBag.UserName = User.FullName;
            string chuoinewnew = "";
            var NewsNew = db.tblNews.Where(p => p.Active == true && p.Tag!=tag).OrderByDescending(p => p.DateCreate).Take(5).ToList();
            for (int i = 0; i < NewsNew.Count; i++)
            {
                chuoinewnew += "<a href=\"/" + NewsNew[i].Tag + ".html\" title=\"" + NewsNew[i].Name + "\" rel=\"nofollow\"> " + NewsNew[i].Name + " </a>";
            }
            ViewBag.chuoinewnews = chuoinewnew;

            //load listnews
            var Groupnews = db.tblGroupNews.First(p => p.id == tblnews.idCate);
            ViewBag.menuname = Groupnews.Name;
            int dodai = Groupnews.Level.Length / 5;
            string nUrl = "";
            for (int i = 0; i < dodai; i++)
            {
                var NameGroups = db.tblGroupNews.First(p => p.Level.Substring(0, (i + 1) * 5) == Groupnews.Level.Substring(0, (i + 1) * 5));
                nUrl = nUrl + " <a href=\"/2/" + NameGroups.Tag + "\" title=\"\"> " + " " + NameGroups.Name + "</a> /";
            }
            ViewBag.nUrl = "<a href=\"/\" title=\"Trang chủ\" rel=\"nofollow\"><span class=\"iCon\"></span> Trang chủ</a> /" + nUrl + " " + tblnews.Name;
            tblConfig tblconfig = db.tblConfigs.First();
            if(tblconfig.Coppy==true)
            {
                ViewBag.Coppy = " <script src=\"/Scripts/disable-copyright.js\"></script> <link href=\"/Content/Display/Css/Coppy.css\" rel=\"stylesheet\" /> ";   
            }
            return View(tblnews);
        }
        public ActionResult ListNews(string tag, int? page)
        {

            int idCate = int.Parse(db.tblGroupNews.First(p => p.Tag == tag).id.ToString());
            var listnews = db.tblNews.Where(p => p.idCate == idCate && p.Active == true).OrderByDescending(p => p.Ord).ToList();
            const int pageSize = 20;
            var pageNumber = (page ?? 1);
            // Thiết lập phân trang
            var ship = new PagedListRenderOptions
            {
                DisplayLinkToFirstPage = PagedListDisplayMode.Always,
                DisplayLinkToLastPage = PagedListDisplayMode.Always,
                DisplayLinkToPreviousPage = PagedListDisplayMode.Always,
                DisplayLinkToNextPage = PagedListDisplayMode.Always,
                DisplayLinkToIndividualPages = true,
                DisplayPageCountAndCurrentLocation = false,
                MaximumPageNumbersToDisplay = 5,
                DisplayEllipsesWhenNotShowingAllPageNumbers = true,
                EllipsesFormat = "&#8230;",
                LinkToFirstPageFormat = "Trang đầu",
                LinkToPreviousPageFormat = "«",
                LinkToIndividualPageFormat = "{0}",
                LinkToNextPageFormat = "»",
                LinkToLastPageFormat = "Trang cuối",
                PageCountAndCurrentLocationFormat = "Page {0} of {1}.",
                ItemSliceAndTotalFormat = "Showing items {0} through {1} of {2}.",
                FunctionToDisplayEachPageNumber = null,
                ClassToApplyToFirstListItemInPager = null,
                ClassToApplyToLastListItemInPager = null,
                ContainerDivClasses = new[] { "pagination-container" },
                UlElementClasses = new[] { "pagination" },
                LiElementClasses = Enumerable.Empty<string>()
            };
            ViewBag.ship = ship;

            var groupnew = db.tblGroupNews.First(p => p.Tag == tag);
            int dodai = groupnew.Level.Length / 5;
            string nUrl = "";
            for (int i = 0; i < dodai; i++)
            {
                var NameGroups = db.tblGroupNews.First(p => p.Level.Substring(0, (i + 1) * 5) == groupnew.Level.Substring(0, (i + 1) * 5) && p.Level.Length == (i + 1) * 5);
                nUrl = nUrl + " <a href=\"/2/" + NameGroups.Tag + "\" title=\"" + NameGroups.Name + "\"> " + " " + NameGroups.Name + "</a> /";
            }
            ViewBag.nUrl = "<a href=\"/\" title=\"Trang chủ\" rel=\"nofollow\"><span class=\"iCon\"></span> Trang chủ</a> /" + nUrl;
            ViewBag.Title = "<title>" + groupnew.Title + "</title>";
            ViewBag.Description = "<meta name=\"description\" content=\"" + groupnew.Description + "\"/>";
            ViewBag.Keyword = "<meta name=\"keywords\" content=\"" + groupnew.Keyword + "\" /> ";
            ViewBag.Name = groupnew.Name;
            return View(listnews.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult TagNews(string tag, int? page)
        {

             var listnews = db.tblNews.Where(p => p.Tabs.Contains(tag) && p.Active == true).OrderByDescending(p => p.Ord).ToList();
            const int pageSize = 20;
            var pageNumber = (page ?? 1);
            // Thiết lập phân trang
            var ship = new PagedListRenderOptions
            {
                DisplayLinkToFirstPage = PagedListDisplayMode.Always,
                DisplayLinkToLastPage = PagedListDisplayMode.Always,
                DisplayLinkToPreviousPage = PagedListDisplayMode.Always,
                DisplayLinkToNextPage = PagedListDisplayMode.Always,
                DisplayLinkToIndividualPages = true,
                DisplayPageCountAndCurrentLocation = false,
                MaximumPageNumbersToDisplay = 5,
                DisplayEllipsesWhenNotShowingAllPageNumbers = true,
                EllipsesFormat = "&#8230;",
                LinkToFirstPageFormat = "Trang đầu",
                LinkToPreviousPageFormat = "«",
                LinkToIndividualPageFormat = "{0}",
                LinkToNextPageFormat = "»",
                LinkToLastPageFormat = "Trang cuối",
                PageCountAndCurrentLocationFormat = "Page {0} of {1}.",
                ItemSliceAndTotalFormat = "Showing items {0} through {1} of {2}.",
                FunctionToDisplayEachPageNumber = null,
                ClassToApplyToFirstListItemInPager = null,
                ClassToApplyToLastListItemInPager = null,
                ContainerDivClasses = new[] { "pagination-container" },
                UlElementClasses = new[] { "pagination" },
                LiElementClasses = Enumerable.Empty<string>()
            };
            ViewBag.ship = ship;


            ViewBag.nUrl = "<a href=\"/\" title=\"Trang chủ\" rel=\"nofollow\"><span class=\"iCon\"></span> Trang chủ</a>/Tag";
            ViewBag.Title = "<title>" + tag+ "</title>";
            ViewBag.Description = "<meta name=\"description\" content=\"" + tag + "\"/>";
            ViewBag.Keyword = "<meta name=\"keywords\" content=\"" + tag + "\" /> ";
            ViewBag.Name = tag;
            return View(listnews.ToPagedList(pageNumber, pageSize));
        }
        public PartialViewResult RightNews()
        {
            return PartialView();
        }
    }
}
