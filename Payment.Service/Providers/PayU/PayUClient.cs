using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Payment.Model.PayU;

namespace Payment.Service.Providers.PayU
{
    public class PayUClient : IPayUClient
    {
        private readonly IOptions<PayUSettings> payuSettings;

        public PayUClient(IOptions<PayUSettings> payuSettings)
        {
            this.payuSettings = payuSettings;
        }
        private async Task<string> GetAccessToken()
        {
            var clientId = payuSettings.Value.ClientId;
            var secret = payuSettings.Value.ClientSecret;

            var baseAddress = new Uri("https://secure.snd.payu.com/");
            using (var httpClient = new HttpClient {BaseAddress = baseAddress})
            {
                using (var content = new StringContent(
                    $"grant_type=client_credentials&client_id={clientId}&client_secret={secret}",
                    System.Text.Encoding.Default,
                    "application/x-www-form-urlencoded"))
                {
                    using (var response = await httpClient.PostAsync("/pl/standard/user/oauth/authorize", content))
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var deserializedResponse =
                                 JsonConvert.DeserializeObject<Dictionary<string, string>>(responseData);

                            var token = deserializedResponse["access_token"];

                            return token;
                        }
                    }
                }
            }
            return null;
        }

        public async Task<string> GetPayUOrderUrl(PayUOrder order)
        {
            var token = await GetAccessToken();
            var jsonOrder = JsonConvert.SerializeObject(order, Formatting.Indented,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            var baseAddress = new Uri("https://secure.snd.payu.com/api/v2_1/orders");

            using (var httpClient = new HttpClient(new HttpClientHandler(){AllowAutoRedirect = false}) { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using (var content = new StringContent(jsonOrder, System.Text.Encoding.Default, "application/json"))
                {
                    using (var response = await httpClient.PostAsync(baseAddress,content))
                    {
                        if (response.StatusCode == HttpStatusCode.Redirect ||
                            response.StatusCode == HttpStatusCode.MovedPermanently)
                        {
                            return response.Headers.Location.AbsoluteUri;
                        }
                    }
                }
            }
            throw new Exception("something went wrong :(");
        }
    }
}