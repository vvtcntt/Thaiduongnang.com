﻿using System;
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
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using Microsoft.Office.Core;
namespace THAIDUONGNANG.Controllers.Admin.Productad
{
    public class ProductadController : Controller
    {
        private THAIDUONGNANGContext db = new THAIDUONGNANGContext();
       
        public ActionResult Index(int? page, string text, string idCate)
        {
            if ((Request.Cookies["Username"] == null))
            {
                return RedirectToAction("LoginIndex", "Login");
            }
            string txtSearch = "";
            var listProduct = db.tblProducts.Where(p=>p.Active==true).OrderByDescending(p=>p.DateCreate).ToList();

            if (text != null)
            { listProduct = db.tblProducts.Where(p => p.Name.ToUpper().Contains(text.ToUpper()) && p.Active == true).OrderByDescending(p => p.DateCreate).ToList(); }

            if (txtSearch != null && txtSearch != "")
            {
                listProduct = db.tblProducts.Where(p => p.Name.Contains(txtSearch) && p.Active==true).ToList();
            }
            Session["txtSearch"] = null;
            const int pageSize = 20;
            var pageNumber = (page ?? 1);
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
            #region[Load Menu]

            var pro = db.tblGroupProducts.OrderByDescending(p => p.Ord).Take(1).ToList();
            var GroupsProducts = db.tblGroupProducts.OrderBy(m => m.Level).ToList();
            var listpage = new List<SelectListItem>();
            var menuModel = db.tblGroupProducts.OrderBy(m => m.Level).ToList();
            var lstMenu = new List<SelectListItem>();
            lstMenu.Clear();
            foreach (var menu in menuModel)
            {
                lstMenu.Add(new SelectListItem { Text = StringClass.ShowNameLevel(menu.Name, menu.Level), Value = menu.id.ToString() });
            }
            if (idCate != "")
            {
                ViewBag.drMenu = new SelectList(lstMenu, "Value", "Text", idCate);
            }
            else
            {
                ViewBag.drMenu = lstMenu;
            }
            #endregion

            if (Request.IsAjaxRequest())
            {
                int idCatelogy;
                if (text != null && text != "")
                {
                    listProduct = db.tblProducts.Where(p => p.Name.ToUpper().Contains(text.ToUpper()) && p.Active == true).OrderByDescending(p => p.DateCreate).ToList();
                    return PartialView("PartialProductData", listProduct);
                }
                if (idCate != null && idCate != "")
                {
                    idCatelogy = int.Parse(idCate);
                    listProduct = db.tblProducts.Where(p => p.idCate == idCatelogy && p.Active == true).OrderByDescending(p => p.DateCreate).ToList(); ViewBag.idMenu = idCate;
                    return PartialView("PartialProductData", listProduct);
                }
                if (text != null && text != "" && idCate != null && idCate != "")
                {
                    idCatelogy = int.Parse(idCate);
                    ViewBag.idMenu = idCate;
                    listProduct = db.tblProducts.Where(p => p.Name.ToUpper().Contains(text.ToUpper()) && p.idCate == (int.Parse(idCate)) && p.Active == true).OrderByDescending(p => p.Ord).ToList();
                    return PartialView("PartialProductData", listProduct);
                }

                else
                {
                    return PartialView("PartialProductData", listProduct);
                }
            }

            if (Session["Thongbao"] != null && Session["Thongbao"] != "")
            {

                ViewBag.thongbao = Session["Thongbao"].ToString();
                Session["Thongbao"] = "";
            }
             if (idCate != "" && idCate != null)
            {
                int idcates = int.Parse(idCate);
                listProduct = db.tblProducts.Where(p => p.idCate == idcates && p.Active == true).OrderByDescending(p => p.DateCreate).ToList();
                ViewBag.idMenu = idCate;
            }
            return View(listProduct.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult UpdateImage(tblProduct tblproduct)
        {
             
            var listProduct = db.tblGroupNews.ToList();
            for (int i = 0; i < listProduct.Count; i++)
            {
                int id = int.Parse(listProduct[i].id.ToString());
                var Product = db.tblGroupNews.First(p => p.id == id);
                string tags = Product.Name;
                Product.Tag = StringClass.NameToTag(tags);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        //
        // GET: /Product/Details/5
        public PartialViewResult PartialProductData()
        {
            
            return PartialView();

        }

        //
        // GET: /Product/Create
        public ActionResult ProductEditActive(string chk, string nchecked)
        {

            var Product = db.tblProducts.Find(int.Parse(chk));
            var result = string.Empty;
            if (nchecked == "true")
            {
                Product.Active = false;
            }
            else
            { Product.Active = true; }

            //db.Entry(Product).State = System.Data.EntityState.Modified;
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Edit  Active Product", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            result = "Active Updated.";
            return Json(new { result = result });
        }
        public ActionResult ProductEditProductSale(string chk, string nchecked)
        {

            var Product = db.tblProducts.Find(int.Parse(chk));
            var result = string.Empty;
            if (nchecked == "true")
            {
                Product.ProductSale = false;
            }
            else
            { Product.ProductSale = true; }

            //db.Entry(Product).State = System.Data.EntityState.Modified;
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Edit  ProductSale Product", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            result = "Active Updated.";
            return Json(new { result = result });
        }
        public ActionResult ProductEditPriority(string chk, string nchecked)
        {

            var Product = db.tblProducts.Find(int.Parse(chk));
            var result = string.Empty;
            if (nchecked == "true")
            {
                Product.Priority = false;
            }
            else
            { Product.Priority = true; }

            //db.Entry(Product).State = System.Data.EntityState.Modified;
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Edit  Priority Product", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            result = "Active Updated.";
            return Json(new { result = result });
        }
        public ActionResult Create(string id)
        {
            if ((Request.Cookies["Username"] == null))
            {
                return RedirectToAction("LoginIndex", "Login");
            }

            var pro = db.tblProducts.OrderByDescending(p => p.Ord).Take(1).ToList();
            var GroupsProducts = db.tblGroupProducts.OrderBy(m => m.Level).ToList();
            var listpage = new List<SelectListItem>();
            listpage.Clear();
            listpage.AddRange(GroupsProducts.Select(t => new SelectListItem { Text = "" + StringClass.ShowNameLevel(t.Name, t.Level), Value = "/danh-muc-san-pham/" + t.Tag.ToString(CultureInfo.InvariantCulture) }));
            var menuModel = db.tblGroupProducts.OrderBy(m => m.Level).ToList();
            var lstMenu = new List<SelectListItem>();
            lstMenu.Clear();
            foreach (var menu in menuModel)
            {

                lstMenu.Add(new SelectListItem { Text = StringClass.ShowNameLevel(menu.Name, menu.Level), Value = menu.id.ToString() });


            }

            if (id != "")
            {

                ViewBag.drMenu = new SelectList(lstMenu, "Value", "Text", id);
                int idcate = int.Parse(id.ToString());
                pro = db.tblProducts.Where(p => p.idCate == idcate).OrderByDescending(p => p.Ord).Take(1).ToList();
            }
            else
                ViewBag.drMenu = lstMenu;
            if (pro.Count > 0)
                ViewBag.Ord = pro[0].Ord + 1;


            //Load chức năng
            string chuoi = "";
            var listFunction = db.tblFunctionProducts.Where(p => p.Active == true).OrderBy(p => p.Ord).ToList();
            for (int i = 0; i < listFunction.Count;i++)
            {
                chuoi += "<input type=\"checkbox\" name=\"chkFun+" + listFunction[i].id + "\" id=\"chkFun+" + listFunction[i].id + "\" class=\"chkFuc\" /> " + listFunction[i].Name + "</br>";
            }
            ViewBag.chuoifun = chuoi;
            //Load chức năng
            string chuoicolor = "";
            var listcolor = db.tblColorProducts.Where(p => p.Active == true).OrderBy(p => p.Ord).ToList();
            for (int i = 0; i < listcolor.Count; i++)
            {
                chuoicolor += "<input type=\"checkbox\" name=\"chkCol+" + listcolor[i].id + "\" id=\"chkCol+" + listcolor[i].id + "\" class=\"chkFuc\" /> " + listcolor[i].Name + "</br>";
            }
            ViewBag.chuoicolor = chuoicolor;

            //Load

            string ChuoiOng = "";
            var listOng = db.tblOngs.Where(p => p.Active == true).OrderBy(p => p.Ord).ToList();
            for (int i = 0; i < listOng.Count;i++)
            {
                ChuoiOng += "<input type=\"radio\" name=\"chkO\" id=\"chkO_" + listOng[i].id + "\" value=\"" + listOng[i].id + "\" /> : " + listOng[i].Name + "";
            }
            ViewBag.ChuoiOng = ChuoiOng;

            return View();
        }

        //
        // POST: /Product/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(tblProduct tblproduct, FormCollection Collection, string id, List<HttpPostedFileBase> uploadFile, HttpPostedFileBase uploadFiles)
        {

            if ((Request.Cookies["Username"] == null))
            {
                return RedirectToAction("LoginIndex", "Login");
            }
            string nidCate = Collection["drMenu"];
            if (nidCate != "")
            {
                string group = Collection["Group"];
                string nchkO= Collection["chkO"];
                tblproduct.idCate = int.Parse(nidCate);
                int idcate = int.Parse(nidCate);
                tblproduct.LoaiOng = nchkO;
                tblproduct.DateCreate = DateTime.Now;
                tblproduct.Tag = StringClass.NameToTag(tblproduct.Name);
                tblproduct.DateCreate = DateTime.Now;
                tblproduct.Visit = 0;
                string[] listarray = tblproduct.ImageLinkDetail.Split('/');
                string imagethum=listarray[listarray.Length-1];
                tblproduct.ImageLinkThumb = "/Images/_thumbs/Images/" + imagethum;
                string idUser = Request.Cookies["Username"].Values["UserID"];
                tblproduct.idUser = int.Parse(idUser);
       
                db.tblProducts.Add(tblproduct);
                db.SaveChanges();
                var listprro = db.tblProducts.OrderByDescending(p => p.id).Take(1).ToList();
                string Urls = db.tblGroupProducts.Find(idcate).Tag;
                //Update sitemaps

                var GroupProduct = db.tblGroupProducts.Find(idcate);

                clsSitemap.CreateSitemap( tblproduct.Tag +"-mua-san-pham.html", listprro[0].id.ToString(), "Product");
                TempData["Msg"] = "";
                string abc = "";
                string def = "";
                var list = db.tblProducts.OrderByDescending(p => p.id).Take(1).ToList();
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
                            ImageProduct imgp = new ImageProduct();
                            imgp.idProduct = idpro;
                            imgp.Images = "/Images/ImagesList/" + System.IO.Path.GetFileName(item.FileName);
                            db.ImageProducts.Add(imgp);
                            db.SaveChanges();
                        }

                    }
                    TempData["Msg"] = abc + "</br>" + def;
                }
                //Upload file
                if(uploadFiles!=null)
                {
                if (uploadFiles.ContentLength > 0)
                {
                    string filePath = Path.Combine(HttpContext.Server.MapPath("~/Images/files"),
                                                   Path.GetFileName(uploadFiles.FileName));
                    tblFile tblfile = new tblFile();
                    tblfile.Name = tblproduct.Name + "["+uploadFiles.FileName+"]";
                    tblfile.File = "/Images/files/" + uploadFiles.FileName + "";
                    tblfile.Cate = 2;
                    tblfile.idp = int.Parse(db.tblProducts.OrderByDescending(p => p.id).Take(1).First().id.ToString());
                    db.tblFiles.Add(tblfile);
                    db.SaveChanges();
                    uploadFiles.SaveAs(filePath);
                }
                }
                if (Collection["btnAdd"] != null)
                {
                    foreach (string key in Request.Form)
                    {
                        var checkbox = "";
                        if (key.StartsWith("chkFun+"))
                        {
                            checkbox = Request.Form["" + key];
                            if (checkbox != "false")
                            {
                                Int32 idkey = Convert.ToInt32(key.Remove(0, 7));
                                tblConnectFunProuduct connectionfunction = new tblConnectFunProuduct();
                                connectionfunction.idFunc = idkey;
                                connectionfunction.idPro = idpro;
                                db.tblConnectFunProuducts.Add(connectionfunction);
                                db.SaveChanges();

                            }
                        }
                    }
                }
                foreach (string key in Request.Form)
                {
                    var checkbox = "";
                    if (key.StartsWith("chkCol+"))
                    {
                        checkbox = Request.Form["" + key];
                        if (checkbox != "false")
                        {
                            Int32 idkey = Convert.ToInt32(key.Remove(0, 7));
                            tblConnectColorProduct tblconection = new tblConnectColorProduct();
                            tblconection.idColor = idkey;
                            tblconection.idPro = idpro;
                            db.tblConnectColorProducts.Add(tblconection);
                            db.SaveChanges();

                        }
                    }
                }
            }
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Add Product", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
          
            return Redirect("Index?idCate=" + id);

        }
         
        public PartialViewResult ListImages(int id)
        {
            var listImages = db.ImageProducts.Where(p => p.idProduct == id).ToList();
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
        public ActionResult Edit(int?id)
        {
           
           
            if ((Request.Cookies["Username"] == null))
            {
                return RedirectToAction("LoginIndex", "Login");
            }
            Session["id"] = id.ToString();
            Int32 ids = Int32.Parse(id.ToString());
            tblProduct tblproduct = db.tblProducts.Find(ids);
            if (tblproduct == null)
            {
                return HttpNotFound();
            }
            var GroupsProducts = db.tblGroupProducts.OrderBy(m => m.Level).ToList();
            var listpage = new List<SelectListItem>();
            listpage.Clear();
            listpage.AddRange(GroupsProducts.Select(t => new SelectListItem { Text = "" + StringClass.ShowNameLevel(t.Name, t.Level), Value = "/danh-muc-san-pham/" + t.Tag.ToString(CultureInfo.InvariantCulture) }));
            var menuModel = db.tblGroupProducts.OrderBy(m => m.Level).ToList();
            var lstMenu = new List<SelectListItem>();
            lstMenu.Clear();
            foreach (var menu in menuModel)
            {
                lstMenu.Add(new SelectListItem { Text = StringClass.ShowNameLevel(menu.Name, menu.Level), Value = menu.id.ToString() });
            }
            int idGroups = int.Parse(tblproduct.idCate.ToString());
            ViewBag.drMenu = new SelectList(lstMenu, "Value", "Text", idGroups);
            ViewBag.id = id;
            string ChuoiOng = "";
            
            var listOng = db.tblOngs.Where(p => p.Active == true).OrderBy(p => p.Ord).ToList();
            for (int i = 0; i < listOng.Count; i++)
            {
                if(tblproduct.LoaiOng!=null)
                { 
                int idong = int.Parse(tblproduct.LoaiOng.ToString());
                if (listOng[i].id == idong)
                    ChuoiOng += "<input type=\"radio\" name=\"chkO\" id=\"chkO_" + listOng[i].id + "\" checked value=\"" + listOng[i].id + "\" /> : " + listOng[i].Name + "";
                else
                    ChuoiOng += "<input type=\"radio\" name=\"chkO\" id=\"chkO_" + listOng[i].id + "\" value=\"" + listOng[i].id + "\" /> : " + listOng[i].Name + "";
                }
                else
                {
                    ChuoiOng += "<input type=\"radio\" name=\"chkO\" id=\"chkO_" + listOng[i].id + "\" value=\"" + listOng[i].id + "\" /> : " + listOng[i].Name + "";
                }
            }
            ViewBag.ChuoiOng = ChuoiOng;
                return View(tblproduct);
        }
 
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(tblProduct tblproduct, FormCollection collection, int? id, List<HttpPostedFileBase> uploadFile, HttpPostedFileBase uploadFiles)
        {

            if (ModelState.IsValid)
            {
                if (collection["drMenu"] != "" || collection["drMenu"] != null)
                { 
                    if(id==null)
                {
                    id = int.Parse(collection["idProduct"]);
                     
                     tblproduct = db.tblProducts.Find(id);
                }
                    string group = collection["Group"];
                    ViewBag.id = id;
                    int idCate = int.Parse(collection["drMenu"]);
                    tblproduct.idCate = idCate;
                    tblproduct.DateCreate = DateTime.Now;
                    string tag = tblproduct.Tag;
                    string URL = collection["URL"];
                    string Name = collection["Name"];
                    string Code = collection["Code"];
                    string Description = collection["Description"];
                    string Content = collection["Content"];
                    string Info = collection["Info"];
                  
                    string ImageLinkDetail = collection["ImageLinkDetail"];
                    string[] listarray = collection["ImageLinkDetail"].Split('/');
                    string imagethum = listarray[listarray.Length - 1];
              string ImageLinkThumb = "/Images/_thumbs/Images/" + imagethum;
                    string ImageSale = collection["ImageSale"];
                    string chkfile = collection["chkfile"];
                    if(collection["Price"]!=null)
                    { 
                    float Price = float.Parse( collection["Price"]);
                    tblproduct.Price = Price;
                   
                    }
                    if (collection["PriceSale"] != null)
                    {
                        float PriceSale = float.Parse(collection["PriceSale"]);
                        tblproduct.PriceSale = PriceSale;
                    }
                    if(chkfile=="on")
                    {
                        tblFile tblfile = db.tblFiles.Where(p => p.idp == id).First();
                        string fullPath = Request.MapPath("~/"+tblfile.File);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                       
                        db.tblFiles.Remove(tblfile);
                        db.SaveChanges();

                    }
                    bool Vat = (collection["Vat"] == "True") ? true : false;
                    bool ProductSale = (collection["ProductSale"] == "True") ? true : false;
                    bool Note = (collection["Note"] == "True") ? true : false;
                    string Warranty = collection["Warranty"];
                    string Address = collection["Address"];
                    bool Transport = (collection["Transport"] == "True") ? true : false;
                    string Access = collection["Access"];
                    string Sale = collection["Sale"];
                    if (collection["Ord"] != null)
                    {
                        int Ord = int.Parse(collection["Ord"]);
                        tblproduct.Ord = Ord;
                        
                    }
                    if (collection["Status"] != null)
                    {
                        bool Status = (collection["Status"] == "True") ? true : false;
                        tblproduct.Status = Status;
                    }
                    bool Active = (collection["Active"] == "True") ? true : false;
                    bool New = (collection["New"] == "True") ? true : false;
                    bool ViewHomes = (collection["ViewHomes"] == "True") ? true : false;
                     string Title = collection["Title"];
                     string Keyword = collection["Keyword"];
                     string nDungtich = collection["Dungtich"];
                    string nSoOng=collection["SoOng"];
                    string nLoaiong = collection["chkO"];
                    string nKichThuocOng = collection["KichThuocOng"];
                    string nKichThuocDNVao=collection["KichThuocDNVao"];
                    string nKichThuocDNRa = collection["KichThuocDNRa"];
                    int nSoNguoiSuDung=int.Parse(collection["SoNguoiSuDung"].ToString());

                    tblproduct.Visit = tblproduct.Visit;
                    tblproduct.Name = Name;
                    tblproduct.Code = Code;
                    tblproduct.ImageSale = ImageSale;
                    tblproduct.Description = Description;
                    tblproduct.ProductSale = ProductSale;
                    tblproduct.Content = Content;
                    tblproduct.Info = Info;
                    tblproduct.ImageLinkThumb = ImageLinkThumb;
                    tblproduct.ImageLinkDetail = ImageLinkDetail;
                    tblproduct.Vat = Vat;
                    tblproduct.Warranty = Warranty;
                    tblproduct.Address = Address;
                    tblproduct.Transport = Transport;
                    tblproduct.Access = Access;
                    tblproduct.Sale = Sale;
                    tblproduct.Active = Active;
                    tblproduct.New = New;
                    tblproduct.Note = Note;
                    tblproduct.DateCreate = DateTime.Now;
                    tblproduct.ViewHomes = ViewHomes;
                    tblproduct.Title = Title;
                    tblproduct.Keyword = Keyword;
                    tblproduct.Dungtich = nDungtich;
                    tblproduct.SoOng = nSoOng;
                    tblproduct.LoaiOng = nLoaiong;
                    tblproduct.KichThuocOng = nKichThuocOng;
                    tblproduct.KichThuocDNRa = nKichThuocDNRa;
                    tblproduct.KichThuocDNVao = nKichThuocDNVao;
                    tblproduct.SoNguoiSuDung = nSoNguoiSuDung;
                     string urls = db.tblGroupProducts.Find(idCate).Tag;
                     if (URL == "on")
                    {
                        tblproduct.Tag = StringClass.NameToTag(tblproduct.Name);
                        var GroupProduct = db.tblGroupProducts.Find(idCate);

                        clsSitemap.UpdateSitemap(StringClass.NameToTag(tblproduct.Name) + "-mua-san-pham.html", id.ToString(), "Product");
                       
                    }
                    else
                    {
                        tblproduct.Tag = collection["NameURL"];
                        var GroupProduct = db.tblGroupProducts.Find(idCate);

                    }

                    string idUser = Request.Cookies["Username"].Values["UserID"];
                    tblproduct.idUser = int.Parse(idUser);
                    db.SaveChanges();
                   
                }
                #region[Updatehistory]
                Updatehistoty.UpdateHistory("Edit Product", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
                #endregion


                foreach (string key in Request.Form.Cast<string>().Where(key => key.StartsWith("chek-")))
                {
                    var checkbox = "";
                    checkbox = Request.Form["" + key];
                    if (checkbox != "false")
                    {
                        Int32 idchk = Convert.ToInt32(key.Remove(0, 5));
                        ImageProduct image = db.ImageProducts.Find(idchk);
                         db.ImageProducts.Remove(image);
                        db.SaveChanges();
                    }
                }
                //Upload file
                if(uploadFiles!=null)
                {
                    var listfile = db.tblFiles.Where(p => p.idp == id).ToList();
                    if(listfile.Count>0)
                    {
                        string fullPath = Request.MapPath("~/" + listfile[0].File);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        db.tblFiles.Remove(listfile[0]);
                        db.SaveChanges();
                    }
                    string filePath = Path.Combine(HttpContext.Server.MapPath("~/Images/files"),Path.GetFileName(uploadFiles.FileName));
                    tblFile tblfile = new tblFile();
                    tblfile.Name = tblproduct.Name + "[" + uploadFiles.FileName + "]";
                    tblfile.File = "/Images/files/" + uploadFiles.FileName + "";
                    tblfile.Cate = 1;
                    tblfile.idp = id;
                    db.tblFiles.Add(tblfile);
                    db.SaveChanges();
                    uploadFiles.SaveAs(filePath);
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
                            ImageProduct imgp = new ImageProduct();
                            imgp.idProduct = id;
                            imgp.Images = "/Images/ImagesList/" + System.IO.Path.GetFileName(item.FileName);
                            db.ImageProducts.Add(imgp);
                            db.SaveChanges();
                        }

                    }
                    TempData["Msg"] = abc + "</br>" + def;
                }
                var listconnect = db.tblConnectFunProuducts.Where(p => p.idPro == id).ToList();
                for (int i = 0; i < listconnect.Count;i++ )
                {
                    int idchk = int.Parse(listconnect[i].id.ToString());
                    tblConnectFunProuduct image = db.tblConnectFunProuducts.Find(idchk);
                    db.tblConnectFunProuducts.Remove(image);
                    db.SaveChanges();
                }
                    if (collection["btnAdd"] != null)
                    {
                        foreach (string key in Request.Form)
                        {
                            var checkbox = "";
                            if (key.StartsWith("chkFun+"))
                            {
                                checkbox = Request.Form["" + key];
                                if (checkbox != "false")
                                {
                                    Int32 idkey = Convert.ToInt32(key.Remove(0, 7));
                                    tblConnectFunProuduct connectionfunction = new tblConnectFunProuduct();
                                    connectionfunction.idFunc = idkey;
                                    connectionfunction.idPro = id;
                                    db.tblConnectFunProuducts.Add(connectionfunction);
                                    db.SaveChanges();

                                }
                            }
                        }
                    }
                    var ListConnectcolor = db.tblConnectColorProducts.Where(p => p.idPro == id).ToList();
                    for (int i = 0; i < ListConnectcolor.Count; i++)
                    {
                        int idchk = int.Parse(ListConnectcolor[i].id.ToString());
                        tblConnectColorProduct image = db.tblConnectColorProducts.Find(idchk);
                        db.tblConnectColorProducts.Remove(image);
                        db.SaveChanges();
                    }
                    if (collection["btnAdd"] != null)
                    {
                        foreach (string key in Request.Form)
                        {
                            var checkbox = "";
                            if (key.StartsWith("chkCol+"))
                            {
                                checkbox = Request.Form["" + key];
                                if (checkbox != "false")
                                {
                                    Int32 idkey = Convert.ToInt32(key.Remove(0, 7));
                                    tblConnectColorProduct connectionfunction = new tblConnectColorProduct();
                                    connectionfunction.idColor = idkey;
                                    connectionfunction.idPro = id;
                                    db.tblConnectColorProducts.Add(connectionfunction);
                                    db.SaveChanges();

                                }
                            }
                        }
                    }
                return Redirect("/Productad/Index?idCate=" + int.Parse(collection["drMenu"]));
            }
            return View(tblproduct);
        }
        public ActionResult DeleteConfirmed(int id)
        {
            tblProduct tblproduct = db.tblProducts.Find(id);
            clsSitemap.DeteleSitemap(id.ToString(), "Product");
            db.tblProducts.Remove(tblproduct);
            db.SaveChanges();
            var listImage = db.ImageProducts.Where(p => p.idProduct == id).ToList();
               for(int i=0;i<listImage.Count;i++)
               {
                     db.ImageProducts.Remove(listImage[i]);
                    db.SaveChanges();
               }
               var listconnect = db.tblConnectFunProuducts.Where(p => p.idPro == id).ToList();
               for (int i = 0; i < listconnect.Count; i++)
               {
                   int idchk = int.Parse(listconnect[i].id.ToString());
                   tblConnectFunProuduct image = db.tblConnectFunProuducts.Find(idchk);
                   db.tblConnectFunProuducts.Remove(image);
                   db.SaveChanges();
               }
               var ListConnectcolor = db.tblConnectColorProducts.Where(p => p.idPro == id).ToList();
               for (int i = 0; i < ListConnectcolor.Count; i++)
               {
                   int idchk = int.Parse(ListConnectcolor[i].id.ToString());
                   tblConnectColorProduct image = db.tblConnectColorProducts.Find(idchk);
                   db.tblConnectColorProducts.Remove(image);
                   db.SaveChanges();
               }
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Delete Product", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        [HttpPost]
        public ActionResult ProductEditOrd(int txtSort, string ts)
        {
            var Product = db.tblProducts.Find(txtSort);
            var result = string.Empty;
            Product.Ord = int.Parse(ts);
            //db.Entry(Product).State = System.Data.EntityState.Modified;
            result = "Ord Updated.";
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Edit Ord Product", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            return Json(new { result = result });
        }
        public ActionResult UpdateProduct(string Code,string Price,string PriceSale,string Active)
        {
            int nPrice = int.Parse(Price);
            int nPriceSale = int.Parse(PriceSale);
            bool nActive = bool.Parse(Active);
            var tblproduct = db.tblProducts.First(p => p.Code == Code);
            tblproduct.Price = nPrice;
            tblproduct.PriceSale = nPriceSale;
            tblproduct.Active =nActive;
            db.SaveChanges();
            var result = string.Empty;
            return Json(new { result = result });
        }
        [HttpPost]
         #region[Delete]
        public ActionResult DeleteProduct(int id)
        {
            tblProduct tblproduct = db.tblProducts.Find(id);
            clsSitemap.DeteleSitemap(id.ToString(), "Product");
            var listconnect = db.tblConnectFunProuducts.Where(p => p.idPro == id).ToList();
            for (int i = 0; i < listconnect.Count; i++)
            {
                int idchk = int.Parse(listconnect[i].id.ToString());
                tblConnectFunProuduct image = db.tblConnectFunProuducts.Find(idchk);
                db.tblConnectFunProuducts.Remove(image);
                db.SaveChanges();
            }
            var result = string.Empty;
            db.tblProducts.Remove(tblproduct);
            db.SaveChanges();
            var listImage = db.ImageProducts.Where(p => p.idProduct == id).ToList();
            for (int i = 0; i < listImage.Count; i++)
            {
                db.ImageProducts.Remove(listImage[i]);
                db.SaveChanges();
            }
            var ListConnectcolor = db.tblConnectColorProducts.Where(p => p.idPro == id).ToList();
            for (int i = 0; i < ListConnectcolor.Count; i++)
            {
                int idchk = int.Parse(ListConnectcolor[i].id.ToString());
                tblConnectColorProduct image = db.tblConnectColorProducts.Find(idchk);
                db.tblConnectColorProducts.Remove(image);
                db.SaveChanges();
            }
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Delete Product", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            result = "Bạn đã xóa thành công.";
            return Json(new { result = result });

        }
        [HttpPost]
        public string CheckValue(string text)
        {
            string chuoi = "";
            var listProduct = db.tblProducts.Where(p => p.Name == text).ToList();
            if (listProduct.Count > 0)
            {
                chuoi = "Duplicate Name !";

            }
            Session["Check"] = listProduct.Count;
            return chuoi;
        }
        public ActionResult Command(FormCollection collection)
        {
            if (collection["btnSearch"] != null)
            {
                return Redirect("/Productad/index?text=" + collection["txtSearch"] + "");
            }
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
                            clsSitemap.DeteleSitemap(id.ToString(), "Product");

                            var Del = (from emp in db.tblProducts where emp.id == id select emp).SingleOrDefault();
                            db.tblProducts.Remove(Del);
                            db.SaveChanges();
                            var listImage = db.ImageProducts.Where(p => p.idProduct == id).ToList();
                            for (int i = 0; i < listImage.Count; i++)
                            {
                                db.ImageProducts.Remove(listImage[i]);
                                db.SaveChanges();
                            }
                            var listconnect = db.tblConnectFunProuducts.Where(p => p.idPro == id).ToList();
                            for (int i = 0; i < listconnect.Count; i++)
                            {
                                int idchk = int.Parse(listconnect[i].id.ToString());
                                tblConnectFunProuduct image = db.tblConnectFunProuducts.Find(idchk);
                                db.tblConnectFunProuducts.Remove(image);
                                db.SaveChanges();
                            }
                            var ListConnectcolor = db.tblConnectColorProducts.Where(p => p.idPro == id).ToList();
                            for (int i = 0; i < ListConnectcolor.Count; i++)
                            {
                                int idchk = int.Parse(ListConnectcolor[i].id.ToString());
                                tblConnectColorProduct image = db.tblConnectColorProducts.Find(idchk);
                                db.tblConnectColorProducts.Remove(image);
                                db.SaveChanges();
                            }
                            #region[Updatehistory]
                            Updatehistoty.UpdateHistory("Delete Product", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
                            #endregion
                        }
                    }
                }
                return RedirectToAction("Index");
            }
         
