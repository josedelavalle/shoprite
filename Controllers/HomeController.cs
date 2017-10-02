using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
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
            ViewBag.Message = "This is the work of an independant individual and is not to be interpretted as a representation of Shoprite, Wakefern, or its associates.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "My name is Jose DeLavalle.  Nice to meet you. :)";
            return View();
        }

        [HttpGet]
        [OutputCache(Duration=86400)]
        public JsonResult GetStores()
        {
            try
            {
                string myUrl = "http://plan.shoprite.com/Stores/Get?Country=United%20States";
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(myUrl);

                var storeList = new List<store>();
                // get all stores returned from DOM
                var storesInDOM = doc.DocumentNode.SelectNodes("//div[@class='storelist-inner-tab']").ToList();
                for (var x = 0; x < storesInDOM.Count(); x++)
                {
                    //loop through stores and pull details
                    var thisStore = new store();
                    string ndx = (x + 1).ToString();
                    thisStore.id = x;
                    thisStore.name = WebUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("/div[2]/div[" + ndx + "]/div[2]/div[1]/div[1]/h4[1]/a[1]")?.InnerText);
                    thisStore.link = doc.DocumentNode.SelectSingleNode("/div[2]/div[" + ndx + "]/div[2]/div[1]/div[1]/a[1]")?.Attributes["data-outboundhref"]?.Value;
                    thisStore.address1 = WebUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("/div[2]/div[" + ndx + "]/div[2]/div[1]/div[1]/p[1]")?.InnerText);
                    thisStore.address2 = WebUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("/div[2]/div[" + ndx + "]/div[2]/div[1]/div[1]/p[2]")?.InnerText);
                    thisStore.phonenumber = WebUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("/div[2]/div[" + ndx + "]/div[2]/div[3]/div[1]/p[3]")?.InnerText);
                    //add to our array to be returned in JSON
                    if (thisStore.name != null) storeList.Add(thisStore);
                }

                return Json(new
                {
                    status = "ok",
                    storeList
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw;
            }
            
        }
        
        [HttpGet]
        [OutputCache(Duration = 86400)]
        public JsonResult GetEvents()
        {
            try
            {
                string myUrl = "http://www.shoprite.com/health-events/";
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(myUrl);

                List<events> eventList = new List<events>();

                var theseEvents = doc.DocumentNode.SelectNodes("//p[@class='pageheadline']/a");

                if (theseEvents != null)
                {
                    foreach (var item in theseEvents)
                    {
                        events e = new events();
                        e.store = item.InnerText;
                        e.link = item.Attributes["href"].Value;
                        eventList.Add(e);
                    }
                }
                
                return Json(new
                {
                    status = "ok",
                    eventList
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                throw;
            }
        }

        [HttpGet]
        [OutputCache(Duration = 86400)]
        public JsonResult GetWeeklyCircular(string circularUrl, int pageNum)
        {
            try
            {
                string myUrl = "http://plan.shoprite.com" + circularUrl + "/Weekly/1/" + pageNum;
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(myUrl);

                List<product> dataList = new List<product>();
                var validDates = WebUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("//div[@id='CircularValidDates']")?.InnerText.Trim(' '));
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
                            var listitem = new product();
                            listitem.photo = itemdoc.DocumentNode.SelectSingleNode("//img")?.Attributes["src"]?.Value;
                            listitem.title = WebUtility.HtmlDecode(itemdoc.DocumentNode.SelectSingleNode("//p[@class='title']").InnerText);
                            listitem.price = WebUtility.HtmlDecode(itemdoc.DocumentNode.SelectSingleNode("//p[@class='price']").InnerText);
                            listitem.details = WebUtility.HtmlDecode(itemdoc.DocumentNode.SelectSingleNode("//p[@class='description']").InnerText);
                            dataList.Add(listitem);
                        }
                    }
                }

                return Json(new
                {
                    status = "ok",
                    validDates = validDates,
                    dataList
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                throw;
            }
            
        }

        
    }
}