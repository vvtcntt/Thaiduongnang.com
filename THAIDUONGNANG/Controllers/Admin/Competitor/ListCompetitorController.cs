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
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using Microsoft.Office.Core;
using System.Data.Entity;
namespace THAIDUONGNANG.Controllers.Admin.Competitor
{
    public class ListCompetitorController : Controller
    {
        //
        // GET: /ListCompetitor/


        private THAIDUONGNANGContext db = new THAIDUONGNANGContext();
        public ActionResult Index(int? page, string id)
        {
            if ((Request.Cookies["Username"] == null))
            {
                return RedirectToAction("LoginIndex", "Login");
            }
            var ListCompetitor = db.tblCompetitorHomes.ToList();

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
            var pro = db.tblCompetitorHomes.OrderByDescending(p => p.Ord).Take(1).ToList();
            if (pro.Count > 0)
                ViewBag.Ord = pro[0].Ord + 1;
            //Load list Competitor
            var listcompetitor = db.tblCompetitors.Where(p => p.Active == true).OrderBy(p => p.Ord).ToList();
            string chuoi = "";
            for (int i = 0; i< listcompetitor.Count;i++ )
            {
               chuoi+="<div style=\"width:100%; height:auto; float:left; margin:1px 0px\">";
               chuoi += "<div style=\"width:15%; float:left; text-align:right\">";
               chuoi += "<span style=\"  margin:2px 10px\">" + listcompetitor[i].Name + " : </span>"; 
                chuoi += "</div>";
                chuoi += "<div style=\"width:84%; float:left\">";
                    chuoi += "<input type=\"text\" name=\"txtCompetitor_" + listcompetitor[i].id + "\" class = \"Leght_2\" id=\"txtCompetitor_" + listcompetitor[i].id + "\" />";
                chuoi+="</div>";
                chuoi += "</div>";
            }
            ViewBag.chuoi = chuoi;
                return View();
        }

        //
        // POST: /Url/Create

        [HttpPost]
        public ActionResult Create(tblCompetitorHome tblcompetitorhome, FormCollection collection)
        {
            tblcompetitorhome.DateCreate = DateTime.Now;
            string idUser = Request.Cookies["Username"].Values["UserID"];
            tblcompetitorhome.idUser = int.Parse(idUser);
            db.tblCompetitorHomes.Add(tblcompetitorhome);
            db.SaveChanges();

            foreach (string key in Request.Form)
            {
                var checkbox = "";
                if (key.StartsWith("txtCompetitor_"))
                {
                    checkbox = Request.Form["" + key];
                    if (checkbox != "false")
                    {
                        Int32 idkey = Convert.ToInt32(key.Remove(0, 14));
                        var listcompe = db.tblCompetitorHomes.OrderByDescending(p => p.id).Take(1).ToList();
                        int idcompe=int.Parse(listcompe[0].id.ToString());
                        tblCompetitorLink tblcompetitorlink = new tblCompetitorLink();
                        tblcompetitorlink.idCompetitor = idkey;
                        tblcompetitorlink.idHomes=idcompe;
                         tblcompetitorlink.idUser = int.Parse(idUser);
                        tblcompetitorlink.Url = collection["txtCompetitor_"+idkey+""];
                        db.tblCompetitorLinks.Add(tblcompetitorlink);
                        db.SaveChanges();

                    }
                }
            }
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Add tblcompetitorlink", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
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
            tblCompetitorHome tblcompetitorhomes = db.tblCompetitorHomes.Find(id);
            if (tblcompetitorhomes == null)
            {
                return HttpNotFound();
            }

            // fill dữ liêu

            var listcompetitor = db.tblCompetitors.Where(p => p.Active == true).OrderBy(p => p.Ord).ToList();
            string chuoi = "";
            for (int i = 0; i < listcompetitor.Count; i++)
            {
                int idcompe = int.Parse(listcompetitor[i].id.ToString());
                var listcompetitorlink = db.tblCompetitorLinks.Where(p => p.idHomes == id && p.idCompetitor == idcompe).ToList();

                chuoi += "<div style=\"width:100%; height:auto; float:left; margin:1px 0px\">";
                chuoi+="<div style=\"width:15%; float:left; text-align:right\">";
                chuoi += "<span style=\"margin:2px 10px\">" + listcompetitor[i].Name + " : </span>";
                chuoi+="</div>";
                chuoi+="<div style=\"width:84%; float:left\">";
                if (listcompetitorlink.Count > 0)
                {
                    chuoi += "<input type=\"text\" name=\"txtCompetitor_" + listcompetitor[i].id + "\" class = \"Leght_2\" id=\"txtCompetitor_" + listcompetitor[i].id + "\" value=\"" + listcompetitorlink[0].Url + "\" />";
                }
                else
                {
                    chuoi += "<input type=\"text\" name=\"txtCompetitor_" + listcompetitor[i].id + "\" class = \"Leght_2\" id=\"txtCompetitor_" + listcompetitor[i].id + "\"   />";
                }
                chuoi += "</div>";
                chuoi += "</div>";
            }
            ViewBag.chuoi = chuoi;
            return View(tblcompetitorhomes);
        }

