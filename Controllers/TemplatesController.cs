using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace shoprite.Controllers
{
    public class TemplatesController : Controller
    {
        // GET: Templates
        public ActionResult ProductCard()
        {
            return PartialView();
        }

        public ActionResult StoreCard()
        {
            return PartialView();
        }
    }
}