using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace us
{
	public class SimpleWebClient
	{
		#region Tools

		private void EnsureHeaders(CustomWebClient client)
		{
			var headers =
				new Dictionary<string, string> { { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36" } };

			foreach (var header in headers)
				client.Headers.Add(header.Key, header.Value);
		}

		private class CustomWebClient : WebClient
		{
			protected override WebRequest GetWebRequest(Uri address)
			{
				var result = base.GetWebRequest(address);
				result.Proxy = null;

				if (result is HttpWebRequest httpWebRequest)
					httpWebRequest.ServicePoint.ConnectionLimit = 50;

				return result;
			}
		}

		#endregion

		#region Read Url

		public async Task<string> GetUrlAsync(string url, Dictionary<string, string> parameters)
		{
			string result = null;

			using var client = new CustomWebClient();
			EnsureHeaders(client);

			var pairs = 
				parameters.Select(x => $"{x.Key}={x.Value}");

			var queryString =
				string.Join("&", pairs);

			url += "?" + queryString;

			using var stream = client.OpenRead(url);
			using var reader = new StreamReader(stream);
			result = await reader.ReadToEndAsync();

			return result;
		}

		#endregion
	}
}