        //
        // POST: /Url/Edit/5

        [HttpPost]
        public ActionResult Edit(tblCompetitorHome tblcompetitorhome, int id, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                tblcompetitorhome.DateCreate = DateTime.Now;

                string idUser = Request.Cookies["Username"].Values["UserID"];
                tblcompetitorhome.idUser = int.Parse(idUser);
                db.Entry(tblcompetitorhome).State = EntityState.Modified;
                db.SaveChanges();
                #region[Updatehistory]
                Updatehistoty.UpdateHistory("Edit tblcompetitorlink", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
                #endregion

                // Xóa sản phẩm có trong bảng
                var xoa = db.tblCompetitorLinks.Where(p => p.idHomes == id).ToList();
                for (int i = 0; i < xoa.Count;i++ )
                {
                    int idcompe=int.Parse(xoa[i].idCompetitor.ToString());
                    var listcompetitor = db.tblCompetitors.Where(p => p.Active == true && p.id == idcompe).ToList();
                    if(listcompetitor.Count>0)
                    { 
                     db.tblCompetitorLinks.Remove(xoa[i]);
                     db.SaveChanges();
                    }
                }

                foreach (string key in Request.Form)
                {
                    var checkbox = "";
                    if (key.StartsWith("txtCompetitor_"))
                    {
                        checkbox = Request.Form["" + key];
                        if (checkbox != "false")
                        {
                            Int32 idkey = Convert.ToInt32(key.Remove(0, 14));
                            var listcompe = db.tblCompetitorHomes.OrderByDescending(p => p.id).Take(1).ToList();
                            int idcompe = id;
                            tblCompetitorLink tblcompetitorlink = new tblCompetitorLink();
                            tblcompetitorlink.idCompetitor = idkey;
                            tblcompetitorlink.idHomes = idcompe;
                            tblcompetitorlink.idUser = int.Parse(idUser);
                            tblcompetitorlink.Url = collection["txtCompetitor_" + idkey + ""];
                            db.tblCompetitorLinks.Add(tblcompetitorlink);
                            db.SaveChanges();

                        }
                    }
                }
                    return RedirectToAction("Index");
            }
            return View(tblcompetitorhome);
        }

        //
        // GET: /Url/Delete/5

        public ActionResult Delete(int id = 0)
        {
            tblCompetitorLink tblcompetitorlink = db.tblCompetitorLinks.Find(id);
            if (tblcompetitorlink == null)
            {
                return HttpNotFound();
            }
            return View(tblcompetitorlink);
        }

        //
        // POST: /Url/Delete/5


