using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using THAIDUONGNANG.Models;
namespace THAIDUONGNANG.Models
{
    public class Updatehistoty
    {
        public THAIDUONGNANGContext db = new THAIDUONGNANGContext();
        public static void UpdateHistory(string task,string FullName,string UserID)
        {

            THAIDUONGNANGContext db = new THAIDUONGNANGContext();
            tblHistoryLogin tblhistorylogin = new tblHistoryLogin();
            tblhistorylogin.FullName = FullName;
            tblhistorylogin.Task = task;
            tblhistorylogin.idUser = int.Parse(UserID);
            tblhistorylogin.DateCreate = DateTime.Now;
            tblhistorylogin.Active = true;
            
            db.tblHistoryLogins.Add(tblhistorylogin);
            db.SaveChanges();
           
        }
    }
}