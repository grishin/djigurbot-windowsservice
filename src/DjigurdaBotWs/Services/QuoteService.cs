﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using DjigurdaBotWs.Models;

namespace DjigurdaBotWs.Services
{
    public interface IQuoteService
    {
        Quote GetRandomQuote();
    }

    public class QuoteService : IQuoteService
    {
        public Quote GetRandomQuote()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.forismatic.com/api/1.0/");
                var result = client.GetAsync("?method=getQuote&format=json&lang=ru").Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                
                var resultJsonObject = JObject.Parse(resultContent);
                string text = resultJsonObject["quoteText"].ToString();
                string author = resultJsonObject["quoteAuthor"].ToString();

                return new Quote
                {
                    Author = author,
                    Text = text
                };
            }
        }
    }
}
