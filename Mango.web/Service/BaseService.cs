﻿using Mango.web.Models;
using Mango.web.Service.IService;
using Mango.Web.Models;
using Newtonsoft.Json;
using System.Text;
using Mango.web.Utility;
using System.Net;

namespace Mango.web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }
        public async Task<ResponseDto> SendAsync(RequestDto requestDto, bool withBearer = true )
        {
            try
            {
                
            HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            //token
            if(withBearer )
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }
            message.RequestUri = new Uri(requestDto.Url);

            if(requestDto.Data != null)
            { 
                message.Content= new StringContent(JsonConvert.SerializeObject(requestDto.Data),Encoding.UTF8,"application/json");
                //File.WriteAllText(@"c:\temp\file.txt", JsonConvert.SerializeObject(requestDto.Data));
            }

            HttpResponseMessage? apiResponse = null;
            switch(requestDto.ApiType)
            {
                case SD.ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;

                case SD.ApiType.DELETE: message.Method= HttpMethod.Delete; break;
                case SD.ApiType.PUT: message.Method= HttpMethod.Put; break;
                default: message.Method = HttpMethod.Get; break;
            }
            apiResponse = await client.SendAsync(message);
            
                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound: return new() { IsSuccess = false, Message = "Not found" };
                    case HttpStatusCode.Forbidden: return new() { IsSuccess = false, Message = "Access Denied" };
                    case HttpStatusCode.Unauthorized: return new() { IsSuccess = false, Message = "Unauthorized" };
                    case HttpStatusCode.InternalServerError: return new() { IsSuccess = false, Message = "Internal Server Error" };
                    //case HttpStatusCode.BadRequest: return new() { IsSuccess = false, Message = "Bad Request" };
                    default:
                        {
                            var apiContent = await apiResponse.Content.ReadAsStringAsync();
                            var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto?>(apiContent);
                            return apiResponseDto;
                        }

                }
            }
            catch(Exception ex)
            {
                var dto = new ResponseDto
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false

                };
                return dto;
            }
            

        }

    }
}
