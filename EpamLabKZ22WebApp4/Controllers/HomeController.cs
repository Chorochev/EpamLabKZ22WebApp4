using EpamLabKZ22WebApp4.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EpamLabKZ22WebApp4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private IConfigurationRoot ConfigRoot;
        public HomeController(IConfiguration configRoot, ILogger<HomeController> logger)
        {
            ConfigRoot = (IConfigurationRoot)configRoot;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            ViewData["SecretKeyFromAzure"] = GetSecretKeyFromAzure();
            ViewData["SecretFromAzure"] = GetSecretFromAzure();
            ViewData["AzureEpamLabTestVar"] = GetAzureEpamLabTestVar();
            var dataconnect = GetConnectionString();
            ViewData["dataconnect"] = dataconnect;
            var _connectionString = dataconnect[4] == "1" ? dataconnect[0] : dataconnect[2];
            try
            {
                EpamLabTestDbContext dBEntities = new EpamLabTestDbContext(_connectionString);
                ViewData["DBMessageID"] = dBEntities.KeyValues.First().Id;
                ViewData["DBMessageName"] = dBEntities.KeyValues.First().Name;
                ViewData["DBMessageInfo"] = dBEntities.KeyValues.First().Info;
                ViewData["DBError"] = "ok";
            }
            catch (Exception ex)
            {
                ViewData["DBMessageID"] = "error";
                ViewData["DBMessageName"] = "error";
                ViewData["DBMessageInfo"] = "error";
                ViewData["DBError"] = ex.ToString();
            }
            return View();
        }

        private string[] GetConnectionString()
        {           
            var result = new string[] { "connect1", "-", "connect2", "-", "0" };

            try
            {
                result[0] = ConfigRoot.GetConnectionString("DevSecretConnectionString") ?? "null";               
            }
            catch (Exception ex)
            {
                result[1] = "DevSecretConnectionString - ERROR: " + ex.ToString();
                result[4] = "1";
            }

            if (result[0] != "null") return result;

            try
            {
                result[2] = ConfigRoot.GetConnectionString("DefaultConnectionString") ?? "null";
                result[4] = "2";
            }
            catch (Exception ex)
            {
                result[3] = "DefaultConnectionString - ERROR: " + ex.ToString();
            }

            return result;
        }

        private string GetSecretKeyFromAzure()
        {
            try
            {
                Uri azureKyeVaultLink = new Uri(ConfigRoot.GetValue<string>("ApiSettings:AzureKyeVaultLink"));
                var client = new SecretClient(azureKyeVaultLink, new DefaultAzureCredential());
                KeyVaultSecret secret = client.GetSecret("AlekseiKhoroshevNumber1");
                string secretValue = secret.Value;
                return secretValue;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private string GetSecretFromAzure()
        {
            try
            {
                Uri azureKyeVaultLink = new Uri(ConfigRoot.GetValue<string>("ApiSettings:AzureKyeVaultLink"));
                var client = new SecretClient(azureKyeVaultLink, new DefaultAzureCredential());
                KeyVaultSecret secret = client.GetSecret("SecretVault1");
                string secretValue = secret.Value;
                return secretValue;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        //epamlabtestvar
        private string GetAzureEpamLabTestVar()
        {
            try
            {               
                string secretValue = ConfigRoot.GetValue<string>("epamlabtestvar", "not found!!!");
                return secretValue;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}