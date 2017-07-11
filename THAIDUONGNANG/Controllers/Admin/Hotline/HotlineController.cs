using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THAIDUONGNANG.Models;
using PagedList;
using PagedList.Mvc;
using System.Globalization;
using System.Data;
using System.Data.Entity;
namespace THAIDUONGNANG.Controllers.Admin.Hotline
{
    public class HotlineController : Controller
    {
        //
        // GET: /Hotline/
        private THAIDUONGNANGContext db = new THAIDUONGNANGContext();


        public ActionResult Index(int? page, string id)
        {
            if ((Request.Cookies["Username"] == null))
            {
                return RedirectToAction("LoginIndex", "Login");
            }
            var ListHotline = db.tblHotlines.OrderBy(p=>p.Ord).ToList();

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
            return View(ListHotline.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult Create()
        {
            if ((Request.Cookies["Username"] == null))
            {
                return RedirectToAction("LoginIndex", "Login");
            }
            var pro = db.tblHotlines.OrderByDescending(p => p.Ord).Take(1).ToList();
            if (pro.Count > 0)
                ViewBag.Ord = pro[0].Ord + 1;
            return View();
        }

        //
        // POST: /Url/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(tblHotline tblhotline)
        {
            db.tblHotlines.Add(tblhotline);
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Add tblhotline", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
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
            tblHotline tblhotline = db.tblHotlines.Find(id);
            if (tblhotline == null)
            {
                return HttpNotFound();
            }
            return View(tblhotline);
        }

        //
        // POST: /Url/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(tblHotline tblhotline)
        {
            if (ModelState.IsValid)
            {

                db.Entry(tblhotline).State = EntityState.Modified;
                db.SaveChanges();
                #region[Updatehistory]
                Updatehistoty.UpdateHistory("Edit tblhotline", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
                #endregion
                return RedirectToAction("Index");
            }
            return View(tblhotline);
        }

        //
        // GET: /Url/Delete/5

        public ActionResult Delete(int id = 0)
        {
            tblHotline tblhotline = db.tblHotlines.Find(id);
            if (tblhotline == null)
            {
                return HttpNotFound();
            }
            return View(tblhotline);
        }

        //
        // POST: /Url/Delete/5

        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            tblHotline tblhotline = db.tblHotlines.Find(id);
            db.tblHotlines.Remove(tblhotline);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public ActionResult HotlineEditOrd(int txtSort, string ts)
        {
            var tblhotline = db.tblHotlines.Find(txtSort);
            var result = string.Empty;
            tblhotline.Ord = int.Parse(ts);
            //db.Entry(MenuGroupsProduct).State = System.Data.EntityState.Modified;
            result = "Update Ord.";
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Update Ord tblhotline", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            return Json(new { result = result });
        }
        [HttpPost]
        public ActionResult HotlineEditActive(string chk, string nchecked)
        {

            var tblhotline = db.tblHotlines.Find(int.Parse(chk));
            var result = string.Empty;
            if (nchecked == "true")
            {
                tblhotline.Active = false;
            }
            else
            { tblhotline.Active = true; }

            //db.Entry(MenuGroupsProduct).State = System.Data.EntityState.Modified; 
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Update Active tblhotline", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
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
                            tblHotline tblhotline = db.tblHotlines.Find(id);
                            db.tblHotlines.Remove(tblhotline);
                            db.SaveChanges();
                            #region[Updatehistory]
                            Updatehistoty.UpdateHistory("Delete tblhotline", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
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
