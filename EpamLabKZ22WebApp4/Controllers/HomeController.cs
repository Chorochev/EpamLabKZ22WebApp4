using EpamLabKZ22WebApp4.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;

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
            ViewData["SecretSecretFromAzure"] = GetSecretSecretFromAzure();
            string _connectionString = GetConnectionString();
            ViewData["ConnectionString"] = _connectionString;
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

        private string GetConnectionString()
        {
            string result = "-";
            try
            {
                result = ConfigRoot.GetConnectionString("DefaultConnectionString") ?? "empty";
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            return result;
        }

        private string GetSecretKeyFromAzure()
        {
            try { 
            var client = new SecretClient(new Uri("https://epamlabkz2022keyvault.vault.azure.net/"), new DefaultAzureCredential());
            KeyVaultSecret secret = client.GetSecret("SecretKeyVault1");
            string secretValue = secret.Value;
            return secretValue;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private string GetSecretSecretFromAzure()
        {
            try
            {
                var client = new SecretClient(new Uri("https://epamlabkz2022keyvault.vault.azure.net/"), new DefaultAzureCredential());
                KeyVaultSecret secret = client.GetSecret("SecretVault1");
                string secretValue = secret.Value;
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