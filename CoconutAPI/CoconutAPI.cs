using RestSharp;
using System.Net;
using Newtonsoft.Json;
using RestSharp.Authenticators;

namespace Coconut
{
	public class CoconutAPI
	{
	    private RestClient _cli;

		/// <summary>
		/// Create an CoconutAPI instance
		/// </summary>
		/// <param name="apiKey"></param>
		/// <example>
		/// CoconutAPI Coconut = new CoconutAPI("k-myapikey");
		/// </example>
		public CoconutAPI (string apiKey)
		{
		    _cli = new RestClient("https://api.coconut.co")
		    {
		        Authenticator = new HttpBasicAuthenticator(apiKey, "")
		    };
		    _cli.AddDefaultHeader("Accept", "application/json");
			_cli.UserAgent = "Coconut/2.0.0 (dotnet)";
		}


		/// <summary>
		/// Create a Job
		/// </summary>
		/// <param name="data">A string representing the config content</param>
		/// <returns>CoconutJob instance</returns>
		public CoconutJob Submit(string data)
		{
			return JsonConvert.DeserializeObject<CoconutJob>(Request("v1/job", Method.POST, data));
		}

		private string Request(string path, RestSharp.Method method, string data) {
			var request = new RestRequest(path, method);
			request.AddParameter("text/plain", data, ParameterType.RequestBody);

			var response = _cli.Execute(request);
			var code = response.StatusCode;

			if(code == HttpStatusCode.Created || code == HttpStatusCode.OK || code == HttpStatusCode.NoContent)
			{
				return response.Content;
			}
			else
			{
				var ErrorMessage = JsonConvert.DeserializeObject<CoconutError> (response.Content).Message;
				throw new CoconutException(ErrorMessage);
			}

		}
	}
}