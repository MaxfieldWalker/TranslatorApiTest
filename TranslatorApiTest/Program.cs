using Microsoft.Translator.API;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace TranslatorApiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var key = "cf9801edf3964445884bf066b8729baa";
            var token = GetToken(key);

            var translatorService = new ServiceReference1.LanguageServiceClient();

            var rootNode = new LanguageRootNode("ja", translatorService, token);

            var chain = Enumerable.Range(0, 3).Select(_ =>
            {
                var enNode = new LanguageNode("en");
                var itNode = new LanguageNode("it");
                var jaNode = new LanguageEndNode("ja");

                enNode.Connect(itNode).Connect(jaNode);

                return (enNode, jaNode);
            }).Aggregate((a, b) =>
            {
                var bottom = a.Item2;
                var top = b.Item1;

                bottom.Connect(top);

                return (a.Item1, b.Item2);
            });

            rootNode.Connect(chain.Item1);

            string text = System.IO.File.ReadAllText(@"momotaro.txt");
            var result = rootNode.Translate(text);

            Console.WriteLine(result);
            Console.ReadKey();
        }

        static string GetToken(string key)
        {
            var authTokenSource = new AzureAuthToken(key);
            var token = string.Empty;

            try
            {
                token = authTokenSource.GetAccessToken();
            }
            catch (HttpRequestException)
            {
                switch (authTokenSource.RequestStatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        Console.WriteLine("Request to token service is not authorized (401). Check that the Azure subscription key is valid.");
                        break;
                    case HttpStatusCode.Forbidden:
                        Console.WriteLine("Request to token service is not authorized (403). For accounts in the free-tier, check that the account quota is not exceeded.");
                        break;
                }
                throw;
            }

            return token;
        }
    }
}
