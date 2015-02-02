﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Windows.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using XEurope.JsonClasses;

namespace XEurope.Common
{
    public static class ConnHelper
    {
        /// <summary>
        /// Contains the Dtouch servivice base URL, ending with '/'
        /// </summary>
        public const String BaseUri = "http://xeurope.eitictlabs.hu/dtouch_service/v1/";
        public const String DtouchProcessUri = "http://www.dtouch.somee.com/api/dtouch/";

        public static string AddHttpToUrl(string inUrl)
        {
            inUrl = inUrl ?? String.Empty;
            if (!(inUrl.StartsWith("http://") || inUrl.StartsWith("https://")))
            {
                return "http://" + inUrl;
            }   
            return inUrl;
        }

        public static byte[] GetSampleImageBytes()
        {
            return
                Convert.FromBase64String(
                    "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/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAAAkJ/wDAABAcsI/AAAAJCf8AwAAQHLCPwAAACQn/AMAAEBywj8AAACk9vr1/wdomAfbvIz9fAAAAABJRU5ErkJggg==");
        }

        public static async Task<string> PostToUri(Uri uri, string postdata, string authHeader = null)
        {
            authHeader = authHeader ?? "c7dda4ed33ff8f5220ebb597cd823c01";
            var received = String.Empty;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "POST";
                if (!String.IsNullOrEmpty(authHeader))
                {
                    request.Headers["Authorization"] = authHeader;
                }
                byte[] data = Encoding.UTF8.GetBytes(postdata);
                //request.ContentLength = data.Length;
                request.ContentType = "application/json";

                using (var requestStream = await Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, request))
                {
                    await requestStream.WriteAsync(data, 0, data.Length);
                }

                WebResponse responseObject = null;
                try
                {
                    responseObject = await Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, request);
                    var responseStream = responseObject.GetResponseStream();
                    var sr = new StreamReader(responseStream);
                    received = await sr.ReadToEndAsync();
                }
                catch (WebException ex)
                {
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        //var sr = reader.ReadToEnd();
                        received = reader.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    received = JsonConvert.SerializeObject(new ErrorJson { error = true, message = ex.Message });
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                received = JsonConvert.SerializeObject(new ErrorJson {error = true, message = ex.Message});
            }

            return received;
        }

        public static async Task<string> GetFromUri(Uri url, string authHeader = null)
        {
            authHeader = authHeader ?? "c7dda4ed33ff8f5220ebb597cd823c01";
            var received = String.Empty;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                if (!String.IsNullOrEmpty(authHeader))
                {
                    request.Headers["Authorization"] = authHeader;
                }
                var response = (HttpWebResponse) await request.GetResponseAsync();

                var stream = new StreamReader(response.GetResponseStream());
                received = await stream.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                received = JsonConvert.SerializeObject(new ErrorJson { error = true, message = ex.Message });
            }
            return received;
        }
    }
}
