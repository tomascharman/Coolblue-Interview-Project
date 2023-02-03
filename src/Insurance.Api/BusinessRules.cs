using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Insurance.Api.Controllers;
using Newtonsoft.Json;

namespace Insurance.Api
{
    public static class BusinessRules
    {
        public static void GetProductType(string baseAddress, int productID, ref HomeController.InsuranceDto insurance)
        {
            HttpClient client = new HttpClient{ BaseAddress = new Uri(baseAddress)};
            string json = client.GetAsync(GetAllProductTypesUri).Result.Content.ReadAsStringAsync().Result;
            var collection = JsonConvert.DeserializeObject<dynamic>(json);

            json = client.GetAsync(string.Format(GetSpecificProductUri, productID)).Result.Content.ReadAsStringAsync().Result;
            var product = JsonConvert.DeserializeObject<dynamic>(json);

            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i].id == product.productTypeId && collection[i].canBeInsured == true)
                {
                    insurance.ProductTypeName = collection[i].name;
                    insurance.ProductTypeHasInsurance = true;
                }
            }
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

        private const string LaptopsProductType = "Laptops";
        private const string SmartphonesProductType = "Smartphones";

        private const int SalesPriceMinimumThreshold = 500;
        private const int SalesPriceMaximumThreshold = 2000;
        private const int InsuranceValueForLowEndProducts = 1000;
        private const int InsuranceValueForHighEndProducts = 2000;
        private const int InsuranceValueForLaptopProducts = 500;

        private const string GetAllProductTypesUri = "/product_types";
        private const string GetSpecificProductUri = "/products/{0:G}";
    }
}