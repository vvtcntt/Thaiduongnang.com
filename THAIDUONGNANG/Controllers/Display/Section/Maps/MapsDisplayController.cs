using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THAIDUONGNANG.Models;
namespace THAIDUONGNANG.Controllers.Display.Section.Maps
{
    public class MapsDisplayController : Controller
    {
        THAIDUONGNANGContext db = new THAIDUONGNANGContext();
        // GET: MapsDisplay
        public ActionResult Index()
        {
            tblMap tblmap = db.tblMaps.First();
            ViewBag.Title = "<title>" + tblmap.Name + "</title>";
            ViewBag.Description = "<meta name=\"description\" content=\"" + tblmap.Description + "\"/>";
            ViewBag.Keyword = "<meta name=\"keywords\" content=\"" + tblmap.Name + "\" /> ";
            ViewBag.dcTitle = "<meta name=\"DC.title\" content=\"" + tblmap.Name + "\" />";
            ViewBag.dcDescription = "<meta name=\"DC.description\" content=\"" + tblmap.Description + "\" />";
            return View(db.tblMaps.First());
        }
    }
}