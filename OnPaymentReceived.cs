using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionApp1
{
    public static class OnPaymentReceived
    {
        [FunctionName("OnPaymentReceived")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Queue("orders", Connection="MyAppSetting")] IAsyncCollector<Order> orderQueue,
            ILogger log)
        {

            log.LogInformation("Payment was received");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var order = JsonConvert.DeserializeObject<Order>(requestBody);
            await orderQueue.AddAsync(order);
            log.LogInformation($"order for {order.Name} purchased {order.Item} at ${order.Price}");

            return new OkObjectResult("Thank you for your business!");
        }

        public class Order
        {
            public string Name { get; set; }
            public string Item { get; set; }
            public decimal Price { get; set; }
        }
    }
}
