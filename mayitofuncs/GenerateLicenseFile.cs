using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace mayitofuncs
{
    public class GenerateLicenseFile
    {
        /// <summary>
        /// Changed from void, to static async Task
        /// </summary>
        /// <param name="order"></param>
        /// <param name="binder">this will help to customize any binding at runtime</param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("GenerateLicenseFile")]
        //public void Run(
        public static async Task Run(
            [QueueTrigger("orders", Connection = "AzureWebJobsStorage")]Order order,
            IBinder binder, //
            //   [Blob("licenses/{rand-guid}.lic")] TextWriter outputBlob,
            ILogger log)
        {
            var outputBlob = await binder.BindAsync<TextWriter>(
                new BlobAttribute($"licenses/{order.OrderId}.lic")
                {
                    Connection = "AzureWebJobsStorage"
                });

            outputBlob.WriteLine($"OrderId: {order.OrderId}");
            outputBlob.WriteLine($"Email: {order.Email}");
            outputBlob.WriteLine($"ProductId: {order.ProductId}");
            outputBlob.WriteLine($"PurchaseDate: {DateTime.UtcNow}");
            var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(
                System.Text.Encoding.UTF8.GetBytes(order.Email + "secret"));
            outputBlob.WriteLine($"SecretCode: {BitConverter.ToString(hash).Replace("-", "")}");
        }
    }
}
