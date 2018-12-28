using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Desktop_Notification.Controllers
{
    public class FcmNotificationController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("SendNotification");
        }
    }
}

