using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Razorpay.Api;
using RazorPayPOC.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPayPOC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private string razorPayKey;
        private string razorPaySecret;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            razorPayKey = _configuration.GetSection("RazorPay")["Key"];
            razorPaySecret = _configuration.GetSection("RazorPay")["Secret"];
        }

        public IActionResult Index()
        {
            Dictionary<string, object> options = new Dictionary<string, object>();
            Dictionary<string, string> customValues = new Dictionary<string, string>();
            customValues.Add("CustId", "123");
            options.Add("amount", 40000); // amount in the smallest currency unit
            options.Add("receipt", "order_rcptid_11");
            options.Add("currency", "INR");
            options.Add("notes", customValues);
            RazorpayClient client = new RazorpayClient(razorPayKey, razorPaySecret);
            Order order = client.Order.Create(options);
            var orderId = order.Attributes.id;
            var orderModel = new OrderModel();
            orderModel.OrderId = orderId;
            return View(orderModel);

            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
