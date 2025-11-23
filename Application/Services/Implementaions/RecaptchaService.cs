using Application.Services.Interfaces;
using Domain.Shared;
using Domain.Statics;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Nodes;

namespace Application.Services.Implementaions
{
    public class RecaptchaService : IRecaptchaService
    {
        #region Fields
        private readonly IConfiguration _configuration;
        #endregion

        #region Ctor
        public RecaptchaService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        public async Task<Result> VerifyRecaptchaV2(string response)
        {
            using (var client = new HttpClient())
            {
                var secret = _configuration["googleReCaptcha:SecretKey"];
                MultipartFormDataContent content = new();
                content.Add(new StringContent(response), "response");
                content.Add(new StringContent(secret), "secret");

                var result = await client.PostAsync(SiteTools.RecaptchaVerifyUrl, content);

                if (result.IsSuccessStatusCode)
                {
                    var strResponse = await result.Content.ReadAsStringAsync();

                    var jsonResponse = JsonNode.Parse(strResponse);
                    if (jsonResponse is not null)
                    {
                        var success = ((bool?)jsonResponse["success"]);
                        if (success is not null && success == true) return Result.Success();
                    }
                }
            }

            return Result.Failure(Messages.CaptchaFailedError);
        }
    }
}
