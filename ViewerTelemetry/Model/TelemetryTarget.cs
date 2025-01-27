using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ViewerTelemetry.Model.Contracts;
using ViewerTelemetry.Model.Vitapur;

namespace ViewerTelemetry.Model
{
    public abstract class TelemetryTarget<T> where T : ITelemetryData
    {
        //public abstract string TargetFileLocation { get; set; }
        //public abstract string Title { get; set; }
        //public abstract string Url { get; set; }
        //public abstract HttpMethod Method { get; set; }

        //public abstract HttpContent HttpContent { get; set; }
        public abstract Task<T> SendRequest(/*string url, HttpContent data, HttpMethod method*/);

        public abstract Task AppendValueToFile(T value);
    }

    public class VitapurHybrid20TelemetryTarget : TelemetryTarget<VisitorData>
    {
        public VitapurHybrid20TelemetryTarget()
        {
            this.Title = "Hibridno ležišče Vitapur Zero Gravity 2.0 Firm";
            this.TargetFileLocation =
                $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}//Telemetry//vitapur.log";
            this.Url = "https://www.vitapur.si/vproductviews/view/visitors/";
            this.Method = HttpMethod.Post; 
        }

        public  string TargetFileLocation { get; set; }

        public  string Title { get; set; }
        
        public  string Url { get; set; }

        public  HttpMethod Method { get; set; }
        
        public  HttpContent HttpContent { get; set; } = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("product", "13149"),
            new KeyValuePair<string, string>("form_key", "93vGvCloOdQRBo27")
        });

        public override async Task<VisitorData> SendRequest(/*string url, HttpContent data, HttpMethod method*/)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(this.Method, this.Url)
                {
                    Content = this.HttpContent
                };

                request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

                HttpResponseMessage response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    return null;

                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
                {
                    return await JsonSerializer.DeserializeAsync<VisitorData>(stream);
                }
            }
        }

        public override async Task AppendValueToFile(VisitorData value)
        {
            if (!File.Exists(TargetFileLocation))
            {
                var dirName = Path.GetDirectoryName(TargetFileLocation);
                if(!Directory.Exists(dirName))
                    Directory.CreateDirectory(dirName);

                File.Create(TargetFileLocation);
            }

            // Append the value to the file
            using (StreamWriter writer = new StreamWriter(TargetFileLocation, append: true))
            {
                var str = $"[{DateTime.Now:dd. MM. yyyy @ HH:mm:ss}]: {value.Visitors}";
                Console.WriteLine(str);
                await writer.WriteLineAsync(str);
            }
        }
    }
}
