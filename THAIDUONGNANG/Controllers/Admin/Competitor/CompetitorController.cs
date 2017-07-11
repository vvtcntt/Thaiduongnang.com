using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THAIDUONGNANG.Models;
using System.Data;
using PagedList;
using PagedList.Mvc;
using System.Globalization;
using System.Data.Entity;
namespace THAIDUONGNANG.Controllers.Admin.Competitor
{
    public class CompetitorController : Controller
    {
        //
        // GET: /Competitor/
        private THAIDUONGNANGContext db = new THAIDUONGNANGContext();
        public ActionResult Index(int? page, string id)
        {
            if ((Request.Cookies["Username"] == null))
            {
                return RedirectToAction("LoginIndex", "Login");
            }
            var ListCompetitor = db.tblCompetitors.ToList();

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
            return View(ListCompetitor.ToPagedList(pageNumber, pageSize));
         }
        public ActionResult Create()
        {
            if ((Request.Cookies["Username"] == null))
            {
                return RedirectToAction("LoginIndex", "Login");
            }
            var pro = db.tblCompetitors.OrderByDescending(p => p.Ord).Take(1).ToList();
            if (pro.Count > 0)
                ViewBag.Ord = pro[0].Ord + 1;
            return View();
        }

        //
        // POST: /Url/Create

        [HttpPost]
        public ActionResult Create(tblCompetitor tblcompetitor)
        {
            tblcompetitor.DateCreate = DateTime.Now;
            string idUser = Request.Cookies["Username"].Values["UserID"];
            tblcompetitor.idUser = int.Parse(idUser);
            db.tblCompetitors.Add(tblcompetitor);
            //db.tblUrls.Add(tblurl);
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Add tblcompetitor", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            return RedirectToAction("Index");
        }

        //
        // GET: /Url/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if ((Request.Cookies["Username"] == null))
            {
                return RedirectToAction("LoginIndex", "Login");
            }
            tblCompetitor tblcompetitor = db.tblCompetitors.Find(id);
            if (tblcompetitor == null)
            {
                return HttpNotFound();
            }
            return View(tblcompetitor);
        }

        //
        // POST: /Url/Edit/5

        [HttpPost]
        public ActionResult Edit(tblCompetitor tblcompetitor)
        {
            if (ModelState.IsValid)
            {
                tblcompetitor.DateCreate = DateTime.Now;

                string idUser = Request.Cookies["Username"].Values["UserID"];
                tblcompetitor.idUser = int.Parse(idUser);
                db.Entry(tblcompetitor).State = EntityState.Modified;
                db.SaveChanges();
                #region[Updatehistory]
                Updatehistoty.UpdateHistory("Edit tblcompetitor", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
                #endregion
                return RedirectToAction("Index");
            }
            return View(tblcompetitor);
        }

        //
        // GET: /Url/Delete/5

        public ActionResult Delete(int id = 0)
        {
            tblCompetitor tblcompetitor = db.tblCompetitors.Find(id);
            if (tblcompetitor == null)
            {
                return HttpNotFound();
            }
            return View(tblcompetitor);
        }

        //
        // POST: /Url/Delete/5


        public ActionResult DeleteConfirmed(int id)
        {
            tblCompetitor tblcompetitor = db.tblCompetitors.Find(id);
            db.tblCompetitors.Remove(tblcompetitor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public ActionResult CompetitorEditOrd(int txtSort, string ts)
        {
            var tblcompetitor = db.tblCompetitors.Find(txtSort);
            var result = string.Empty;
            tblcompetitor.Ord = int.Parse(ts);
            //db.Entry(MenuGroupsProduct).State = System.Data.EntityState.Modified;
            result = "Update Ord.";
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Update Ord tblcompetitor", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            return Json(new { result = result });
        }
        [HttpPost]
        public ActionResult CompertitorEditActive(string chk, string nchecked)
        {

            var tblcompetitor = db.tblCompetitors.Find(int.Parse(chk));
            var result = string.Empty;
            if (nchecked == "true")
            {
                tblcompetitor.Active = false;
            }
            else
            { tblcompetitor.Active = true; }

            //db.Entry(MenuGroupsProduct).State = System.Data.EntityState.Modified; 
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Update Active tblcompetitor", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            result = "Updated Active.";
            return Json(new { result = result });
        }
        public ActionResult Command(FormCollection collection)
        {

            if (collection["btnDeleteAll"] != null)
            {
                foreach (string key in Request.Form)
                {
                    var checkbox = "";
                    if (key.StartsWith("chkitem+"))
                    {
                        checkbox = Request.Form["" + key];
                        if (checkbox != "false")
                        {
                            Int32 id = Convert.ToInt32(key.Remove(0, 8));
                            tblCompetitor tblcompetitor = db.tblCompetitors.Find(id);
                            db.tblCompetitors.Remove(tblcompetitor);
                            db.SaveChanges();
                            #region[Updatehistory]
                            Updatehistoty.UpdateHistory("Delete tblcompetitor", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
                            #endregion
                            return Redirect("Index");
                        }
                    }
                }
            }
            return RedirectToAction("Index");


        }

    }
}
