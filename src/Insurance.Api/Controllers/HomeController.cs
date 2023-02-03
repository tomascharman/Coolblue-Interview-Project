using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.Api.Controllers
{
    public class HomeController: Controller
    {
        [HttpPost]
        [Route("api/insurance/order")]
        public OrderDto CalculateInsurance([FromBody] OrderDto orderToInsure)
        {
            InsuranceDto[] insuranceDtos = new InsuranceDto[orderToInsure.ProductIds.Length];

            for (int i = 0; i < orderToInsure.ProductIds.Length; i++)
            {
                InsuranceDto toInsure = new InsuranceDto();
                toInsure.ProductId = orderToInsure.ProductIds[i];
                insuranceDtos[i] = toInsure;
            }

            for (int i = 0; i < insuranceDtos.Length; i++)
            {
                BusinessRules.GetProductType(ProductApi, insuranceDtos[i].ProductId, ref insuranceDtos[i]);
                BusinessRules.GetSalesPrice(ProductApi, insuranceDtos[i].ProductId, ref insuranceDtos[i]);
                BusinessRules.SetInsuranceValues(ref insuranceDtos[i]);
            }

            orderToInsure.TotalInsuranceCost = insuranceDtos.Sum(x => x.InsuranceValue);

            return orderToInsure;
        }

        [HttpPost]
        [Route("api/insurance/product")]
        public InsuranceDto CalculateInsurance([FromBody] InsuranceDto toInsure)
        {
            BusinessRules.GetProductType(ProductApi, toInsure.ProductId, ref toInsure);
            BusinessRules.GetSalesPrice(ProductApi, toInsure.ProductId, ref toInsure);
            BusinessRules.SetInsuranceValues(ref toInsure);

            return toInsure;
        }

        public class InsuranceDto
        {
            public int ProductId { get; set; }
            public float InsuranceValue { get; set; }
            [JsonIgnore]
            public string ProductTypeName { get; set; }
            [JsonIgnore]
            public bool ProductTypeHasInsurance { get; set; }
            [JsonIgnore]
            public float SalesPrice { get; set; }
        }

        public class OrderDto
        {
            public int OrderId { get; set; }
            public int[] ProductIds { get; set; }
            public float TotalInsuranceCost { get; set; }
        }

        private const string ProductApi = "http://localhost:5002";
    }
}