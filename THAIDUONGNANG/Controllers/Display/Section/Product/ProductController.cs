using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THAIDUONGNANG.Models;
namespace THAIDUONGNANG.Controllers.Display.Section.Product
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/
        THAIDUONGNANGContext db = new THAIDUONGNANGContext();
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult ListMenuProductHomes()
        {
            var ListMenu = db.tblGroupProducts.Where(p => p.Active == true && p.Priority == true).OrderBy(p => p.Ord).ToList();
            string chuoi = "";
            for (int i = 0; i < ListMenu.Count; i++)
            {
                int id = int.Parse(ListMenu[i].id.ToString());
                chuoi += " <div class=\"Tear_Product\">";
                chuoi += " <div class=\"nVar_Product\">";

                chuoi += "  <div class=\"Nanme_Product\">";
                chuoi += "  <div class=\"Left_NameProduct\"><span>" + (i + 1) + "</span></div>";
                chuoi += "  <div class=\"Right_NameProduct\">";
                chuoi += "   <h2><a href=\"/0/"+ListMenu[i].Tag+"\" title=\""+ListMenu[i].Name+"\">" + ListMenu[i].Name + "</a></h2>";
                chuoi += "   </div>";
                chuoi += "</div>";


                chuoi += "  <hr class=\"hrpd\" />";
                chuoi += "  </div>";

                chuoi += "  <div class=\"Content_TearProduct\">";
                chuoi += "  <div class=\"Adw_Product\">";
                var listAdw = db.tblImages.Where(p => p.Active == true && p.idMenu == 3 && p.idCate == id).OrderByDescending(p => p.Ord).ToList();
                for (int j = 0; j < listAdw.Count; j++)
                {
                    chuoi += "  <a href=\"" + listAdw[j].Url + "\" title=\"" + listAdw[j].Name + "\"><img src=\"" + listAdw[j].Images + "\" alt=\"" + listAdw[j].Name + "\" /></a>";
                }
                chuoi += "  </div>";
                chuoi += " <div class=\"Content_Product\">";
                var listProduct = db.tblProducts.Where(p => p.Active == true && p.ViewHomes == true && p.idCate == id).OrderBy(p => p.Ord).Take(6).ToList();
                for (int j = 0; j < listProduct.Count; j++)
                {

                    chuoi += "<div class=\"Tear_1\">";
                     chuoi += "<div class=\"Top\">";
                    chuoi += "<div class=\"lit\"><span>" + listProduct[j].Dungtich + "L</span></div>";
                    chuoi += " <div class=\"Img\"><a href=\"/" + listProduct[j].Tag + "-mua-san-pham.html\" title=\"" + listProduct[j].Name + "\"><img src=\"" + listProduct[j].ImageLinkThumb + "\" alt=\"" + listProduct[j].Name + "\" /></a></div>";
                    chuoi += "</div>";
                    chuoi += " <div class=\"Info\">";
                    chuoi += "<h3><a href=\"/" + listProduct[j].Tag + "-mua-san-pham.html\" title=\"" + listProduct[j].Name + "\" class=\"Name\">" + listProduct[j].Name + "</a></h3>";
                    chuoi += "<span class=\"ph\">Sản phẩm phù hợp cho : <span>" + listProduct[j].SoNguoiSuDung.ToString() + " người</span></span>";
                    chuoi += " <div class=\"box_Price\">";
                    chuoi += "<div class=\"Left_box_Price\">";
                    chuoi += "<span class=\"Price_ghichu\"> Giá niêm yết</span>";
                    chuoi += "<span class=\"Price\">" + string.Format("{0:#,#}", listProduct[j].Price) + "đ</span>";
                    chuoi += " </div>";
                    chuoi += "<div class=\"Right_box_Price\">";
                    chuoi += "<span class=\"PriceSale_ghichu\">Giá khuyến mại</span>";
                    chuoi += " <span class=\"PriceSale\"> " + string.Format("{0:#,#}", listProduct[j].PriceSale) + "đ  </span>";
                    chuoi += "    </div>";
                    chuoi += " </div>";
                    chuoi += " </div>";
                    chuoi += " </div>";
                }
                chuoi += " </div>";
                chuoi += "</div>";
                chuoi += " </div>";
                //chuoi += "</div>";
            }
            ViewBag.chuoi = chuoi;
            return PartialView();
        }
        public ActionResult ListProduct(string tag)
        {
            string chuoi = "";
            var GroupProduct = db.tblGroupProducts.First(p => p.Tag == tag);
            ViewBag.Title = "<title>" + GroupProduct.Title + "</title>";
            ViewBag.dcTitle = "<meta name=\"DC.title\" content=\"" + GroupProduct.Title + "\" />";
            ViewBag.dcDescription = "<meta name=\"DC.description\" content=\"" + GroupProduct.Description + "\" />";
            ViewBag.Description = "<meta name=\"description\" content=\"" + GroupProduct.Description + "\"/>";
            ViewBag.Keyword = "<meta name=\"keywords\" content=\"" + GroupProduct.Keyword + "\" /> ";
            string level = GroupProduct.Level;
            var ListMenu = db.tblGroupProducts.Where(p => p.Active == true && p.Level.Substring(0, level.Length) == level && p.Level.Length > level.Length).OrderBy(p => p.Ord).ToList();
            if (ListMenu.Count > 0)
            {
                chuoi += " <div class=\"Nvar_h1\">";
                chuoi += " <span></span>";
                chuoi += "<span>" + GroupProduct.Name + "</span>";

                chuoi += "</div>";
                chuoi += "<div class=\"Des\">";
                chuoi += " <div class=\"Content_des\">" + GroupProduct.Content + "</div>";
                chuoi += "</div>";
                 chuoi += "<div id=\"Sale\">";
                 var listAdw = db.tblImages.Where(p => p.idMenu == 6 && p.Active==true).OrderByDescending(p => p.Ord).Take(1).ToList();
                  chuoi += "<a href=\"" + listAdw[0].Url + "\" title=\"" + listAdw[0].Name + "\">";
                 chuoi += "<img src=\"" + listAdw[0].Images + "\" alt=\"" + listAdw[0].Name + "\" title=\"" + listAdw[0].Name + "\"/>";
                 chuoi += " </a>";
        chuoi += "</div>";
                chuoi += " <div class=\"Clear\"></div>";
                for (int i = 0; i < ListMenu.Count; i++)
                {

                    chuoi += "<div class=\"Tear_Product\">";
                    chuoi += " <div class=\"nVar_Product\">";
                    chuoi += "<div class=\"Nanme_Product\">";
                    chuoi += "  <div class=\"Left_NameProduct\"><span>" + i + 1 + "</span></div>";
                    chuoi += "  <div class=\"Right_NameProduct\">";
                    chuoi += " <h2><a href=\"/0/"+ListMenu[i].Tag+"\" title=\""+ListMenu[i].Name+"\">" + ListMenu[i].Name + "</a></h2>";
                    chuoi += " </div>";
                    chuoi += "</div>";
                    chuoi += " <hr class=\"hrpd\" />";
                    chuoi += "</div>";

                    chuoi += "<div class=\"Content_TearProduct ListProducts\">";
                    int id = int.Parse(ListMenu[i].id.ToString());
                    //chuoi += "<div class=\"Adw_Product\">";
                    //var listAdw = db.tblImages.Where(p => p.Active == true && p.idCate == 3 && p.idMenu == id).OrderByDescending(p => p.Ord).ToList();
                    //for (int j = 0; j < listAdw.Count; j++)
                    //{
                    //    chuoi += "  <a href=\"" + listAdw[j].Url + "\" title=\"" + listAdw[j].Name + "\"><img src=\"" + listAdw[j].Images + "\" alt=\"" + listAdw[j].Name + "\" /></a>";
                    //}

                    //chuoi += "</div>";
                    chuoi += "<div class=\"Content_Product\">";
                    var listProduct = db.tblProducts.Where(p => p.Active == true && p.idCate == id).OrderBy(p => p.Ord).ToList();
                    for (int j = 0; j < listProduct.Count; j++)
                    {
                        chuoi += "<div class=\"Tear_1\">";
                         chuoi += "<div class=\"Top\">";
                        chuoi += "<div class=\"lit\"><span>" + listProduct[j].Dungtich + "L</span></div>";
                        chuoi += " <div class=\"Img\"><a href=\"/" + listProduct[j].Tag + "-mua-san-pham.html\" title=\"" + listProduct[j].Name + "\"><img src=\"" + listProduct[j].ImageLinkThumb + "\" alt=\"" + listProduct[j].Name + "\" /></a></div>";
                        chuoi += "</div>";
                        chuoi += " <div class=\"Info\">";
                        chuoi += "<h3><a href=\"/" + listProduct[j].Tag + "-mua-san-pham.html\" title=\"" + listProduct[j].Name + "\" class=\"Name\">" + listProduct[j].Name + "</a></h3>";
                        chuoi += "<span class=\"ph\">Sản phẩm phù hợp cho : <span>" + listProduct[j].SoNguoiSuDung + " người</span></span>";
                        chuoi += " <div class=\"box_Price\">";
                        chuoi += "<div class=\"Left_box_Price\">";
                        chuoi += "<span class=\"Price_ghichu\"> Giá niêm yết</span>";
                        chuoi += "<span class=\"Price\">" + string.Format("{0:#,#}", listProduct[j].Price) + "đ</span>";
                        chuoi += " </div>";
                        chuoi += "<div class=\"Right_box_Price\">";
                        chuoi += "<span class=\"PriceSale_ghichu\">Giá khuyến mại</span>";
                        chuoi += " <span class=\"PriceSale\"> " + string.Format("{0:#,#}", listProduct[j].PriceSale) + "đ  </span>";
                        chuoi += "    </div>";
                        chuoi += " </div>";
                        chuoi += " </div>";
                        chuoi += " </div>";

                    }

                    chuoi += " </div>";
                    chuoi += " </div>";
                    chuoi += " </div>";
                }
            }
            else
            {
                chuoi += "<div class=\"Tear_Product\">";
                chuoi += " <div class=\"nVar_Product\">";
                chuoi += "<div class=\"Nanme_Product\">";
                chuoi += "  <div class=\"Left_NameProduct\"><span>1</span></div>";
                chuoi += "  <div class=\"Right_NameProduct\">";
                chuoi += " <span>" + GroupProduct.Name + "</span>";
                chuoi += " </div>";
                chuoi += "</div>";
                chuoi += " <hr class=\"hrpd\" />";
                chuoi += "</div>";
                chuoi += "<div class=\"Des\">";
                chuoi += " <div class=\"Content_des\">" + GroupProduct.Content + "</div>";
                chuoi += "</div>";
                chuoi += "<div id=\"Sale\">";
                var listAdw = db.tblImages.Where(p => p.idMenu == 6 && p.Active == true).OrderByDescending(p => p.Ord).Take(1).ToList();
                chuoi += "<a href=\"" + listAdw[0].Url + "\" title=\"" + listAdw[0].Name + "\">";
                chuoi += "<img src=\"" + listAdw[0].Images + "\" alt=\"" + listAdw[0].Name + "\" title=\"" + listAdw[0].Name + "\"/>";
                chuoi += " </a>";
                chuoi += "</div>";
                chuoi += "<div class=\"Content_TearProduct ListProducts\">";
                int id = int.Parse(GroupProduct.id.ToString());
                //chuoi += "<div class=\"Adw_Product\">";
                //var listAdw = db.tblImages.Where(p => p.Active == true && p.idCate == 3 && p.idMenu == id).OrderByDescending(p => p.Ord).ToList();
                //for (int j = 0; j < listAdw.Count; j++)
                //{
                //    chuoi += "  <a href=\"" + listAdw[j].Url + "\" title=\"" + listAdw[j].Name + "\"><img src=\"" + listAdw[j].Images + "\" alt=\"" + listAdw[j].Name + "\" /></a>";
                //}

                //chuoi += "</div>";
                chuoi += "<div class=\"Content_Product\">";
                var listProduct = db.tblProducts.Where(p => p.Active == true && p.idCate == id).OrderBy(p => p.Ord).ToList();
                for (int j = 0; j < listProduct.Count; j++)
                {
                    chuoi += "<div class=\"Tear_1\">";
                     chuoi += "<div class=\"Top\">";
                    chuoi += "<div class=\"lit\"><span>" + listProduct[j].Dungtich + "L</span></div>";
                    chuoi += " <div class=\"Img\"><a href=\"/" + listProduct[j].Tag + "-mua-san-pham.html\" title=\"" + listProduct[j].Name + "\"><img src=\"" + listProduct[j].ImageLinkThumb + "\" alt=\"" + listProduct[j].Name + "\" /></a></div>";
                    chuoi += "</div>";
                    chuoi += " <div class=\"Info\">";
                    chuoi += "<h3><a href=\"/" + listProduct[j].Tag + "-mua-san-pham.html\" title=\"" + listProduct[j].Name + "\" class=\"Name\">" + listProduct[j].Name + "</a></h3>";
                    chuoi += "<span class=\"ph\">Sản phẩm phù hợp cho : <span>" + listProduct[j].SoNguoiSuDung + " người</span></span>";
                    chuoi += " <div class=\"box_Price\">";
                    chuoi += "<div class=\"Left_box_Price\">";
                    chuoi += "<span class=\"Price_ghichu\"> Giá niêm yết</span>";
                    chuoi += "<span class=\"Price\">" + string.Format("{0:#,#}", listProduct[j].Price) + "đ</span>";
                    chuoi += " </div>";
                    chuoi += "<div class=\"Right_box_Price\">";
                    chuoi += "<span class=\"PriceSale_ghichu\">Giá khuyến mại</span>";
                    chuoi += " <span class=\"PriceSale\"> " + string.Format("{0:#,#}", listProduct[j].PriceSale) + "đ  </span>";
                    chuoi += "    </div>";
                    chuoi += " </div>";
                    chuoi += " </div>";
                    chuoi += " </div>";

                }

                chuoi += " </div>";
                chuoi += " </div>";
                chuoi += " </div>";

            }
            ViewBag.chuoi = chuoi;
            string nUrl = "";
            int dodai = GroupProduct.Level.Length / 5;
            for (int i = 0; i < dodai; i++)
            {
                var NameGroups = db.tblGroupProducts.First(p => p.Level.Substring(0, (i + 1) * 5) == GroupProduct.Level.Substring(0, (i + 1) * 5) && p.Level.Length == (i + 1) * 5);
                string id = NameGroups.id.ToString();
                string ntaga = NameGroups.Tag;
                string levals = GroupProduct.Level.Substring(0, (i + 1) * 5);
                nUrl = nUrl + " <a href=\"/0/" + ntaga + "\" title=\"" + NameGroups.Name + "\"> " + " " + NameGroups.Name + "</a> /";
            }
            ViewBag.nUrl = "<a href=\"/\" title=\"Trang chủ\" rel=\"nofollow\"><span class=\"iCon\"></span> Trang chủ</a> /" + nUrl +"/<h1>"+GroupProduct.Title+"</h1>";
            return View();
        }
         public ActionResult Tuvan()
        {
            ViewBag.Title = "<title> Tư vấn mua thái dương năng</title>";
            ViewBag.dcTitle = "<meta name=\"DC.title\" content=\"Tư vấn mua thái dương năng\" />";
            ViewBag.dcDescription = "<meta name=\"DC.description\" content=\"Cách mua thái dương năng dành cho bạn và gia đình thông qua hệ thống tư vấn mua tự động của chúng tôi\" />";
            ViewBag.Description = "<meta name=\"description\" content=\"Cách mua thái dương năng dành cho bạn và gia đình thông qua hệ thống tư vấn mua tự động của chúng tôi\"/>";
            ViewBag.Keyword = "<meta name=\"keywords\" content=\"Tư vấn mua thái dương năng chính hãng dành cho bạn\" /> ";
            int Ord1;
            int Ord2;
            if (Session["txtOrd1"] != null)
                Ord1 = int.Parse(Session["txtOrd1"].ToString());
            else
                Ord1 = 0;
            if (Session["txtOrd2"] != null)
                Ord2 = int.Parse(Session["txtOrd2"].ToString());
            else
            {
                if (Ord1 > 0)
                {
                    Ord2 = Ord1;
                }
                else
                { Ord2 = 0; }
            }
            string chuoi = "";
            chuoi += "<div class=\"Tear_Product\">";
            chuoi += " <div class=\"nVar_Product\">";
            chuoi += "<div class=\"Nanme_Product\">";
            chuoi += "  <div class=\"Left_NameProduct\"><span>1</span></div>";
            chuoi += "  <div class=\"Right_NameProduct\">";
            chuoi += " <h1>Tư vấn mua Thái Dương Năng</h1>";
            chuoi += " </div>";
            chuoi += "</div>";
            
            chuoi += " <hr class=\"hrpd\" />";
            chuoi += "</div>";
            chuoi += "<div class=\"Des\">";
            chuoi += " <div class=\"Content_des\"> Những sản phẩm thái dương năng phù hợp cho gia đình có từ " + Ord1 + " đến " + Ord2 + " thành viên mà bạn nên mua là : </div>";
            chuoi += "</div>";
         
            chuoi += "<div class=\"Content_TearProduct ListProducts\">";
  
            chuoi += "<div class=\"Content_Product\">";
              
            var listProduct = db.tblProducts.Where(p => p.Active == true && p.SoNguoiSuDung <= Ord2 && p.SoNguoiSuDung >= Ord1).OrderBy(p => p.Ord).ToList();
            for (int j = 0; j < listProduct.Count; j++)
            {
                chuoi += "<div class=\"Tear_1\">";
                chuoi += "<div class=\"Top\">";
                chuoi += "<div class=\"lit\"><span>" + listProduct[j].Dungtich + "L</span></div>";
                chuoi += " <div class=\"Img\"><a href=\"/" + listProduct[j].Tag + "-mua-san-pham.html\" title=\"" + listProduct[j].Name + "\"><img src=\"" + listProduct[j].ImageLinkThumb + "\" alt=\"" + listProduct[j].Name + "\" /></a></div>";
                chuoi += "</div>";
                chuoi += " <div class=\"Info\">";
                chuoi += "<a href=\"/" + listProduct[j].Tag + "-mua-san-pham.html\" title=\"" + listProduct[j].Name + "\" class=\"Name\">" + listProduct[j].Name + "</a>";
                chuoi += "<span class=\"ph\">Sản phẩm phù hợp cho : <span>" + listProduct[j].SoNguoiSuDung + " người</span></span>";
                chuoi += " <div class=\"box_Price\">";
                chuoi += "<div class=\"Left_box_Price\">";
                chuoi += "<span class=\"Price_ghichu\"> Giá niêm yết</span>";
                chuoi += "<span class=\"Price\">" + string.Format("{0:#,#}", listProduct[j].Price) + "đ</span>";
                chuoi += " </div>";
                chuoi += "<div class=\"Right_box_Price\">";
                chuoi += "<span class=\"PriceSale_ghichu\">Giá khuyến mại</span>";
                chuoi += " <span class=\"PriceSale\"> " + string.Format("{0:#,#}", listProduct[j].PriceSale) + "đ  </span>";
                chuoi += "    </div>";
                chuoi += " </div>";
                chuoi += " </div>";
                chuoi += " </div>";

            }

            chuoi += " </div>";
            chuoi += " </div>";
            chuoi += " </div>";
            ViewBag.chuoi = chuoi;
            Session["txtOrd1"] = null;
            Session["txtOrd2"] = null;
            return View();

        }
         public ActionResult TagProduct(string tag)
         {
             ViewBag.Title = "<title>" + tag + "</title>";
             ViewBag.dcTitle = "<meta name=\"DC.title\" content=\"" + tag + "\" />";
             ViewBag.dcDescription = "<meta name=\"DC.description\" content=\"" + tag + "\" />";
             ViewBag.Description = "<meta name=\"description\" content=\"" + tag + "\"/>";
             ViewBag.Keyword = "<meta name=\"keywords\" content=\"" + tag + "\" /> ";
             string chuoi = "";
             chuoi += "<div class=\"Tear_Product\">";
             chuoi += " <div class=\"nVar_Product\">";
             chuoi += "<div class=\"Nanme_Product\">";
             chuoi += "  <div class=\"Left_NameProduct\"><span>1</span></div>";
             chuoi += "  <div class=\"Right_NameProduct\">";
             chuoi += " <h1>"+tag+"</h1>";
             chuoi += " </div>";
             chuoi += "</div>";
             chuoi += " <hr class=\"hrpd\" />";
             chuoi += "</div>";


             chuoi += "<div class=\"Content_TearProduct ListProducts\">";

             chuoi += "<div class=\"Content_Product\">";
          
             var listProduct = db.tblProducts.Where(p=>p.Keyword.Contains(tag)).OrderBy(p => p.Ord).ToList();
             for (int j = 0; j < listProduct.Count; j++)
             {
                 chuoi += "<div class=\"Tear_1\">";
                 chuoi += "<div class=\"Top\">";
                 chuoi += "<div class=\"lit\"><span>" + listProduct[j].Dungtich + "L</span></div>";
                 chuoi += " <div class=\"Img\"><a href=\"/" + listProduct[j].Tag + "-mua-san-pham.html\" title=\"" + listProduct[j].Name + "\"><img src=\"" + listProduct[j].ImageLinkThumb + "\" alt=\"" + listProduct[j].Name + "\" /></a></div>";
                 chuoi += "</div>";
                 chuoi += " <div class=\"Info\">";
                 chuoi += "<a href=\"/" + listProduct[j].Tag + "-mua-san-pham.html\" title=\"" + listProduct[j].Name + "\" class=\"Name\">" + listProduct[j].Name + "</a>";
                 chuoi += "<span class=\"ph\">Sản phẩm phù hợp cho : <span>" + listProduct[j].SoNguoiSuDung + " người</span></span>";
                 chuoi += " <div class=\"box_Price\">";
                 chuoi += "<div class=\"Left_box_Price\">";
                 chuoi += "<span class=\"Price_ghichu\"> Giá niêm yết</span>";
                 chuoi += "<span class=\"Price\">" + string.Format("{0:#,#}", listProduct[j].Price) + "đ</span>";
                 chuoi += " </div>";
                 chuoi += "<div class=\"Right_box_Price\">";
                 chuoi += "<span class=\"PriceSale_ghichu\">Giá khuyến mại</span>";
                 chuoi += " <span class=\"PriceSale\"> " + string.Format("{0:#,#}", listProduct[j].PriceSale) + "đ  </span>";
                 chuoi += "    </div>";
                 chuoi += " </div>";
                 chuoi += " </div>";
                 chuoi += " </div>";

             }

             chuoi += " </div>";
             chuoi += " </div>";
             chuoi += " </div>";
             ViewBag.chuoi = chuoi;
             return View();

         }
         public ActionResult ProductDetail(string tag)
        {
            tblProduct tblproduct = db.tblProducts.First(p => p.Tag == tag);
            int idcate = int.Parse(tblproduct.idCate.ToString());
            ViewBag.menuname = db.tblGroupProducts.Find(idcate).Name;
            ViewBag.Title = "<title>" + tblproduct.Title + "</title>";
            ViewBag.dcTitle = "<meta name=\"DC.title\" content=\"" + tblproduct.Title + "\" />";
            ViewBag.dcDescription = "<meta name=\"DC.description\" content=\"" + tblproduct.Description + "\" />";
            ViewBag.Description = "<meta name=\"description\" content=\"" + tblproduct.Description + "\"/>";
            ViewBag.Keyword = "<meta name=\"keywords\" content=\"" + tblproduct.Keyword + "\" /> ";
             string chuoitag = "";
             if (tblproduct.Keyword != null)
             {
                 string Chuoi = tblproduct.Keyword;
                 string[] Mang = Chuoi.Split(',');

                 List<int> araylist = new List<int>();
                 for (int i = 0; i < Mang.Length; i++)
                 {
                     chuoitag += "<a href=\"/Tag/" + Mang[i] + "\" title=\"" + Mang[i] + "\">" + Mang[i] + "</a>";
                 }
             }
             ViewBag.chuoitag = chuoitag;
            int visit = int.Parse(tblproduct.Visit.ToString());
            if (visit > 0)
            {
                tblproduct.Visit = tblproduct.Visit + 1;
                db.SaveChanges();
            }
            else
            {
                tblproduct.Visit = tblproduct.Visit + 1;
                db.SaveChanges();
            }
            tblGroupProduct grouproduct = db.tblGroupProducts.Find(idcate);
            ViewBag.video = grouproduct.VideoInfo;
            int dodai = grouproduct.Level.Length / 5;
            string nUrl = "";
            for (int i = 0; i < dodai; i++)
            {
                var NameGroups = db.tblGroupProducts.First(p => p.Level.Substring(0, (i + 1) * 5) == grouproduct.Level.Substring(0, (i + 1) * 5) && p.Level.Length == (i + 1) * 5);
                string ids = NameGroups.id.ToString();
                string levals = grouproduct.Level.Substring(0, (i + 1) * 5);


                nUrl = nUrl + " <a href=\"/0/" + NameGroups.Tag + "\" title=\"" + NameGroups.Name + "\"> " + " " + NameGroups.Name + "</a> /";
            }
            ViewBag.nUrl = "<a href=\"/\" title=\"Trang chủ\" rel=\"nofollow\"><span class=\"iCon\"></span> Trang chủ</a> /" + nUrl +" "+tblproduct.Name;

             //Load sản phẩm khyến mại
            var listAdw = db.tblImages.Where(p => p.idMenu == 6&&p.Active==true).OrderByDescending(p => p.Ord).Take(1).ToList();
            string adw = "";
            if (listAdw.Count > 0)
            {

                adw += "<div class=\"ContentProduct\">";
                adw += "<div class=\"Nvar1\">";
                adw += "<div class=\"Name_one\">";
                adw += "<span class=\"iCon\"></span>";
                adw += "<span class=\"Name\">Chương trình khuyến mại dành cho khách hàng</span>";
                adw += "</div>";
                adw += "</div>";
                adw += "<div class=\"Content_Image\">";
                adw += "<a href=\"" + listAdw[0].Url + "\" title=\"" + listAdw[0].Name + "\"><img src=\"" + listAdw[0].Images + "\" alt=\"" + listAdw[0].Name + "\" title=\"" + listAdw[0].Name + "\" /></a>";
                adw += " </div>";

                adw += "</div>";
            }
            ViewBag.image = adw;
            int ido = int.Parse(tblproduct.LoaiOng.ToString());
            var kiemtra = db.tblOngs.Where(p => p.id == ido).ToList();
            if(kiemtra.Count>0)
            {
                tblOng tblong = db.tblOngs.Find(ido);
                ViewBag.loaiong = "<td>"+ tblong.Name + "</td >";
            }
           
            return View(tblproduct);
        }
        public PartialViewResult PartialVideo(string idCate,string type)
         {
             int idcate = int.Parse(idCate);
             tblGroupProduct tblgroup = db.tblGroupProducts.Find(idcate);
            if(type=="1")
            {
                ViewBag.chuoi="<iframe width=\"100%\" height=\"97%\" src=\"https://www.youtube.com/embed/"+tblgroup.VideoInfo+"?rel=0&amp;controls=0&amp;showinfo=0\" frameborder=\"0\" allowfullscreen></iframe>";
            }
            else
            {
                ViewBag.chuoi = "<iframe width=\"100%\" height=\"97%\" src=\"https://www.youtube.com/embed/" + tblgroup.VideoSetup + "?rel=0&amp;controls=0&amp;showinfo=0\" frameborder=\"0\" allowfullscreen></iframe>";
            }
             return PartialView(tblgroup);
         }
        public PartialViewResult PartialCertificate(string idCate)
        {
            int idcate = int.Parse(idCate);
            tblGroupProduct tblgroup = db.tblGroupProducts.Find(idcate);
            return PartialView(tblgroup);
        }
        public ActionResult Command(FormCollection collection, string tag)
        {
            if (collection["btnOrd"] != null)
            {

                Session["idProduct"] = collection["idPro"];
                Session["OrdProduct"] = "1";
                Session["idMenu"] = collection["idCate"];
                Session["Url"] = Request.Url.ToString();
                return RedirectToAction("OrderIndex", "Order");
            }
            return View();
        }
        public PartialViewResult RightProductDetail(string tag)
        {

            tblProduct Product = db.tblProducts.First(p => p.Tag == tag);
            int id = int.Parse(Product.id.ToString());
            tblConfig tblconfig = db.tblConfigs.First();
            string chuoisupport = "";
            var listSupport = db.tblSupports.Where(p => p.Active == true).OrderBy(p => p.Ord).ToList();
            for (int i = 0; i < listSupport.Count; i++)
            {
                chuoisupport += "<div class=\"Line_Buttom\"></div>";
                chuoisupport += "<div class=\"Tear_Supports\">";
                chuoisupport += "<div class=\"Left_Tear_Support\">";
                chuoisupport += "<span class=\"htv1\">" + listSupport[i].Mission + ":</span>";
                chuoisupport += "<span class=\"htv2\">" + listSupport[i].Name + " :</span>";
                chuoisupport += "</div>";
                chuoisupport += "<div class=\"Right_Tear_Support\">";
                chuoisupport += "<a href=\"ymsgr:sendim?" + listSupport[i].Yahoo + "\">";
                chuoisupport += "<img src=\"http://opi.yahoo.com/online?u=" + listSupport[i].Yahoo + "&m=g&t=1\" alt=\"Yahoo\" class=\"imgYahoo\" />";
                chuoisupport += " </a>";
                chuoisupport += "<a href=\"Skype:" + listSupport[i].Skyper + "?chat\">";
                chuoisupport += "<img class=\"imgSkype\" src=\"/Content/Display/iCon/skype-icon.png\" title=\"Kangaroo\" alt=\"" + listSupport[i].Name + "\">";
                chuoisupport += "</a>";
                chuoisupport += "</div>";
                chuoisupport += "</div>";
            }
            ViewBag.chuoisupport = chuoisupport;

            //lIST Menu
            int idCate = int.Parse(Product.idCate.ToString());
            tblGroupProduct grouproduct = db.tblGroupProducts.Find(idCate);
            string level = grouproduct.Level.ToString();
            int leght = level.Length;
            string chuoimenu = "";
            var listGroupProduct = db.tblGroupProducts.Where(p => p.Level.Substring(0, leght - 5) == level.Substring(0, leght - 5) && p.Active == true && p.Level.Length == level.Length).OrderBy(p => p.Ord).ToList();
            for (int i = 0; i < listGroupProduct.Count; i++)
            {
                string ntag = listGroupProduct[i].Tag;

                chuoimenu += "<a href=\"/0/" + ntag + "\" title=\"" + listGroupProduct[i].Name + "\">› " + listGroupProduct[i].Name + "</a>";

            }
            ViewBag.chuoimenu = chuoimenu;
            //Load sản phẩm liên quan
            string Url = grouproduct.Tag;
            string chuoiproduct = "";
            var listProduct = db.tblProducts.Where(p => p.Active == true && p.idCate == idCate).OrderByDescending(p => p.Visit).OrderBy(p => p.Ord).Take(5).ToList();
            for (int j = 0; j < listProduct.Count; j++)
            {
                chuoiproduct += "<div class=\"Tear_1\">";
                chuoiproduct += "<div class=\"Box_News\"></div>";
                chuoiproduct += "<div class=\"Top\">";
                chuoiproduct += "<div class=\"lit\"><span>" + listProduct[j].Dungtich + "L</span></div>";
                chuoiproduct += " <div class=\"Img\"><a href=\"/" + listProduct[j].Tag + "-mua-san-pham.html\" title=\"" + listProduct[j].Name + "\"><img src=\"" + listProduct[j].ImageLinkThumb + "\" alt=\"" + listProduct[j].Name + "\" /></a></div>";
                chuoiproduct += "</div>";
                chuoiproduct += " <div class=\"Info\">";
                chuoiproduct += "<a href=\"/" + listProduct[j].Tag + "-mua-san-pham.html\" title=\"" + listProduct[j].Name + "\" class=\"Name\">" + listProduct[j].Name + "</a>";
                chuoiproduct += "<span class=\"ph\">Sản phẩm phù hợp cho : <span>" + listProduct[j].SoNguoiSuDung + " người</span></span>";
                chuoiproduct += " <div class=\"box_Price\">";
                chuoiproduct += "<div class=\"Left_box_Price\">";
                chuoiproduct += "<span class=\"Price_ghichu\"> Giá niêm yết</span>";
                chuoiproduct += "<span class=\"Price\">" + string.Format("{0:#,#}", listProduct[j].Price) + "đ</span>";
                chuoiproduct += " </div>";
                chuoiproduct += "<div class=\"Right_box_Price\">";
                chuoiproduct += "<span class=\"PriceSale_ghichu\">Giá khuyến mại</span>";
                chuoiproduct += " <span class=\"PriceSale\"> " + string.Format("{0:#,#}", listProduct[j].PriceSale) + "đ  </span>";
                chuoiproduct += "    </div>";
                chuoiproduct += " </div>";
                chuoiproduct += " </div>";
                chuoiproduct += " </div>";
            }
            ViewBag.chuoiproduct = chuoiproduct;
            return PartialView(db.tblConfigs.First());
        }
    }
}