            if (collection["btnExport"] != null)
            {
                #region[Updatehistory]
                Updatehistoty.UpdateHistory("Export  Product", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
                #endregion
                GridView gv = new GridView();

                var listid = 0;
                List<int> exceptionList = new List<int>();
                foreach (string key in Request.Form.Keys)
                {
                    var checkbox = "";
                    if (key.StartsWith("chkitem+"))
                    {
                        checkbox = Request.Form["" + key];
                        if (checkbox != "false")
                        {
                            int id = Convert.ToInt32(key.Remove(0, 8));
                            exceptionList.Add(id);
                        }
                    }
                }
                gv.DataSource = db.tblProducts.Where(x => exceptionList.Contains(x.id)).ToList();
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=Marklist.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                htw.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();

                return RedirectToAction("Index");
            }
            if (collection["btlPrinter"] != null)
            {


                List<int> exceptionList = new List<int>();
                foreach (string key in Request.Form)
                {
                    var checkbox = "";

                    if (key.StartsWith("chkitem+"))
                    {
                        checkbox = Request.Form["" + key];
                        if (checkbox != "false")
                        {
                            int idp = int.Parse(key.Remove(0, 8));
                            exceptionList.Add(idp);

                        }
                    }
                }
                var list = db.tblProducts.Where(x => exceptionList.Contains(x.id)).ToList();
                return View("Printer", list);
            }
            return View();
        }
        [HttpPost]
        public ActionResult print(FormCollection a)
        {
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Printer Product", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            string chuoi = "";
            chuoi = "<script type=\"text/javascript\">$(document).ready(function() {window.print();});</script>";
            ViewBag.Print = chuoi;
            return View("Printer");
        }

