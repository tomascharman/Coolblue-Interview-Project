using System.Diagnostics;
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
        public OrderDto CalculateOrderInsurance([FromBody] OrderDto orderToInsure)
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
                BusinessRules.GetSurchageValue(ProductApi, insuranceDtos[i].ProductId, ref insuranceDtos[i]);
            }

            orderToInsure.TotalInsuranceCost = insuranceDtos.Sum(x => x.InsuranceValue);

            BusinessRules.AddAdditionalInsuranceCostsToOrder(ref orderToInsure, insuranceDtos);

            return orderToInsure;
        }

        [HttpPost]
        [Route("api/insurance/product")]
        public InsuranceDto CalculateInsurance([FromBody] InsuranceDto toInsure)
        {
            BusinessRules.GetProductType(ProductApi, toInsure.ProductId, ref toInsure);
            BusinessRules.GetSalesPrice(ProductApi, toInsure.ProductId, ref toInsure);
            BusinessRules.SetInsuranceValues(ref toInsure);
            BusinessRules.GetSurchageValue(ProductApi, toInsure.ProductId, ref toInsure);

            return toInsure;
        }

        [HttpPatch]
        [Route("api/insurance/addsurcharge")]
        public ProductTypeDto AddSurchargeToProductType([FromBody] ProductTypeDto productType, int surchargeValue)
        {
            lock (LockObject)
            {
                BusinessRules.UpdateSurchargeForProductType(ProductApi, surchargeValue, ref productType);
                return productType;
            }
        }

        public class InsuranceDto
        {
            public int ProductId { get; set; }
            public decimal InsuranceValue { get; set; }
            [JsonIgnore]
            public string ProductTypeName { get; set; }
            [JsonIgnore]
            public bool ProductTypeHasInsurance { get; set; }
            [JsonIgnore]
            public decimal SalesPrice { get; set; }
        }

        public class OrderDto
        {
            public int OrderId { get; set; }
            public int[] ProductIds { get; set; }
            public decimal TotalInsuranceCost { get; set; }
        }

        public class ProductTypeDto
        {
            public int ProductTypeId { get; set; }
            public string ProductTypeName { get; set; }
            public bool CanBeInsured { get; set; }
            public int Surcharge { get; set; }
        }

        private const string ProductApi = "http://localhost:5002";
        public static readonly object LockObject = new object();
    }
}