using System;
using System.Linq;
using Insurance.Api.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Xunit;
using static Insurance.Tests.InsuranceTests;

namespace Insurance.Tests
{
    public class InsuranceTests : IClassFixture<ControllerTestFixture>
    {
        private readonly ControllerTestFixture _fixture;

        public InsuranceTests(ControllerTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void CalculateInsurance_GivenSalesPriceBetween500And2000Euros_ShouldAddThousandEurosToInsuranceCost()
        {
            const float expectedInsuranceValue = 1000;

            var dto = new HomeController.InsuranceDto
            {
                ProductId = 1,
            };
            var sut = new HomeController();

            var result = sut.CalculateInsurance(dto);

            Assert.Equal(
                expected: expectedInsuranceValue,
                actual: result.InsuranceValue
            );
        }

        [Fact]
        public void CalculateInsurance_GivenSalesPriceLessThan500AndProductTypeIsLaptops_ShouldAdd500EurosToInsuranceCost()
        {
            const float expectedInsuranceValue = 500;

            var dto = new HomeController.InsuranceDto
            {
                ProductId = 837856,
            };
            var sut = new HomeController();

            var result = sut.CalculateInsurance(dto);

            Assert.Equal(
                expected: expectedInsuranceValue,
                actual: result.InsuranceValue
            );
        }

        [Fact]
        public void CalculateOrderInsurance_GivenOrderThatContainsMultipleItems_ShouldBe1500EurosTotalInsuranceCost()
        {
            const float expectedInsuranceValue = 1500;

            var order = new HomeController.OrderDto
            {
                OrderId = 1,
                ProductIds = new int[]
                {
                    837856,
                    735246
                }
            };

            var sut = new HomeController();

            var result = sut.CalculateOrderInsurance(order);

            Assert.Equal(
                expected: expectedInsuranceValue,
                actual: result.TotalInsuranceCost
            );
        }

        [Fact]
        public void CalculateOrderInsurance_GivenOrderThatContainsDigitalCamera_ShouldBe500EurosTotalInsuranceCost()
        {
            const float expectedInsuranceValue = 500;

            var order = new HomeController.OrderDto
            {
                OrderId = 1,
                ProductIds = new int[]
                {
                    715990
                }
            };

            var sut = new HomeController();

            var result = sut.CalculateOrderInsurance(order);

            Assert.Equal(
                expected: expectedInsuranceValue,
                actual: result.TotalInsuranceCost
            );
        }

        [Fact]
        public void CalculateOrderInsurance_GivenOrderThatContainsTwoDigitalCameras_ShouldBe500EurosTotalInsuranceCost()
        {
            const float expectedInsuranceValue = 500;

            var order = new HomeController.OrderDto
            {
                OrderId = 1,
                ProductIds = new int[]
                {
                    715990,
                    715990
                }
            };

            var sut = new HomeController();

            var result = sut.CalculateOrderInsurance(order);

            Assert.Equal(
                expected: expectedInsuranceValue,
                actual: result.TotalInsuranceCost
            );
        }

        public class ControllerTestFixture : IDisposable
        {
            private readonly IHost _host;

            public ControllerTestFixture()
            {
                _host = new HostBuilder()
                       .ConfigureWebHostDefaults(
                            b => b.UseUrls("http://localhost:5002")
                                  .UseStartup<ControllerTestStartup>()
                        )
                       .Build();

                _host.Start();
            }

            public void Dispose() => _host.Dispose();
        }

        public class ControllerTestStartup
        {
            public void Configure(IApplicationBuilder app)
            {
                app.UseRouting();
                app.UseEndpoints(
                    ep =>
                    {
                        ep.MapGet(
                            "products/{id:int}",
                            context =>
                            {
                                int productId = int.Parse((string)context.Request.RouteValues["id"]);

                                var products = new[]
                                                   {
                                                   new
                                                   {
                                                       id = 1,
                                                       name = "Test Product",
                                                       productTypeId = 1,
                                                       salesPrice = 750
                                                   },
                                                   new
                                                   {
                                                       id = 837856,
                                                       name = "Lenovo Chromebook C330-11 81HY000MMH",
                                                       productTypeId = 21,
                                                       salesPrice = 299
                                                   },
                                                   new
                                                   {
                                                       id = 735246,
                                                       name = "AEG L8FB86ES",
                                                       productTypeId = 124,
                                                       salesPrice = 699
                                                   },
                                                   new
                                                   {
                                                       id = 715990,
                                                       name = "Canon Powershot SX620 HS Black",
                                                       productTypeId = 33,
                                                       salesPrice = 195
                                                   },
                                                   new
                                                   {
                                                       id = 832845,
                                                       name = "Apple iPod Touch (2019) 32 GB Space Gray",
                                                       productTypeId = 33,
                                                       salesPrice = 229
                                                   }
                                                   };
                                return context.Response.WriteAsync(JsonConvert.SerializeObject(products.FirstOrDefault(x => x.id == productId)));
                            }
                        );
                        ep.MapGet(
                            "product_types",
                            context =>
                            {
                                var productTypes = new[]
                                                   {
                                                   new
                                                   {
                                                       id = 1,
                                                       name = "Test type",
                                                       canBeInsured = true
                                                   },
                                                   new
                                                   {
                                                       id = 21,
                                                       name = "Laptops",
                                                       canBeInsured = true
                                                   },
                                                   new
                                                   {
                                                       id = 124,
                                                       name = "Washing machines",
                                                       canBeInsured = true
                                                   },
                                                   new
                                                   {
                                                       id = 33,
                                                       name = "Digital cameras",
                                                       canBeInsured = true
                                                   },
                                                   new
                                                   {
                                                       id = 12,
                                                       name = "MP3 players",
                                                       canBeInsured = false
                                                   }
                                                   };
                                return context.Response.WriteAsync(JsonConvert.SerializeObject(productTypes));
                            }
                        );
                    }
                );
            }
        }
    }
}