        #endregion
        #region[Search]
        public ActionResult Search(string Name, string idCate)
        {
            if (Name != null || idCate != null)
            {
                Session["txtSearch"] = Name;
                Session["idCate"] = idCate;

            }
            return RedirectToAction("Index");
        }
        #endregion
        #region[Export]
        [HttpPost]
        public ActionResult ExportData(FormCollection collection)
        {
            GridView gv = new GridView();

            var listid = 0;
            List<int> exceptionList = new List<int>();
            foreach (string key in Request.Form.Keys)
            {
                var checkbox = "";
                if (key.StartsWith("chkitem+"))
                {
                    checkbox = Request.Form["" + key];
                    if (checkbox != "false")
                    {
                        int id = Convert.ToInt32(key.Remove(0, 8));
                        exceptionList.Add(id);
                    }
                }
            }


            gv.DataSource = db.tblProducts.Where(x => exceptionList.Contains(x.id)).ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Marklist.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            htw.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Export Excel Product", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            return View();
        }
        #endregion
        public ActionResult ShowerroInport()
        {
            string chuoi = "";
            //string kiemtra = Session["nid"].ToString();
            if ((Session["nid"] != null) && (Session["nid"] != ""))
            {
                string mang=Session["nid"].ToString();
                string[] Mang = mang.Split('-');
                int leght = Mang.Length-1;
                ViewBag.leght ="Đang có "+ leght+" Sảm sản phẩm bị lỗi ở dưới đây, bạn nhập giá thủ công cho từng sản phẩm hoặc sửa lại file excel";
                for (int i = 0; i < leght; i++)
                {
                     int id = int.Parse(Mang[i].ToString());
                    var Product = db.tblProducts.Find(id);
                    chuoi += "<div class=\"clo_1\"><span style=\"text-align:center; width:10%\">" + (i + 1) + "</span></div>";
                    chuoi+="<div class=\"clo_2\" style=\"width: 80%\"><a href=\"/Productad/Edit?id="+Product.id+"\" target=\"_blank\" title=\"@item.Name\">"+Product.Name+"</a> </div>";
                    chuoi+="<br/>";
                }

                ViewBag.chuoi = chuoi;
                int countnull = int.Parse(Session["CountNULL"].ToString());
                string ncode=Session["Null"].ToString();
                if (countnull>0)
                {
                    ViewBag.Erro = "Hiện tại có " + countnull + " mã sản phẩm có trong bảng exlcel mà không có trên web, các mã đó cụ thể là: " + ncode + ", bạn vui lòng nhập mã còn lại lên website hoặc kiểm tra lại mã trong bảng excel xem đúng không !";

                }
                Session["CountNULL"] = "";
                Session["Null"] = null;
                Session["nid"] = null;
            }
            else
            {
                return RedirectToAction("Index");

            }
            return View();
        }
        public ActionResult Upload(HttpPostedFileBase uploadFile)
        {
            if (ModelState.IsValid)
            {
                string filePath = Path.Combine(HttpContext.Server.MapPath("/Uploads/demo"),
                                               Path.GetFileName(uploadFile.FileName));
                uploadFile.SaveAs(filePath);
                TempData["Msg"] = string.Format("Upload file {0} thành công", uploadFile.FileName);
            }
            return RedirectToAction("Index");
        }
        public ActionResult ProductHiden(int? page, string text, string idCate)
        {
            if ((Request.Cookies["Username"] == null))
            {
                return RedirectToAction("LoginIndex", "Login");
            }
            string txtSearch = "";
            var listProduct = db.tblProducts.Where(p=>p.Active==false).OrderByDescending(p => p.DateCreate ).ToList();



            if (txtSearch != null && txtSearch != "")
            {
                listProduct = db.tblProducts.Where(p => p.Name.Contains(txtSearch) && p.Active == false).ToList();
            }
            Session["txtSearch"] = null;
            const int pageSize = 500;
            var pageNumber = (page ?? 1);
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
        public ActionResult Export()
        {

            try
            {
                Excel._Application application = new Excel.Application();
                Excel.Workbook workbook = application.Workbooks.Add(System.Reflection.Missing.Value);
                Excel.Worksheet worksheet = workbook.ActiveSheet;
  
                worksheet.Cells[1, 1] = "Code";
                worksheet.Cells[1, 2] = "Price";
                worksheet.Cells[1, 3] = "PriceSale";
            
                var listproduct = db.tblProducts.ToList();
                int row = 2;
                int stt = 0;
                foreach (var product in listproduct)
                {
                    worksheet.Cells[row, 1] = product.Code;
                    worksheet.Cells[row, 2] = product.Price;
                    worksheet.Cells[row, 3] = product.PriceSale;
                      row++; stt++;
                }
                int col = int.Parse(listproduct.Count.ToString());
                // Dinh dang dong, cot
                worksheet.get_Range("A1", "C1").EntireColumn.AutoFit();
                var range_heading = worksheet.get_Range("A1", "C1");
                range_heading.Font.Bold = true;
                range_heading.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range_heading.Font.Color = System.Drawing.Color.Red;
                //Dinh dang tien
                var range_sums = worksheet.get_Range("B2", "B" + (col + 1));
                range_sums.NumberFormat = "#,###,###";
                var range_sums1 = worksheet.get_Range("C2", "C" + (col + 1));
                range_sums1.NumberFormat = "#,###,###";
                 
                workbook.SaveAs("E:\\toitestthu.xls");
                workbook.Close();
                Marshal.ReleaseComObject(workbook);
                application.Quit();
                Marshal.FinalReleaseComObject(application);
                ViewBag.Result = "Thanh cong";

            }
            catch (Exception ex)
            {

                ViewBag.Result = ex.Message;
            }
            return View();
        }
    }
}