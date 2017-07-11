using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THAIDUONGNANG.Models;
using PagedList;
using PagedList.Mvc;
using System.Data;
using System.Data.Entity;
namespace THAIDUONGNANG.Controllers.Admin.Ong
{
    public class OngController : Controller
    {
        THAIDUONGNANGContext db = new THAIDUONGNANGContext();
        // GET: Ong
        public ActionResult Index(int? page, string id)
        {
            if ((Request.Cookies["Username"] == null))
            {
                return RedirectToAction("LoginIndex", "Login");
            }
            var ListOng = db.tblOngs.ToList();

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
            return View(ListOng.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            if ((Request.Cookies["Username"] == null))
            {
                return RedirectToAction("LoginIndex", "Login");
            }
            var pro = db.tblOngs.OrderByDescending(p => p.Ord).Take(1).ToList();
            if (pro.Count > 0)
                ViewBag.Ord = pro[0].Ord + 1;
            return View();
        }

        //
        // POST: /Url/Create

        [HttpPost]
        public ActionResult Create(tblOng tblong)
        {
         

             
            db.tblOngs.Add(tblong);
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Add Ong", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
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
            tblOng tblong = db.tblOngs.Find(id);
            if (tblong == null)
            {
                return HttpNotFound();
            }
            return View(tblong);
        }

        //
        // POST: /Url/Edit/5

        [HttpPost]
        public ActionResult Edit(tblOng tblong)
        {
            if (ModelState.IsValid)
            {
               
                db.Entry(tblong).State = EntityState.Modified;
                db.SaveChanges();
                #region[Updatehistory]
                Updatehistoty.UpdateHistory("Edit Url", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
                #endregion
                return RedirectToAction("Index");
            }
            return View(tblong);
        }

        //
        // GET: /Url/Delete/5

        public ActionResult Delete(int id = 0)
        {
            tblOng tblong = db.tblOngs.Find(id);
            if (tblong == null)
            {
                return HttpNotFound();
            }
            return View(tblong);
        }

        //
        // POST: /Url/Delete/5

        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            tblOng tblong = db.tblOngs.Find(id);
            db.tblOngs.Remove(tblong);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public ActionResult OngEditOrd(int txtSort, string ts)
        {
            var tblong = db.tblOngs.Find(txtSort);
            var result = string.Empty;
            tblong.Ord = int.Parse(ts);
            //db.Entry(MenuGroupsProduct).State = System.Data.EntityState.Modified;
            result = "Update Ord.";
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Update Ord Ong", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
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
                            tblOng tblong = db.tblOngs.Find(id);
                            db.tblOngs.Remove(tblong);
                            db.SaveChanges();
                            #region[Updatehistory]
                            Updatehistoty.UpdateHistory("Delete Ong", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
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