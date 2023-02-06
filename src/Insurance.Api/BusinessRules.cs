using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Insurance.Api.Controllers;
using Newtonsoft.Json;
using static Insurance.Api.Controllers.HomeController;

namespace Insurance.Api
{
    public static class BusinessRules
    {
        public static void GetProductType(string baseAddress, int productID, ref HomeController.InsuranceDto insurance)
        {
            HttpClient client = new HttpClient { BaseAddress = new Uri(baseAddress) };

            string json = client.GetAsync(string.Format(GetSpecificProductUri, productID)).Result.Content.ReadAsStringAsync().Result;
            var product = JsonConvert.DeserializeObject<dynamic>(json);

            json = client.GetAsync(string.Format(GetSpecificProductTypeUri, product.productTypeId)).Result.Content.ReadAsStringAsync().Result;
            var type = JsonConvert.DeserializeObject<dynamic>(json);

            insurance.ProductTypeName = type.name;
            insurance.ProductTypeHasInsurance = type.canBeInsured;
        }

        public static void GetSalesPrice(string baseAddress, int productID, ref HomeController.InsuranceDto insurance)
        {
            HttpClient client = new HttpClient{ BaseAddress = new Uri(baseAddress)};
            string json = client.GetAsync(string.Format(GetSpecificProductUri, productID)).Result.Content.ReadAsStringAsync().Result;
            var product = JsonConvert.DeserializeObject<dynamic>(json);

            insurance.SalesPrice = product.salesPrice;
        }

        public static void SetInsuranceValues(ref HomeController.InsuranceDto insurance)
        {
            if (insurance.SalesPrice < SalesPriceMinimumThreshold && insurance.ProductTypeName != LaptopsProductType)
                insurance.InsuranceValue = 0;
            else if (insurance.ProductTypeHasInsurance)
            {
                if (insurance.SalesPrice > SalesPriceMinimumThreshold && insurance.SalesPrice < SalesPriceMaximumThreshold)
                    insurance.InsuranceValue += InsuranceValueForLowEndProducts;
                else if (insurance.SalesPrice >= SalesPriceMaximumThreshold)
                    insurance.InsuranceValue += InsuranceValueForHighEndProducts;

                if (insurance.ProductTypeName == LaptopsProductType || insurance.ProductTypeName == SmartphonesProductType)
                    insurance.InsuranceValue += InsuranceValueForLaptopProducts;
            }
        }

        public static void AddAdditionalInsuranceCostsToOrder(ref OrderDto orderToInsure, InsuranceDto[] insuranceDtos)
        {
            if (insuranceDtos.Any(x => x.ProductTypeName == DigitalCamerasProductType))
                orderToInsure.TotalInsuranceCost += AdditionalInsuranceCostsForOrderContainingDigitalCamera;
        }

        public static void UpdateSurchargeForProductType(string baseAddress, int surchargeValue, ref HomeController.ProductTypeDto productType)
        {
            HttpClient client = new HttpClient { BaseAddress = new Uri(baseAddress) };

            string json = client.GetAsync(string.Format(GetSpecificProductTypeUri, productType.ProductTypeId)).Result.Content.ReadAsStringAsync().Result;
            var product = JsonConvert.DeserializeObject<dynamic>(json);
            product.surcharge = surchargeValue;

            productType.Surcharge = surchargeValue;
        }

        public static void GetSurchageValue(string baseAddress, int productID, ref HomeController.InsuranceDto insurance)
        {
            HttpClient client = new HttpClient { BaseAddress = new Uri(baseAddress) };
            string json = client.GetAsync(string.Format(GetSpecificProductUri, productID)).Result.Content.ReadAsStringAsync().Result;
            var product = JsonConvert.DeserializeObject<dynamic>(json);

            json = client.GetAsync(string.Format(GetSpecificProductTypeUri, product.productTypeId)).Result.Content.ReadAsStringAsync().Result;
            var type = JsonConvert.DeserializeObject<dynamic>(json);

            decimal surcharge = decimal.Parse((string)type.surcharge);

            insurance.InsuranceValue += surcharge;
        }

        private const string LaptopsProductType = "Laptops";
        private const string SmartphonesProductType = "Smartphones";
        private const string DigitalCamerasProductType = "Digital cameras";

        private const int SalesPriceMinimumThreshold = 500;
        private const int SalesPriceMaximumThreshold = 2000;
        private const int InsuranceValueForLowEndProducts = 1000;
        private const int InsuranceValueForHighEndProducts = 2000;
        private const int InsuranceValueForLaptopProducts = 500;
        private const int AdditionalInsuranceCostsForOrderContainingDigitalCamera = 500;

        private const string GetAllProductTypesUri = "/product_types";
        private const string GetSpecificProductUri = "/products/{0:G}";
        private const string GetSpecificProductTypeUri = "/product_types/{0:G}";
    }
}