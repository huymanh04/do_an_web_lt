using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace do_an_web.Models
{
    public class MomoPaymentRequest
    {
        public string partnerCode { get; set; }
        public string accessKey { get; set; }
        public string requestId { get; set; }
        public string orderId { get; set; }
        public string amount { get; set; }
        public string orderInfo { get; set; }
        public string returnUrl { get; set; }
        public string notifyUrl { get; set; }
        public string signature { get; set; }
    }
}