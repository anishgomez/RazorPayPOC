using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPayPOC.Models
{
    public class RazorPayResponse
    {
        public string PaymentId { get; set; }
        public string OrderId { get; set; }

        public string Signature { get; set; }

        public Error Error { get; set; }
    }

    public class Error
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Field { get; set; }
    }
}
