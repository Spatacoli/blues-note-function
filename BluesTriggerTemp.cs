using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Spatacoli.Blues
{
    public static class BluesTriggerTemp
    {
        [FunctionName("BluesTriggerTemp")]
        [return: Table("BluesYouTubeData")]
        public static async Task<RowData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<TempData>(requestBody);

            log.LogInformation($"Temp is {data.value} and the calibration is {data.calibration}");
            return new RowData 
            { 
                PartitionKey = "Http",
                RowKey = Guid.NewGuid().ToString(),
                Temperature = data.value,
                Calibration = data.calibration
            };
        }
    }

    public class RowData
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public double Temperature { get; set; }
        public double Calibration { get; set; }
    }

    public class TempData
    {
        public double value { get; set; }
        public double calibration { get; set; }
    }
}
