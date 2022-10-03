using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mayitofuncs
{
    public static class OnPaymentReceived
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="orderQueue">Instance of IAsyncCollector which will allow us to send messages to
        /// this queue from inside my azure function by calling the AddAsync method</param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("OnPaymentReceived")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Queue("orders")] IAsyncCollector<Order> orderQueue,
            ILogger log)
        {
            log.LogInformation("Received a payment.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var order = JsonConvert.DeserializeObject<Order>(requestBody);
            // end order to queue
            await orderQueue.AddAsync(order);
            log.LogInformation($"Order {order.OrderId} received from {order.Email} for product {order.ProductId}");
            return new OkObjectResult($"Thank you for your purchase");
        }
    }

    public class Order
    {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string Email { get; set; }
        public decimal Price { get; set; }
    }
}
