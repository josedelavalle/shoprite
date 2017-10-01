using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shoprite.Models;
using HtmlAgilityPack;


namespace Shoprite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "This is a work of an independant individual and does not in any way represent Shoprite or Wakefern.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "My name is Jose DeLavalle.  Nice to meet you. :)";
            return View();
        }

        public JsonResult GetNearbyStores()
        {
            //string myUrl = "http://plan.shoprite.com/Stores/Get?IsIPSearch=true&StoreType=Cir&_=1506737512632";
            //string myUrl = "http://plan.shoprite.com/Stores/Get?Country=United%20States&Region=New%20Jersey&City=Edison&Radius=20";
            string myUrl = "http://plan.shoprite.com/Stores/Get?Country=United%20States";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(myUrl);

            var storeList = new List<storeList>();
            // get all stores returned from DOM
            var storesInDOM = doc.DocumentNode.SelectNodes("//div[@class='storelist-inner-tab']").ToList();
            for (var x = 0; x < storesInDOM.Count(); x++)
            {
                //loop through stores and pull details
                var thisStore = new storeList();
                string ndx = (x + 1).ToString();
                thisStore.id = x;
                thisStore.name = doc.DocumentNode.SelectSingleNode("/div[2]/div[" + ndx + "]/div[2]/div[1]/div[1]/h4[1]/a[1]")?.InnerText;
                thisStore.link = doc.DocumentNode.SelectSingleNode("/div[2]/div[" + ndx + "]/div[2]/div[1]/div[1]/a[1]")?.Attributes["data-outboundhref"]?.Value;
                thisStore.address1 = doc.DocumentNode.SelectSingleNode("/div[2]/div[" + ndx + "]/div[2]/div[1]/div[1]/p[1]")?.InnerText.Replace("&amp;","&");
                thisStore.address2 = doc.DocumentNode.SelectSingleNode("/div[2]/div[" + ndx + "]/div[2]/div[1]/div[1]/p[2]")?.InnerText;
                thisStore.phonenumber = doc.DocumentNode.SelectSingleNode("/div[2]/div[" + ndx + "]/div[2]/div[3]/div[1]/p[3]")?.InnerText;
                //add to our array to be returned in JSON
                if (thisStore.name != null) storeList.Add(thisStore);
            }
           
            return Json(new
            {
                status = "ok",
                storeList
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetWeeklyCircular(string circularUrl, int pageNum)
        {
            string myUrl = "http://plan.shoprite.com" + circularUrl + "/Weekly/1/" + pageNum;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(myUrl);

            var dataList = new List<products>();
            var validDates = doc.DocumentNode.SelectSingleNode("//div[@id='CircularValidDates']")?.InnerText.Replace("\r\n", "").Trim(' ');
            var tooltips = doc.DocumentNode.SelectNodes("//div[@class='tooltip']")?.ToList();
            if (tooltips != null)
            {
                foreach (var item in tooltips)
                {
                    var dataid = item.Attributes["data-id"]?.Value;
                    if (dataid != null)
                    {
                        string itemUrl = "http://plan.shoprite.com/CircularDetails/Item/" + dataid;
                        HtmlDocument itemdoc = web.Load(itemUrl);
                        var listitem = new products();
                        listitem.photo = itemdoc.DocumentNode.SelectSingleNode("//img")?.Attributes["src"]?.Value;
                        listitem.title = itemdoc.DocumentNode.SelectSingleNode("//p[@class='title']").InnerText.Replace("&amp;", "&");
                        listitem.price = itemdoc.DocumentNode.SelectSingleNode("//p[@class='price']").InnerText.Replace("&#162;", "¢");
                        listitem.details = itemdoc.DocumentNode.SelectSingleNode("//p[@class='description']").InnerText;
                        dataList.Add(listitem);
                    }
                }
            }
            

            //var nodes = doc.DocumentNode.SelectNodes("//img[@class='itemImage']")?.ToList();
            
            //if (nodes != null)
            //{
            //    int ndx = 3;
            //    foreach (var item in nodes)
            //    {

            //        var listitem = new products();
            //        listitem.title = item.Attributes["alt"].Value.Replace("&amp;","&");
            //        listitem.photo = item.Attributes["src"].Value;
            //        listitem.price = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[2]/section[1]/section[1]/div[5]/div[1]/div[" + ndx + "]/div[2]/p[1]")?.InnerHtml.Replace("&#162;", "¢");
            //        dataList.Add(listitem);
            //        ndx++;
            //    }
            //}
            return Json(new
            {
                status = "ok",
                validDates = validDates,
                dataList
            }, JsonRequestBehavior.AllowGet);
        }

        
    }
}