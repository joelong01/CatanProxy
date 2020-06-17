using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Catan.Proxy
{
    /// <summary>
    ///     A proxy shared by client and service.  This is in both projects and can be found in https://github.com/joelong/catan
    /// </summary>
    public partial class CatanProxy : IDisposable
    {

        public HttpClient Client { get; set; } = new HttpClient() { Timeout = TimeSpan.FromDays(1) };
        private readonly CancellationTokenSource _cts = new CancellationTokenSource(TimeSpan.FromDays(1));
        public TimeSpan Timeout { get => Client.Timeout; set => Client.Timeout = value; }
        public string HostName { get; set; } // "http://localhost:5000";
        public CatanResult LastError { get; set; } = null;
        public string LastErrorString { get; set; } = "";
        public static string ProxyVersion { get; } = "1.11";

        public CatanProxy()
        {
        }
        private async Task<T> Post<T>(string url, string body)
        {
            if (String.IsNullOrEmpty(HostName))
            {
                throw new ArgumentException("HostName cannot be null!");
            }

            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentException("the URL can't be null or empty");
            }



            LastError = null;
            LastErrorString = "";
            string json = "";

            try
            {
                HttpResponseMessage response;
                if (body != null)
                {
                    response = await Client.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"), _cts.Token);
                }
                else
                {
                    response = await Client.PostAsync(url, new StringContent("", Encoding.UTF8, "application/json"));
                }

                if (typeof(T) == typeof(void)) return default;
                
                json = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    if (typeof(T) == typeof(string))
                    {
                        T workaround = (T)(object)json;
                        return workaround;
                    }

                    T obj = CatanProxy.Deserialize<T>(json);
                    return obj;
                }
                else
                {
                    LastErrorString = json;
                    CatanMessage error = CatanProxy.Deserialize<CatanMessage>(json);
                    LastError = (CatanResult)ParseCatanMessage(error);
                    return default;
                }
            }
            catch (Exception e)
            {
                LastErrorString = $"Url: {url} Response: {json} Exception:{e}";
                return default;
            }
        }
      
        private async Task<T> Delete<T>(string url)
        {

            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentException("the URL can't be null or empty");
            }



            LastError = null;
            LastErrorString = "";
            string json = "";
            try
            {

                var response = await Client.DeleteAsync(url, _cts.Token);
                json = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    T obj = CatanProxy.Deserialize<T>(json);
                    return obj;
                }
                else
                {
                    LastErrorString = json;
                    try
                    {
                        CatanMessage error = CatanProxy.Deserialize<CatanMessage>(json);
                        LastError = (CatanResult)ParseCatanMessage(error)  ;
                    }
                    catch { }
                    return default;
                }
            }
            catch (Exception e)
            {
                LastErrorString = $"Url: {url} Response: {json} Exception:{e}";
                return default;

            }
        }
        private async Task<T> Get<T>(string url)
        {


            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentException("the URL can't be null or empty");
            }



            LastError = null;
            LastErrorString = "";
            string json = "";
            try
            {
                var response = await Client.GetAsync(url, _cts.Token);
                if (response.IsSuccessStatusCode)
                {
                    json = await response.Content.ReadAsStringAsync();

                    if (typeof(T) == typeof(string))
                    {
                        T workaround = (T)(object)json;
                        return workaround;
                    }
                    T obj = CatanProxy.Deserialize<T>(json);
                    return obj;
                }
                else
                {
                    Debug.WriteLine($"Error grom GetAsync: {response} {Environment.NewLine} {response.ReasonPhrase}");
                }


            }
            catch (HttpRequestException)
            {
                // see if there is a Catan Exception

                LastErrorString = json;
                try
                {
                    CatanMessage error = CatanProxy.Deserialize<CatanMessage>(json);
                    LastError = (CatanResult)ParseCatanMessage(error);
                }
                catch
                {
                    return default;
                }

            }
            catch (Exception e)
            {
                LastErrorString = $"Url: {url} Response: {json} Exception:{e}";
                return default;
            }
            return default;
        }

       

        public void CancelAllRequests()
        {
            _cts.Cancel();
        }
        public void Dispose()
        {
            CancelAllRequests();
            Client.Dispose();
        }

       

       
        public static JsonSerializerOptions GetJsonOptions(bool indented = false)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = indented

            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
        static public string Serialize<T>(T obj, bool indented = false)
        {
            if (obj == null) return null;
            return JsonSerializer.Serialize<T>(obj, GetJsonOptions(indented));
        }
        static public T Deserialize<T>(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IgnoreNullValues = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Deserialize<T>(json, options);
        }
     
    }
}
