using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;

namespace PriSecDBAPI.Helper
{
    public class PaypalCaptureOrder
    {
        public async static Task<HttpResponse> CaptureOrder(string OrderId)
        {
            var request = new OrdersCaptureRequest(OrderId);
            request.Prefer("return=representation");
            request.RequestBody(new OrderActionRequest());
            var response = await PayPalClient.client().Execute(request);

            return response;
        }
    }
}
