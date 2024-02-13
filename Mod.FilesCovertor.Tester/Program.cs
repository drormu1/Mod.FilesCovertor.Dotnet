using Mod.FilesCovertor.Server;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

class Program
{
    static readonly string _apiEndpoint = "http://localhost:5048/Convert";
    static async Task Main()
    {
       foreach (string file in Directory.GetFiles(@"Samples"))
        {
            if (file.Contains(".pptx"))
            {
              

                await CallWebApipPost(file,".pdf");
                await CallWebApipPost(file, ".png");


            }
        }
        
    }

   
    static async Task CallWebApipPost(string file,string toExtension)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                Byte[] bytes = File.ReadAllBytes(file);
                String fileAs64 = Convert.ToBase64String(bytes);

                ConvertRequest convertRequest = new ConvertRequest { 
                    ContentAs64 = fileAs64,
                    FileName= Path.GetFileName(file),
                    ToExtension = toExtension
                };
                string json = JsonConvert.SerializeObject(convertRequest);   //using Newtonsoft.Json

                StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                
                // Make a GET request to the Web API
                HttpResponseMessage response = await client.PostAsync(_apiEndpoint, httpContent);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the content as a string
                    string b64StrRes = await response.Content.ReadAsStringAsync();

                   
                    string resPath = Path.Combine(Environment.CurrentDirectory, "uploads", Path.GetFileName(convertRequest.FileName).Replace('.','_'))+  convertRequest.ToExtension;
                    Byte[] bytesRes = Convert.FromBase64String(b64StrRes);
                    File.WriteAllBytes(resPath, bytesRes);

                     
                    // Print the API response
                    Console.WriteLine(resPath);
                }
                else
                { 
                    
                    // Print the error status code
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }

    async Task CallWebApiGet()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                // Make a GET request to the Web API
                HttpResponseMessage response = await client.GetAsync(_apiEndpoint);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the content as a string
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    // Print the API response
                    Console.WriteLine(apiResponse);
                }
                else
                {
                    // Print the error status code
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }
}