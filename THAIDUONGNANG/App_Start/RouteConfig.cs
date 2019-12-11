using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace THAIDUONGNANG
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
             
            routes.MapRoute("MyRoute2", "{tag}-mua-san-pham.html", new { controller = "Product", action = "ProductDetail", tag = UrlParameter.Optional }, new { controller = "^P.*" }, new[] { "MyNamespace2" });
            routes.MapRoute("Baogia", "{tag}-bao-gia.html", new { controller = "Baogia", action = "BaogiaDetail", tag = UrlParameter.Optional }, new { controller = "^B.*" }, new[] { "MyNamespace" });
            routes.MapRoute("MyRoute", "{tag}.html", new { controller = "News", action = "NewsDetail", tag = UrlParameter.Optional }, new { controller = "^N.*" }, new[] { "MyNamespace3" });
            routes.MapRoute("Danh_Sach", "0/{Tag}/{*catchall}", new { controller = "Product", action = "ListProduct", tag = UrlParameter.Optional }, new { controller = "^P.*", action = "^ListProduct$" });
            routes.MapRoute("TagProduct", "Tag/{Tag}/{*catchall}", new { controller = "Product", action = "TagProduct", tag = UrlParameter.Optional }, new { controller = "^P.*", action = "^TagProduct$" });

            routes.MapRoute("TagNews", "TagNews/{Tag}/{*catchall}", new { controller = "News", action = "TagNews", tag = UrlParameter.Optional }, new { controller = "^N.*", action = "^TagNews$" });
            routes.MapRoute("ListNews", "2/{Tag}/{*catchall}", new { controller = "News", action = "ListNews", tag = UrlParameter.Optional }, new { controller = "^N.*", action = "^ListNews$" });
            routes.MapRoute(name: "ban-tin-khuyen-mai", url: "ban-tin-khuyen-mai", defaults: new { controller = "product", action = "detail" });

            routes.MapRoute(name: "Hang-san-xuat", url: "Hang-san-xuat", defaults: new { controller = "ManufacturesDeplay", action = "ListManufactures" });
            routes.MapRoute(name: "He-Thong-phan-phoi", url: "He-Thong-phan-phoi", defaults: new { controller = "Agencys", action = "ListAgency" });
            routes.MapRoute(name: "Contact", url: "Lien-he", defaults: new { controller = "Contact", action = "Index" });
            routes.MapRoute(name: "SearchProduct", url: "SearchProduct", defaults: new { controller = "Products", action = "SearchProduct" });
            routes.MapRoute(name: "Order", url: "Order", defaults: new { controller = "Order", action = "OrderIndex" });
            routes.MapRoute(name: "Khuyenmai", url: "Khuyen-mai-toto", defaults: new { controller = "Sale", action = "ListSale" });
            routes.MapRoute(name: "Maps", url: "Ban-do", defaults: new { controller = "MapsDisplay", action = "Index" });
            routes.MapRoute(name: "Spdongbo", url: "san-pham-toto-dong-bo", defaults: new { controller = "ProductSynDisplay", action = "Hienthidongbo" });
            routes.MapRoute(name: "Admin", url: "Admin", defaults: new { controller = "Login", action = "LoginIndex" });
            routes.MapRoute(name: "Catalogues", url: "Catalogues", defaults: new { controller = "Catalogues", action = "ListCatalogues" });
            routes.MapRoute(name: "Default", url: "{controller}/{action}/{id}", defaults: new { controller = "Default", action = "Index", id = UrlParameter.Optional });
        }
    }
}
