using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using THAIDUONGNANG.Models;
namespace THAIDUONGNANG.Controllers.Display.Section.Contact
{
    public class ContactController : Controller
    {
        THAIDUONGNANGContext db = new THAIDUONGNANGContext();
        // GET: Contact
        public ActionResult Index()
        {
            ViewBag.Title = "<title> Liên hệ mua thái dương năng</title>";
            ViewBag.Description = "<meta name=\"description\" content=\"Bạn đang liên hệ tới trung tâm phân phối sản phẩm thái dương năng Sơn Hà dành cho kahcsh hàng\"/>";
            ViewBag.Keyword = "<meta name=\"keywords\" content=\"Liên hệ\" /> ";
            if (Session["Status"] != "" && Session["Status"] != null)
            {

                ViewBag.Lienhe = Session["Status"];
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index(tblContact Contact, FormCollection collection)
        {
            tblConfig config = db.tblConfigs.First();
            // Gmail Address from where you send the mail
            var fromAddress = config.UserEmail;
            // any address where the email will be sending
            var toAddress = config.Email;
            //Password of your gmail address
            string fromPassword = config.PassEmail;
            // Passing the values and make a email formate to display
            //string subject = "Bạn có liên hệ mới từ  Thaiduongnang.com: ";
            //string body = "From: " + collection["name"] + "\n";
            string Name = collection["Name"];
            string Address = collection["Address"];
            string Mobile = collection["Mobile"];
            string Email = collection["Email"];
            string Content = collection["Content"];
            //body += "Tên khách hàng: " + collection["Name"] + "\n";
            //body += "Địa chỉ: " + collection["Address"] + "\n";
            //body += "Điện thoại: " + collection["Mobile"] + "\n";
            //body += "Email: " + collection["Email"] + "\n";
            //body += "Nội dung: \n" + collection["Content"] + "\n";
            string ararurl = Request.Url.ToString();
            var listurl = ararurl.Split('/');
            string urlhomes = "";
            for (int i = 0; i < listurl.Length - 2; i++)
            {
                if (i > 0)
                    urlhomes += "/" + listurl[i];
                else
                    urlhomes += listurl[i];
            }
            ////string OrderId = clsConnect.sqlselectOneString("select max(idOrder) as MaxID from [Order]");
            string subject = "Đơn hàng mới từ " + urlhomes + "";
            string chuoihtml = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /><title>Thông tin đơn hàng</title></head><body style=\"background:#f2f2f2; font-family:Arial, Helvetica, sans-serif\"><div style=\"width:750px; height:auto; margin:5px auto; background:#FFF; border-radius:5px; overflow:hidden\">";
            chuoihtml += "<div style=\"width:100%; height:40px; float:left; margin:0px; background:#1c7fc4\"><span style=\"font-size:14px; line-height:40px; color:#FFF; margin:auto 20px; float:left\">" + DateTime.Now.Date + "</span><span style=\"font-size:14px; line-height:40px; float:right; margin:auto 20px; color:#FFF; text-transform:uppercase\">Hotline : " + config.HotlineIN + "</span></div>";
            chuoihtml += "<div style=\"width:100%; height:auto; float:left; margin:0px\"><div style=\"width:35%; height:100%; margin:0px; float:left\"><a href=\"/\" title=\"\"><img src=\"" + urlhomes + "" + config.ImageLinkLogo + "\" alt=\"Logo\" style=\"margin:8px; display:block; max-height:95% \" /></a></div><div style=\"width:60%; height:100%; float:right; margin:0px; text-align:right\"><span style=\"font-size:18px; margin:20px 5px 5px 5px; display:block; color:#ff5a00; text-transform:uppercase\">" + config.Name + "</span><span style=\"display:block; margin:5px; color:#515151; font-size:13px; text-transform:uppercase\">Lớn nhất - Chính hãng - Giá rẻ nhất việt nam</span> </div>  </div>";
            chuoihtml += "<span style=\"text-align:center; margin:10px auto; font-size:20px; color:#000; font-weight:bold; text-transform:uppercase; display:block\">Thông tin đơn hàng</span>";
            chuoihtml += " <div style=\"width:90%; height:auto; margin:10px auto; background:#f2f2f2; padding:15px\">";
            chuoihtml += "<p style=\"font-size:14px; color:#464646; margin:5px 20px\">Đơn hàng từ website : <span style=\"color:#1c7fc4\">" + urlhomes + "</span></p>";
            chuoihtml += "<p style=\"font-size:14px; color:#464646; margin:5px 20px\">Ngày gửi đơn hàng : <span style=\"color:#1c7fc4\">Vào lúc " + DateTime.Now.Hour + " giờ " + DateTime.Now.Minute + " phút ( ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year + ") </span></p>";
          
            chuoihtml += "<div style=\" width:100%; margin:15px 0px\">";
            chuoihtml += "<div style=\"width:99%; height:auto; float:left; margin:0px; border:1px solid #d5d5d5\">";
            chuoihtml += "<div style=\" width:100%; height:30px; float:left; background:#1c7fc4; font-size:12px; color:#FFF; text-indent:15px; line-height:30px\">    	Thông tin người gửi     </div>";
            chuoihtml += "<div style=\"width:100%; height:auto; float:left\">";
            chuoihtml += "<p style=\"font-size:12px; margin:5px 10px\">Họ và tên :<span style=\"font-weight:bold\"> " + Name + "</span></p>";
            chuoihtml += "<p style=\"font-size:12px; margin:5px 10px\">Địa chỉ :<span style=\"font-weight:bold\"> " + Address + "</span></p>";
            chuoihtml += "<p style=\"font-size:12px; margin:5px 10px\">Điện thoại :<span style=\"font-weight:bold\"> " + Mobile + "</span></p>";
            chuoihtml += "<p style=\"font-size:12px; margin:5px 10px\">Email :<span style=\"font-weight:bold\">" + Email + "</span></p>";
            chuoihtml += "</div>";
            chuoihtml += "</div>";
            chuoihtml += "<div style=\"width:99%; height:auto; float:right; margin:0px; border:1px solid #d5d5d5\">";
            chuoihtml += "<div style=\" width:100%; height:30px; float:left; background:#1c7fc4; font-size:12px; color:#FFF; text-indent:15px; line-height:30px\">   	Nội dung     </div>";
            chuoihtml += " <div style=\"width:100%; height:auto; float:left\">";
            chuoihtml += "<p style=\"font-size:12px; margin:5px 10px; font-weight:bold; color:#F00\"> - " + Content + "</p>";
            chuoihtml += "</div>";
            chuoihtml += "</div>";
            chuoihtml += "</div>";
            chuoihtml += "<div style=\"width:100%; height:auto; float:left; margin:0px\">";
            chuoihtml += "<hr style=\"width:80%; height:1px; background:#d8d8d8; margin:20px auto 10px auto\" />";
            chuoihtml += "<p style=\"font-size:12px; text-align:center; margin:5px 5px\">" + config.Address + "</p>";
            chuoihtml += "<p style=\"font-size:12px; text-align:center; margin:5px 5px\">Điện thoại : " + config.MobileIN + " - " + config.HotlineIN + "</p>";
            chuoihtml += " <p style=\"font-size:12px; text-align:center; margin:5px 5px; color:#ff7800\">Thời gian mở cửa : Từ 7h30 đến 18h30 hàng ngày (làm cả thứ 7, chủ nhật). Khách hàng đến trực tiếp xem hàng giảm thêm giá.</p>";
            chuoihtml += "</div>";
            chuoihtml += "<div style=\"clear:both\"></div>";
            chuoihtml += " </div>";
            chuoihtml += " <div style=\"width:100%; height:40px; float:left; margin:0px; background:#1c7fc4\">";
            chuoihtml += "<span style=\"font-size:12px; text-align:center; color:#FFF; line-height:40px; display:block\">Copyright (c) 2002 – 2015 SEABIG VIET NAM. All Rights Reserved</span>";
            chuoihtml += " </div>";
            chuoihtml += "</div>";
            chuoihtml += "</body>";
            chuoihtml += "</html>";
            string body = chuoihtml;
             
            var smtp = new System.Net.Mail.SmtpClient();
            {
                smtp.Host = config.Host;
                smtp.Port = int.Parse(config.Port.ToString());
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(fromAddress, fromPassword);
                smtp.Timeout = int.Parse(config.Timeout.ToString());
            }
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true,


            })
            {
                smtp.Send(message);
            }


            
            Contact.Active = false;
            Contact.DateCreate = DateTime.Now;
            db.tblContacts.Add(Contact);
            db.SaveChanges();
            Session["Status"] = "<script>$(document).ready(function(){ alert('Bạn đã liên hệ thành công !') });</script>";

            return View();
        }

    }
}