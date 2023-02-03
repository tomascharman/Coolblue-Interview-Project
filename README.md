# Coolblue-Interview-Project

## TASK 1 [BUGFIX] ##

#### ISSUE ####

Everytime `HomeController.CalculateInsurance()` is executed for a laptop product type that cost less than €500, the `toInsure.InsuranceValue` is always set to 0.
```
if (toInsure.SalesPrice < 500)
  toInsure.InsuranceValue = 0;
```

#### FIX ####

Issue is fixed by simply adding an additional check to make sure that the `toInsure.ProductTypeName` is not a laptop. Now the `toInsure.InsuranceValue` will only be set to 0 if the product is less than €500 and not a laptop.
```
if (toInsure.SalesPrice < 500 && toInsure.ProductTypeName != "Laptops")
  toInsure.InsuranceValue = 0;
```

I created an additional unit test in `InsuranceTests` (`CalculateInsurance_GivenSalesPriceLessThan500AndProductTypeIsLaptops_ShouldAdd500EurosToInsuranceCost()`).
I have also expanded the `MapGet` for specific products `ControllerTestStartup` to be reusable by making an array of products. This will now return the specific product that is passed in. This will make it a lot easier to test insurance calculations and less work to maintain.

