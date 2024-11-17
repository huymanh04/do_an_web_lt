using do_an_web.Models;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace do_an_web.Controllers
{
    public class autobankController : Controller
    {
        public class HttpClientSingleton
        {
            private static readonly HttpClient client = new HttpClient();

            static HttpClientSingleton()
            {
                client.BaseAddress = new Uri("https://api.vietqr.org/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }

            public static HttpClient Instance => client;
        }

        // GET: autobank
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> Create(decimal amount, string content)
        {
            if (amount <= 0 || string.IsNullOrEmpty(content))
            {
                return Json(new { success = false, message = "Invalid parameters" }, JsonRequestBehavior.AllowGet);
            }

            string qrUrl = $"https://img.vietqr.io/image/vietcombank-1032471320-compact2.jpg?amount={amount}&addInfo={content}&accountName=DAM%20HUY%20MANH";

            try
            {
               
                using (var client = new HttpClient())
                {
                    var imageBytes = await client.GetByteArrayAsync(qrUrl);
                    var base64Image = Convert.ToBase64String(imageBytes);
                    return Json(new { success = true, qrCodeImage = base64Image }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error generating QR Code: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}
