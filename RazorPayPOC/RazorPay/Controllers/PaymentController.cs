using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Razorpay.Api;
using RazorPayPOC.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RazorPayPOC.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private string razorPayKey;
        private string razorPaySecret;
        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
            razorPayKey = _configuration.GetSection("RazorPay")["Key"];
            razorPaySecret = _configuration.GetSection("RazorPay")["Secret"];
        }
        [Route("api/payment/create-order")]
        public IActionResult Payment()
        {
            //Dictionary<string, object> options = new Dictionary<string, object>();
            //options.Add("amount", 40000); // amount in the smallest currency unit
            //options.Add("receipt", "order_rcptid_11");
            //options.Add("currency", "INR");
            //options.Add("name", "YYH");
            //RazorpayClient client = new RazorpayClient(razorPayKey, razorPaySecret);
            //Order order = client.Order.Create(options);
            return Ok();

        }

        [HttpPost]
        [Route("api/payment-status")]
        public async Task<IActionResult> PayPageAsync()
        {
            Dictionary<string, StringValues> dict;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var response = await reader.ReadToEndAsync();
                dict = QueryHelpers.ParseNullableQuery(response);
            }
            var orderResponse = new RazorPayResponse();
            if (dict.TryGetValue("razorpay_payment_id", out StringValues paymentId))
                orderResponse.PaymentId = paymentId;
            if (dict.TryGetValue("razorpay_order_id", out StringValues orderId))
                orderResponse.OrderId = orderId;
            if (dict.TryGetValue("razorpay_signature", out StringValues signature))
                orderResponse.Signature = signature;
            if (dict.TryGetValue("error", out StringValues error))
                orderResponse.Signature = signature;


            var signatureValid = VerifySignature(orderResponse.OrderId, orderResponse.PaymentId, orderResponse.Signature, razorPaySecret);

            return Ok();
        }

        private bool VerifySignature(string orderId, string paymentId, string signature, string secret)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();

            var text = orderId + "|" + paymentId;
            byte[] textBytes = encoding.GetBytes(text);
            byte[] keyBytes = encoding.GetBytes(razorPaySecret);

            byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            var genSign = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return string.Equals(genSign, signature);
        }
    }
}