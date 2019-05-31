﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using MvcApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private new static readonly DefaultHttpContext httpContext = new DefaultHttpContext();

        public HomeController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult  Logout()
        {
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = "Home/Index"
            }, "oidc", "Cookies");
            //AuthenticationProperties au = new AuthenticationProperties{RedirectUri = "Home/Index"};
            //await HttpContext.SignOutAsync("Cookies", au);
            //await HttpContext.SignOutAsync("oidc", au);
            //change 2
        }

        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/Home/Index"
            }, "oidc");
        }
        
        public async Task<IActionResult> Get()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:21021/api/services/app/DeviceType/GetDeviceTypes");
            var token = HttpContext.GetTokenAsync("access_token").Result;
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            object jsoned;
            if (!response.IsSuccessStatusCode)
            {
                jsoned = JsonConvert.DeserializeObject(content);
                return View(jsoned);
            }
            jsoned = JsonConvert.DeserializeObject(content);
            return View(jsoned);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
