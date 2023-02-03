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


## TASK 2 [REFACTORING] ##

* Moved the setting of insurance values from the `HomeController` to its own method in `BusinessRules`.

* Multiple magic strings & integers used for various checks in the setting of insurance values. Created some constant string * integers fields to make lighter work if/when refactoring needs to happen.
```
private const string LaptopsProductType = "Laptops";
private const string SmartphonesProductType = "Smartphones";
private const int SalesPriceMinimumThreshold = 500;
private const int SalesPriceMaximumThreshold = 2000;
private const int InsuranceValueForLowEndProducts = 1000;
private const int InsuranceValueForHighEndProducts = 2000;
private const int InsuranceValueForLaptopProducts = 500;
```

* When setting insurance values the `else` condition contained a lot of repeated checks to see if the `ProductTypeHasInsurance`.
  * Changed `else` condition to an `else if`.  
  * Added `ProductTypeHasInsurance` check to the `if else` condition.

* A local float variable (`insurance`) is created but never used so this has been removed from the `CalculateInsurance` method.

* A local int variable (`productId`) is passed in as a parameter for the `BusinessRules` methods. These methods now use the `toInsure.ProductId`.

* Removed some un-used local variables in `BusinessRules.GetProductType()`
  * `productTypeName` 
  * `hasInsurance`
  
* `insurance` was replaced with a new instance of itself in `BusinessRules.GetProductType()`. This has been removed.

* Local variable `productTypeId` has been removed and check will now used the `product.productTypeId`.

* Replaced Uri magic strings with constants
```
private const string GetAllProductTypesUri = "/product_types";
private const string GetSpecificProductUri = "/products/{0:G}";
```
  
