using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace us;

public interface ISimpleWebClient
{
	Task<SimpleHttpResponse<string>> GetUrlAsStringAsync(string url, Dictionary<string, string> parameters);
}

public class SimpleWebClient : ISimpleWebClient, IDisposable
{
	private readonly HttpClient _HttpClient = new();

	public SimpleWebClient()
	{
		var headers =
			new Dictionary<string, string> { { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36" } };

		foreach (var header in headers)
			_HttpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
	}

	public async Task<SimpleHttpResponse<string>> GetUrlAsStringAsync(string url, Dictionary<string, string> parameters)
	{
		var pairs =
			parameters.Select(x => $"{x.Key}={x.Value}");

		var queryString =
			string.Join("&", pairs);

		url += "?" + queryString;

		using var response =
			await _HttpClient.GetAsync(url);

		var content =
			await response.Content.ReadAsStringAsync();

		return new SimpleHttpResponse<string>
		{
			Content = content,
			Status = response.StatusCode
		};
	}

	public void Dispose()
	{
		_HttpClient.Dispose();
	}
}

public class SimpleHttpResponse<T>
{
	public T Content { get; set; }
	public HttpStatusCode Status { get; set; }
}