        public ActionResult DeleteConfirmed(int id)
        {
            tblCompetitorHome tblcompetitorlink = db.tblCompetitorHomes.Find(id);
            db.tblCompetitorHomes.Remove(tblcompetitorlink);
            db.SaveChanges();
            var xoa = db.tblCompetitorLinks.Where(p => p.idHomes == id).ToList();
            for (int i = 0; i < xoa.Count; i++)
            {
                db.tblCompetitorLinks.Remove(xoa[i]);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public ActionResult CompetitorLinkEditOrd(int txtSort, string ts)
        {
            var tblcompetitorlink = db.tblCompetitorLinks.Find(txtSort);
            var result = string.Empty;
            tblcompetitorlink.Ord = int.Parse(ts);
            //db.Entry(MenuGroupsProduct).State = System.Data.EntityState.Modified;
            result = "Update Ord.";
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Update Ord tblcompetitorlink", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            return Json(new { result = result });
        }
        [HttpPost]
        public ActionResult CompetitorLinkEditActive(string chk, string nchecked)
        {

            var Url = db.tblCompetitorLinks.Find(int.Parse(chk));
            var result = string.Empty;
            if (nchecked == "true")
            {
                Url.Active = false;
            }
            else
            { Url.Active = true; }
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Update Active tblcompetitorlink", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
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
                            tblCompetitorLink tblcompetitorlink = db.tblCompetitorLinks.Find(id);
                            db.tblCompetitorLinks.Remove(tblcompetitorlink);
                            db.SaveChanges();
                            var xoa = db.tblCompetitorLinks.Where(p => p.idHomes == id).ToList();
                            for (int i = 0; i < xoa.Count; i++)
                            {
                                db.tblCompetitorLinks.Remove(xoa[i]);
                                db.SaveChanges();
                            }
                            #region[Updatehistory]
                            Updatehistoty.UpdateHistory("Delete tblcompetitorlink", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
                            #endregion
                            return Redirect("Index");
                        }
                    }
                }
            }
            return RedirectToAction("Index");


        }
        public ActionResult comparePrice(int id)
        {
            tblCompetitorHome compe=db.tblCompetitorHomes.First(p=>p.id==id);
            var listcompe = db.tblCompetitors.Where(p => p.Active == true).OrderBy(p => p.Ord).ToList();
            string chuoilistcompe = "";
            for (int i = 0; i < listcompe.Count;i++ )
            {
                chuoilistcompe += "<td>"+listcompe[i].Name+"</td>";
             }
            string code=compe.CodeProduct;
            var seabig = db.tblProducts.First(p => p.Code == code);
            string chuoiseabig = "<td>" + code + "</td>";
            chuoiseabig+= "<td>" + seabig.PriceSale + "</td>";
            ViewBag.chuoiseabig = chuoiseabig;
            ViewBag.chuoilistcompe = chuoilistcompe;
            var listcompetitorlink = db.tblCompetitorLinks.Where(p => p.idHomes == id).ToList();
            string chuoihienthigia = "";
            for (int i = 0; i < listcompetitorlink.Count;i++ )
            {
                chuoihienthigia += "<td>";
                var getHtmlWeb = new HtmlWeb();
                var document = getHtmlWeb.Load(listcompetitorlink[i].Url);
                int idcompe=int.Parse(listcompetitorlink[i].idCompetitor.ToString());
                tblCompetitor commpetitor = db.tblCompetitors.First(p => p.id == idcompe);
                var aTags = document.DocumentNode.SelectNodes(""+commpetitor.Code+"");
                int counter = 1;
                string gia;
                
                if (aTags != null)
                {
                    foreach (var aTag in aTags)
                    {
                        gia = aTag.InnerHtml;
                        chuoihienthigia += gia;
                    }
                }
                chuoihienthigia += "</td>";
            }
            ViewBag.chuoihienthi = chuoihienthigia;
                return View();

        }
        bool CheckInt(string String)
        {
            int Int;
            if (Int32.TryParse(String, out Int))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public ActionResult Displaycompetitor(string pass,string Download)
        {
             Excel._Application application = new Excel.Application();
                Excel.Workbook workbook = application.Workbooks.Add(System.Reflection.Missing.Value);
                Excel.Worksheet worksheet = workbook.ActiveSheet;
                worksheet.Cells[3, 1] = "STT";
                worksheet.Cells[3, 2] = "Tên sản phẩm";
                worksheet.Cells[3, 3] = "Mã sản phẩm";
                worksheet.Cells[3, 4] = "Giá SEABIG";
                int col1 = 5;
                int row = 4;
                if ((Request.Cookies["Username"] == null))
                {
                    return RedirectToAction("LoginIndex", "Login");
                }
           
                string Namecompe = "";
                string chuoi = "";
                var listhomeCompe = db.tblCompetitorHomes.Where(p => p.Active == true).OrderBy(p => p.Ord).ToList();
                {
                    var listcompe = db.tblCompetitors.Where(p => p.Active == true).OrderBy(p => p.Ord).ToList();
                    for (int j = 0; j < listcompe.Count; j++)
                    {
                        worksheet.Cells[3, col1 + j] = listcompe[j].Name;
                        Namecompe += "<td class=\"Name_Compe\">" + listcompe[j].Name + "<span class=\"web\">" + listcompe[j].Website + "</span></td>";
                    }
                    ViewBag.Namecompe = Namecompe;
                    for (int i = 0; i < listhomeCompe.Count; i++)
                    {
                        int idHome = int.Parse(listhomeCompe[i].id.ToString());
                        //Hiển thị tên đối thủ
                        chuoi += "<tr class=\"Center_1\">";
                        chuoi += "<td class=\"Ord\">" + i + "</td>";
                        string Codeproduct = listhomeCompe[i].CodeProduct;
                        var nProduct = db.tblProducts.First(p => p.Code == Codeproduct);
                        chuoi += "<td class=\"Name\">" + nProduct.Name + "</td>";
                        chuoi += "<td class=\"Code\">" + nProduct.Code + "</td>";
                        int PriceSeabig = int.Parse(nProduct.PriceSale.ToString());
                        chuoi += " <td class=\"Name_seabig\" style=\"color:#F00\"> " + string.Format("{0:#,#}", PriceSeabig) + " vnđ";
                        chuoi += "<div class=\"alert alert-info\" id=\"update-message\"><span></span></div>";
                        chuoi += "<input type=\"number\" name=\"txtPrice_" + nProduct.id + "\" id=\"" + nProduct.id + "\" class=\"clsPrice\" />";
                        chuoi += "</td>";
                        worksheet.Cells[row, 1] = i;
                        var range_stt = worksheet.Cells[row, 1];
                        range_stt.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter; 
                        worksheet.Cells[row, 2] = nProduct.Name;
                        worksheet.Cells[row, 3] = nProduct.Code;
                        worksheet.Cells[row, 4] = nProduct.PriceSale;
                        var range_giasb = worksheet.Cells[row, 4];
                        range_giasb.Font.Bold = true;
                        range_giasb.NumberFormat = "#,###,###";
                        range_giasb.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter; 
                        var Listlink = db.tblCompetitorLinks.Where(p => p.idHomes == idHome).ToList();
                        for (int j = 0; j < Listlink.Count; j++)
                        {
                            int idcompetitor = int.Parse(Listlink[j].idCompetitor.ToString());
                            var listcompetitor = db.tblCompetitors.Where(p => p.id == idcompetitor && p.Active == true).ToList();
                            if (listcompetitor.Count > 0)
                            {
                                if (Listlink[j].Url != "")
                                {
                                    var getHtmlWeb = new HtmlWeb();
                                    var document = getHtmlWeb.Load(Listlink[j].Url);
                                    int idcompe = int.Parse(Listlink[j].idCompetitor.ToString());
                                    tblCompetitor commpetitor = db.tblCompetitors.First(p => p.id == idcompe);
                                    var aTags = document.DocumentNode.SelectNodes("" + commpetitor.Code + "");
                                    string gia;
                                    string manggia = "";
                                    if (aTags != null)
                                    {

                                        foreach (var aTag in aTags.Take(1))
                                        {

                                            gia = aTag.InnerHtml;
                                            string[] numbers = Regex.Split(gia, @"\D+");
                                            foreach (string value in numbers)
                                            {
                                                if (!string.IsNullOrEmpty(value))
                                                {
                                                    manggia += value;
                                                }
                                            }
                                        }
                                    }
                                    int giacompe;
                                    if (CheckInt(manggia) == true)
                                        giacompe = int.Parse(manggia);
                                    else
                                        giacompe = 1;

                                    if (PriceSeabig > giacompe)
                                    {

                                        if (giacompe == 1)
                                        {
                                            chuoi += " <td class=\"Name_compe\" style=\"background:#ff2400; color:#FFF\"> Chưa có giá ";
                                            worksheet.Cells[row, j + 5] = "0";

                                        }
                                        else
                                        {
                                            worksheet.Cells[row, j + 5]= giacompe;
                                            var range_gia = worksheet.Cells[row, j + 5];
                                             range_gia.Font.Color=System.Drawing.Color.Red;
                                             range_gia.Font.Bold = true;
                                             range_gia.NumberFormat = "#,###,###";
                                             range_gia.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                            chuoi += " <td class=\"Name_compe\" style=\"background:#ff2400; color:#FFF\"> " + string.Format("{0:#,#}", giacompe) + " vnđ";

                                            chuoi += "<a class=\"giamgia\" href=\"\" title=\"" + listhomeCompe[i].CodeProduct + "_" + giacompe + "\"><img src=\"/Content/Display/Icon/icon-giam.png\" alt=\"Giảm\"/>Giảm bằng</a>";
                                        }
                                        chuoi += "</td>";
                                    }
                                    else
                                    {
                                        chuoi += "<td class=\"Name_compe\">" + string.Format("{0:#,#}", giacompe) + " vnđ</td>";
                                        worksheet.Cells[row, j + 5] = giacompe;
                                        var range_gia = worksheet.Cells[row, j + 5];
                                        range_gia.Font.Bold = true;
                                        range_gia.NumberFormat = "#,###,###";
                                        range_gia.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                    }
                                }
                                else
                                {
                                    worksheet.Cells[row, j + 5] = "0";

                                    chuoi += "<td class=\"Name_compe\"> Chưa cập nhật</td>";

                                }
                            }

                        }
                        var listchk = db.tblCompetitors.ToList();
                        int checkcot = listcompe.Count - Listlink.Count;
                        if (checkcot > 0)
                        {
                            for (int u = 0; u < checkcot; u++)
                            {
                                chuoi += "<td class=\"Name_compe\"> Chưa cập nhật</td>";
                                worksheet.Cells[row, u + 5] = "0";

                            }
                        }
                        chuoi += " </tr>";
                        row++;
                    }
                }
                ViewBag.chuoi = chuoi;
                if(Download!=null)
                { 
                workbook.SaveAs("E:\\Banggia.xls");
                workbook.Close();
                Marshal.ReleaseComObject(workbook);
                application.Quit();
                Marshal.FinalReleaseComObject(application);
                }
            
          
            return View(db.tblConfigs.First());
        }
        public ActionResult UpdatePrice(string Code)
        {
            string[] chuoi = Code.Split('_');
            string Codes = chuoi[0];
            int Price = int.Parse(chuoi[1]);
            var Product = db.tblProducts.First(p => p.Code == Codes);
            Product.PriceSale = Price;
            var result = string.Empty;
             db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Update Active tblcompetitorlink", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            result = "Thay đổi giá thành công";
            return Json(new { result = result });
        }
        public ActionResult UpdatePriceText(string Code,string Price)
        {
            int id = int.Parse(Code);
            var Product = db.tblProducts.First(p => p.id == id);
            Product.PriceSale = int.Parse(Price);
            var result = string.Empty;
            db.SaveChanges();
            #region[Updatehistory]
            Updatehistoty.UpdateHistory("Update Active tblcompetitorlink", Request.Cookies["Username"].Values["FullName"].ToString(), Request.Cookies["Username"].Values["UserID"].ToString());
            #endregion
            result = "Thay đổi giá thành công";
            return Json(new { result = result });
        }
    }
}
