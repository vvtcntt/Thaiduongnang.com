using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THAIDUONGNANG.Models;
namespace THAIDUONGNANG.Controllers.Display.Section.Baogia
{
    public class BaogiaController : Controller
    {
        THAIDUONGNANGContext db = new THAIDUONGNANGContext();
        // GET: Baogia
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BaogiaDetail(string tag)
        {
            tblGroupProduct tblgroupproduct = db.tblGroupProducts.First(p => p.Tag == tag);
            ViewBag.Title = "<title> Bảng báo giá " + tblgroupproduct.Name + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year + "</title>";
            ViewBag.Description = "<meta name=\"description\" content=\"Bảng báo giá mới nhất dành cho sản phẩm "+tblgroupproduct.Name+" của trung tâm phân phối thái dương năng Sơn Hà dành cho quý khách hàng\"/>";
            ViewBag.Keyword = "<meta name=\"keywords\" content=\"" + tblgroupproduct.Name + "\" /> ";
            ViewBag.dcTitle = "<meta name=\"DC.title\" content=\"" + tblgroupproduct.Name + "\" />";
            ViewBag.dcDescription = "<meta name=\"DC.description\" content=\"" + tblgroupproduct.Description + "\" />";
            int idCate = int.Parse(tblgroupproduct.id.ToString());
            ViewBag.name = tblgroupproduct.Name;
            var listProduct = db.tblProducts.Where(p => p.Active == true && p.idCate == idCate).OrderBy(p=>p.Ord).ToList();
            string chuoi = "";
            for (int i = 0; i < listProduct.Count;i++ )
            {

                chuoi+="<tr>";
                    chuoi+="<td class=\"Ords\">"+(i+1)+"</td>";
                    chuoi += "<td class=\"Names\"><a href=\"/" + listProduct[i].Tag + "-mua-san-pham.html\" title=\"" + listProduct[i].Name + "\">" + listProduct[i].Name + "</a> </td>";
                    chuoi+="<td class=\"Codes\">"+listProduct[i].Code+"</td>";
                    chuoi += "<td class=\"Prices\">" + string.Format("{0:#,#}", listProduct[i].PriceSale) + "đ</td><td class=\"Qualitys\">01</td><td class=\"SumPrices\">" + string.Format("{0:#,#}", listProduct[i].PriceSale) + "đ</td>";
                    chuoi += " <td class=\"Images\"><a href=\"/" + listProduct[i].Tag + "-mua-san-pham.html\" title=\"" + listProduct[i].Name + "\"><img src=\""+listProduct[i].ImageLinkThumb+"\" alt=\"" + listProduct[i].Name + "\" title=\"" + listProduct[i].Name + "\"></a></td>";
                chuoi+="</tr>";
            }
            ViewBag.chuoi = chuoi;
                return View(db.tblConfigs.First());
        }
    }
}