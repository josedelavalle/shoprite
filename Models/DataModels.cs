using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shoprite.Models
{
    public class product
    {
        public string title { get; set; }
        public string price { get; set; }
        public string details { get; set; }
        public string photo { get; set; }
    }

    public class store
    {
        public int id { get; set; }
        public string name { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string phonenumber { get; set; }
        public string link { get; set; }
    }

    public class coupon
    {
        public string brand { get; set; }
        public string price { get; set; }
        public string photo { get; set; }
        public string details { get; set; }
        public string expiration { get; set; }
    }

    public class events
    {
        public string store { get; set; }
        public string link { get; set; }
    }
}