using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;

namespace PriSecDBAPI.Helper
{
    public class PayPalCreateOrder
    {
        private static OrderRequest BuildRequestBody(String CustomInvoiceID, Double TotalPriceValue, Double PriceValue, Double HandlingValue, Double ItemValue, String CustomDescription)
        {
            OrderRequest orderRequest = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",

                ApplicationContext = new ApplicationContext
                {
                    BrandName = "Mr Chew IT Software PriSec DB Hosting",
                    LandingPage = "BILLING",
                    CancelUrl = "https://www.paypal.com",
                    ReturnUrl = "https://mrchewitsoftware.com.my",
                    UserAction = "PAY_NOW",
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest{
                        Description = "Digital Service",
                        SoftDescriptor = "PriSec Solutions",
                        InvoiceId = CustomInvoiceID, //Need to put in custom value
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = "USD",
                            Value = TotalPriceValue.ToString(), //Need to put in custom value
                            AmountBreakdown = new AmountBreakdown
                            {
                                ItemTotal = new Money
                                {
                                    CurrencyCode = "USD",
                                    Value = PriceValue.ToString() //Need to put in custom value
                                },
                                Handling = new Money
                                {
                                    CurrencyCode = "USD",
                                    Value = HandlingValue.ToString() //Need to put in custom value
                                }
                            }
                        },
                        Items = new List<Item>
                        {
                            new Item
                            {
                                Name = "DB Hosting",
                                Description = CustomDescription, //Custom value need to put in
                                Sku = "sku01",
                                UnitAmount = new Money
                                {
                                    CurrencyCode = "USD",
                                    Value = ItemValue.ToString() //Custom value need to put in
                                },
                                Quantity = "1",
                                Category = "DIGITAL_GOODS"
                            }
                        },
                    }
                }
            };

            return orderRequest;
        }

        public async static Task<HttpResponse> CreateOrder(String CustomInvoiceID, Double TotalPriceValue, Double PriceValue, Double HandlingValue, Double ItemValue, String CustomDescription)
        {
            var request = new OrdersCreateRequest();
            request.Headers.Add("prefer", "return=representation");
            request.RequestBody(BuildRequestBody(CustomInvoiceID, TotalPriceValue, PriceValue, HandlingValue, ItemValue, CustomDescription));
            var response = await PayPalClient.client().Execute(request);

            return response;
        }
    }
}
