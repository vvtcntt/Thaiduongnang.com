using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THAIDUONGNANG.Models;
using PagedList;
using PagedList.Mvc;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.IO;
using System.Xml;
namespace THAIDUONGNANG.Controllers.Admin.Product
{
    public class GroupProductController : Controller
    {
        private THAIDUONGNANGContext db = new THAIDUONGNANGContext();

        //
        // GET: /GroupProduct/

        public ActionResult Index(int?page, string id)
        {
             if ((Request.Cookies["Username"] == null))
            {
                return RedirectToAction("LoginIndex", "Login");
            }
            var listProduct = db.tblGroupProducts.Where(p => p.Level.Length == 5).OrderBy(p=>p.Ord).ToList();
            if (id != "" && id != null)
            {
                int idmenu = int.Parse(id);
                var menucha = db.tblGroupProducts.Find(idmenu);
                string Lever = menucha.Level;
                int dodai = menucha.Level.Length;
                listProduct = db.tblGroupProducts.Where(p => p.Level.Length == (dodai + 5) && p.Level.Substring(0, dodai) == Lever).OrderBy(p=>p.Ord).ToList();
                ViewBag.Idcha = id;
            }
            else
            {
                ViewBag.Idcha = 0;
            }
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
            return View(listProduct.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /GroupProduct/Details/5

     
        
        public ActionResult Create(int id)
        {
            if ((Request.Cookies["Username"] == null))
            {
                return RedirectToAction("LoginIndex", "Login");
            }
            var menuName = db.tblGroupProducts.Where(p=>p.Active==true).ToList();
            var pro = db.tblGroupProducts.OrderByDescending(p => p.Ord).Take(1).ToList();
            var GroupsProducts = db.tblGroupProducts.Where(m=>m.Active==true).OrderBy(m => m.Level).ToList();
            var listpage = new List<SelectListItem>();
            listpage.Clear();
            listpage.AddRange(GroupsProducts.Select(t => new SelectListItem { Text = "" +  StringClass.ShowNameLevel(t.Name, t.Level), Value = "/danh-muc-san-pham/" + t.Tag.ToString(CultureInfo.InvariantCulture)}));
            var menuModel = db.tblGroupProducts.Where(m=>m.Active==true).OrderBy(m => m.Level  ).ToList();
            var lstMenu = new List<SelectListItem>();
            lstMenu.Clear();
            foreach (var menu in menuModel)
            {
                lstMenu.Add(new SelectListItem { Text = StringClass.ShowNameLevel(menu.Name, menu.Level), Value = menu.id.ToString() });

            }

            //var ListManufactures = db.tblManufactures.Where(p => p.Active == true).OrderBy(p => p.Ord).ToList();
            ////var menuName = db.Menus.ToList();

            //ViewBag.drManu = new SelectList(ListManufactures, "id", "Name");

            ViewBag.drMenu = new SelectList(lstMenu, "Value", "Text", id);
            if (pro.Count > 0)
                ViewBag.Ord = pro[0].Ord + 1;
            else
                ViewBag.Ord = "0";
            return View();
        }

        //
        // POST: /GroupProduct/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(tblGroupProduct tblgroupproduct, FormCollection collection, List<HttpPostedFileBase> uploadFile)
        {
            if (ModelState.IsValid)
            {
                if ((Request.Cookies["Username"] == null))
                {
                    return RedirectToAction("LoginIndex", "Login");
                }
                string drMenu = collection["drMenu"];
                 string nLevel;

                if (drMenu == "")
                {
                    nLevel = "00000";
                }
                else
                {
                    var dbLeve = db.tblGroupProducts.Find(int.Parse(drMenu));
                    nLevel = dbLeve.Level + "00000";
                }
                
                tblgroupproduct.Level = nLevel;
                tblgroupproduct.DateCreate = DateTime.Now;
                string idUser = Request.Cookies["Username"].Values["UserID"];
                tblgroupproduct.idUser = int.Parse(idUser);

                tblgroupproduct.Tag = StringClass.NameToTag(tblgroupproduct.Name);
                db.tblGroupProducts.Add(tblgroupproduct);
                db.SaveChanges();
                #region[Updatehistory]
                Updatehistoty.UpdateHistory("Add Product", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
                #endregion
                var Groups = db.tblGroupProducts.Where(p => p.Active == true).OrderByDescending(p => p.id).Take(1).ToList();
                string id = Groups[0].id.ToString();

                TempData["Msg"] = "";
                string abc = "";
                string def = "";
                var list = db.tblGroupProducts.OrderByDescending(p => p.id).Take(1).ToList();
                int idpro = int.Parse(list[0].id.ToString());
                if (uploadFile != null)
                {
                    foreach (var item in uploadFile)
                    {
                        if (item != null)
                        {
                            string filename = item.FileName;
                            string path = System.IO.Path.Combine(Server.MapPath("~/Images/ImagesList"), System.IO.Path.GetFileName(item.FileName));
                            item.SaveAs(path);
                            abc = string.Format("Upload {0} file thành công", uploadFile.Count);
                            def += item.FileName + "; ";
                            tblImage3D imgp = new tblImage3D();
                            imgp.idProduct = idpro;
                            imgp.Images = "/Images/ImagesList/" + System.IO.Path.GetFileName(item.FileName);
                            db.tblImage3D.Add(imgp);
                            db.SaveChanges();
                        }

                    }
                    TempData["Msg"] = abc + "</br>" + def;
                }







              
                clsSitemap.CreateSitemap("0/"+StringClass.NameToTag(tblgroupproduct.Name),id,"GroupProduct");
                return Redirect("Index?id=" + drMenu);
            }

            return View(tblgroupproduct);
        }

        //
        // GET: /GroupProduct/Edit/5

        public ActionResult Edit(int?id)
        {
            if ((Request.Cookies["Username"] == null))
            {
                return RedirectToAction("LoginIndex", "Login");
            }

            Int32 ids = Int32.Parse(id.ToString());
            tblGroupProduct tblgroupproduct = db.tblGroupProducts.Find(ids);
            if (tblgroupproduct == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = ids;
            var menuName = db.tblGroupProducts.ToList();
            var pro = db.tblGroupProducts.OrderByDescending(p => p.Ord).Take(1).ToList();
            var GroupsProducts = db.tblGroupProducts.OrderBy(m => m.Level).ToList();
            var listpage = new List<SelectListItem>();
            //listpage.Clear();
            //listpage.AddRange(GroupsProducts.Select(t => new SelectListItem { Text = "" + StringClass.ShowNameLevel(t.Name, t.Level), Value = "/danh-muc-san-pham/" + t.Tag.ToString(CultureInfo.InvariantCulture) }));
            var menuModel = db.tblGroupProducts.OrderBy(m => m.Level).ToList();
            var lstMenu = new List<SelectListItem>();
            lstMenu.Clear();
            foreach (var menu in menuModel)
            {
               
                lstMenu.Add(new SelectListItem { Text = StringClass.ShowNameLevel(menu.Name, menu.Level), Value = menu.id.ToString() });

               
            }

            var listfile = db.tblFiles.Where(p => p.idp == ids).ToList();
            if (listfile.Count > 0)
            {
                ViewBag.tenfile = "File thông số kỹ thuật : " + listfile[0].Name + "";
            }

            ViewBag.drMenu = new SelectList(lstMenu, "Value", "Text", ids);

            
            return View(tblgroupproduct);
        }
  
       
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(tblGroupProduct tblgroupproduct, FormCollection collection, int?id, HttpPostedFileBase uploadFiles, List<HttpPostedFileBase> uploadFile)
        {
            if (ModelState.IsValid)
            {
                
             id = int.Parse(collection["idProduct"]);
            tblgroupproduct = db.tblGroupProducts.Find(id);
                 
                ViewBag.id = id;
                string drMenu = collection["drMenu"];

                string nLevel = "";
                string nName = collection["Name"];
                string nTitle=collection["Title"];
                string nDescription = collection["Description"];
                string nKeyword = collection["Keyword"];
                string nContent = collection["Content"];
                string nNote = collection["Note"];
                int nOrd = int.Parse(collection["Ord"]);
                string nTag;

                string URL = collection["URL"];
                if (URL == "on")
                {
                    nTag =  StringClass.NameToTag(tblgroupproduct.Name) ;
                    clsSitemap.UpdateSitemap("/0/" + StringClass.NameToTag(nTag), id.ToString(), "GroupProduct");

                }
                else
                {
                    nTag = collection["NameURL"];
                }

                bool nIndex = (collection["Index"] == "True") ? true : false;
                bool nPriority = (collection["Priority"] == "True") ? true : false;
                bool nActive = (collection["Active"] == "True") ? true : false;             //bool nActive = (collection["Active"] == "True") ? true : false;
                string nImage = collection["Images"];
                string nBackground = collection["Background"];
                string niCon = collection["iCon"];
                string nVideoInfo = collection["VideoInfo"];
                string nVideoSetup = collection["VideoSetup"];
                string nCertificate = collection["Certificate"];
                string nImage3D = collection["Image3D"];
                bool nBaogia = (collection["Baogia"] == "True") ? true : false;   
                string nInfo = collection["Info"];
                DateTime nDateCreate = DateTime.Now;
                string nidUser = Request.Cookies["Username"].Values["UserID"];
                if (drMenu == "")
                {
                    nLevel = "00000"; tblgroupproduct.Level = nLevel;
                }
                else
                {
                    int idMenu = int.Parse(drMenu);
                    var Listda = db.tblGroupProducts.First(p => p.id == idMenu);
                    if (drMenu == id.ToString())
                    {
                        nLevel = Listda.Level;
                    }
                    else
                    {
                        nLevel = Listda.Level + "00000"; tblgroupproduct.Level = nLevel;
                        

 
                    }

                }
                tblgroupproduct.Name = nName;
                tblgroupproduct.Title = nTitle;
                tblgroupproduct.Description = nDescription;
                tblgroupproduct.Keyword = nKeyword;
                tblgroupproduct.Content = nContent;
                tblgroupproduct.Note = nNote;
                tblgroupproduct.Ord = nOrd;
                tblgroupproduct.Tag = nTag;
                tblgroupproduct.Level = nLevel;
                tblgroupproduct.Index = nIndex;
                tblgroupproduct.Baogia = nBaogia;
                tblgroupproduct.Priority = nPriority;
                tblgroupproduct.Active = nActive;
                tblgroupproduct.Images = nImage;
                tblgroupproduct.Background = nBackground;
                tblgroupproduct.iCon = niCon;
                tblgroupproduct.VideoInfo = nVideoInfo;
                tblgroupproduct.VideoSetup = nVideoSetup;
                tblgroupproduct.Certificate = nCertificate;
                tblgroupproduct.DateCreate = nDateCreate;
                tblgroupproduct.idUser = int.Parse(nidUser);
                tblgroupproduct.Info = nInfo;
                db.SaveChanges();
                #region[Updatehistory]
                Updatehistoty.UpdateHistory("Edit GroupsProduct", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
                #endregion
               
                //Upload file
                if (uploadFiles != null)
                {
                    var listfile = db.tblFiles.Where(p => p.idp == id).ToList();
                    if (listfile.Count > 0)
                    {
                        string fullPath = Request.MapPath("~/" + listfile[0].File);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        db.tblFiles.Remove(listfile[0]);
                        db.SaveChanges();
                    }
                    string filePath = Path.Combine(HttpContext.Server.MapPath("~/Images/files"), Path.GetFileName(uploadFiles.FileName));
                    tblFile tblfile = new tblFile();
                    tblfile.Name = tblgroupproduct.Name + "[" + uploadFiles.FileName + "]";
                    tblfile.File = "/Images/files/" + uploadFiles.FileName + "";
                    tblfile.Cate = 2;
                    tblfile.idp = id;
                    db.tblFiles.Add(tblfile);
                    db.SaveChanges();
                    uploadFiles.SaveAs(filePath);
                }

                foreach (string key in Request.Form.Cast<string>().Where(key => key.StartsWith("chek-")))
                {
                    var checkbox = "";
                    checkbox = Request.Form["" + key];
                    if (checkbox != "false")
                    {
                        Int32 idchk = Convert.ToInt32(key.Remove(0, 5));
                        tblImage3D image = db.tblImage3D.Find(idchk);
                        db.tblImage3D.Remove(image);
                        db.SaveChanges();
                    }
                }
                TempData["Msg"] = "";
                string abc = "";
                string def = "";

                if (uploadFile != null)
                {
                    foreach (var item in uploadFile)
                    {
                        if (item != null)
                        {
                            string filename = item.FileName;
                            string path = System.IO.Path.Combine(Server.MapPath("~/Images/ImagesList"), System.IO.Path.GetFileName(item.FileName));
                            item.SaveAs(path);
                            abc = string.Format("Upload {0} file thành công", uploadFile.Count);
                            def += item.FileName + "; ";
                            tblImage3D imgp = new tblImage3D();
                            imgp.idProduct = id;
                            imgp.Images = "/Images/ImagesList/" + System.IO.Path.GetFileName(item.FileName);
                            db.tblImage3D.Add(imgp);
                            db.SaveChanges();
                        }

                    }
                    TempData["Msg"] = abc + "</br>" + def;
                }
                if (nLevel.Length > 5)
                {
                    var list = db.tblGroupProducts.First(p => p.Level == nLevel.Substring(0, nLevel.Length - 5));
                    return Redirect("Index?id=" + list.id);
                }
                else
                    return Redirect("Index");
             }
            return View(tblgroupproduct);
        }
        public PartialViewResult ListImage3D(int id)
        {
            var listImages = db.tblImage3D.Where(p => p.idProduct == id).ToList();
            string chuoi = "";
            for (int i = 0; i < listImages.Count; i++)
            {
                chuoi += " <div class=\"Tear_Images\">";
                chuoi += " <img src=\"" + listImages[i].Images + "\" alt=\"\"/>";
                chuoi += " <input type=\"checkbox\" name=\"chek-" + listImages[i].id + "\" id=\"chek-" + listImages[i].id + "\" /> Xóa";
                chuoi += "</div>";

            }
            ViewBag.chuoi = chuoi;
            return PartialView();

        }
        //
        // POST: /GroupProduct/Edit/5
        [HttpPost]
        public ActionResult GroupProductEditOrd(int txtSort, string ts)
        {
            var MenuGroupsProduct = db.tblGroupProducts.Find(txtSort);
            var result = string.Empty;
            MenuGroupsProduct.Ord = int.Parse(ts);
            //db.Entry(MenuGroupsProduct).State = System.Data.EntityState.Modified;
            result = "Update Ord.";
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Update Ord GroupsProduct", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            return Json(new { result = result });
        }
        [HttpPost]
        public ActionResult GroupProductEditActive(string chk, string nchecked)
        {

            var MenuGroupsProduct = db.tblGroupProducts.Find(int.Parse(chk));
            var result = string.Empty;
            if (nchecked == "true")
            {
                MenuGroupsProduct.Active = false;
            }
            else
            { MenuGroupsProduct.Active = true; }

            //db.Entry(MenuGroupsProduct).State = System.Data.EntityState.Modified; 
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Update Active GroupsProduct", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            result = "Updated Active.";
            return Json(new { result = result });
        }
        //
        // GET: /GroupProduct/Delete/5

        public ActionResult Delete(int id)
        {
            tblGroupProduct tblgroupproduct = db.tblGroupProducts.Find(id);
            if (tblgroupproduct == null)
            {
                return HttpNotFound();
            }
            clsSitemap.DeteleSitemap(id.ToString(), "GroupProduct");
            return RedirectToAction("Index");
        }

        //
        // POST: /GroupProduct/Delete/5


        public ActionResult DeleteAll(FormCollection collection)
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
                            var ListLevel = db.tblGroupProducts.First(p => p.id == id);
                            string LevelParent = ListLevel.Level;
                            int Length = LevelParent.Length;
                            var ListGroupsProduct = db.tblGroupProducts.Where(p => p.Level.Length > Length && p.Level.Substring(0, Length) == LevelParent).ToList();
                            for (int i = 0; i < ListGroupsProduct.Count; i++)
                            {
                                var idChild = ListGroupsProduct[i].id;
                                var ListChild = db.tblGroupProducts.First(p => p.id == idChild);
                                var listProduct = db.tblProducts.Where(p => p.idCate == ListChild.id).ToList();
                                for (int j = 0; j < listProduct.Count; j++)
                                {

                                    db.tblProducts.Remove(listProduct[i]);
                                }
                                db.tblGroupProducts.Remove(ListChild);

                            }
                            clsSitemap.DeteleSitemap(id.ToString(), "GroupProduct");

                            db.tblGroupProducts.Remove(ListLevel);
                            db.SaveChanges();
                            #region[Updatehistory]
                            Updatehistoty.UpdateHistory("Delete GroupsProduct", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
                            #endregion
                            return Redirect("Index");
                        }
                    }
                }
            }
            return RedirectToAction("Index");
             

        }
        public ActionResult DeleteConfirmed(int id)
        {
            if ((Request.Cookies["Username"] == null))
            {
                return RedirectToAction("LoginIndex", "Login");
            }
            var ListLevel = db.tblGroupProducts.First(p => p.id == id);
            string LevelParent = ListLevel.Level;
            int Length = LevelParent.Length;
            var ListGroupsProduct = db.tblGroupProducts.Where(p => p.Level.Length > Length && p.Level.Substring(0, Length) == LevelParent).ToList();
            for (int i = 0; i < ListGroupsProduct.Count; i++)
            {
                var idChild = ListGroupsProduct[i].id;
                var ListChild = db.tblGroupProducts.First(p => p.id == idChild);
                var listProduct = db.tblProducts.Where(p => p.idCate == ListChild.id).ToList();
                for (int j = 0; j < listProduct.Count; j++)
                {

                    db.tblProducts.Remove(listProduct[i]);
                }
                db.tblGroupProducts.Remove(ListChild);
                clsSitemap.DeteleSitemap(ListGroupsProduct[i].id.ToString(), "GroupProduct");

            }
            clsSitemap.DeteleSitemap(id.ToString(), "GroupProduct");
            db.tblGroupProducts.Remove(ListLevel);
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Delete GroupsProduct", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            return Redirect("Index");
        }
        [HttpPost]
        public string CheckValue(string text)
        {
            string chuoi = "";
            var listProduct = db.tblGroupProducts.Where(p => p.Name == text).ToList();
            if (listProduct.Count > 0)
            {
                chuoi = "Duplicate Name !";

            }
            Session["Check"] = listProduct.Count;
            return chuoi;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public PartialViewResult ParitalListProduct(int? page, string id)
        {
            
        //    if(id==null)
        //{
        //    id = "0";
        //    }
        
         
        //        int idmenu = int.Parse(id);
        //        var menucha = db.tblGroupProducts.Find(idmenu);
        //         var listProduct = db.tblProducts.Where(p => p.idCate == idmenu).OrderBy(p => p.Ord).ToList();
        //        ViewBag.Idcha = "id="+id;
             
        //    const int pageSize = 20;
        //    var pageNumber = (page ?? 1);
        //    // Thiết lập phân trang
        //    var ship = new PagedListRenderOptions
        //    {
        //        DisplayLinkToFirstPage = PagedListDisplayMode.Always,
        //        DisplayLinkToLastPage = PagedListDisplayMode.Always,
        //        DisplayLinkToPreviousPage = PagedListDisplayMode.Always,
        //        DisplayLinkToNextPage = PagedListDisplayMode.Always,
        //        DisplayLinkToIndividualPages = true,
        //        DisplayPageCountAndCurrentLocation = false,
        //        MaximumPageNumbersToDisplay = 5,
        //        DisplayEllipsesWhenNotShowingAllPageNumbers = true,
        //        EllipsesFormat = "&#8230;",
        //        LinkToFirstPageFormat = "Trang đầu",
        //        LinkToPreviousPageFormat = "«",
        //        LinkToIndividualPageFormat = "{0}",
        //        LinkToNextPageFormat = "»",
        //        LinkToLastPageFormat = "Trang cuối",
        //        PageCountAndCurrentLocationFormat = "Page {0} of {1}.",
        //        ItemSliceAndTotalFormat = "Showing items {0} through {1} of {2}.",
        //        FunctionToDisplayEachPageNumber = null,
        //        ClassToApplyToFirstListItemInPager = null,
        //        ClassToApplyToLastListItemInPager = null,
        //        ContainerDivClasses = new[] { "pagination-container" },
        //        UlElementClasses = new[] { "pagination" },
        //        LiElementClasses = Enumerable.Empty<string>()
        //    };
        //    ViewBag.Product = ship;
            //return PartialView(listProduct.ToPagedList(pageNumber, pageSize));
            return PartialView();
        }
    }
}