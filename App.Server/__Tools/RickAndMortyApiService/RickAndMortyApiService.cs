using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace us;

public interface IRickAndMortyApiService
{
	Task<CharacterResponse> FilterCachedAsync(string name, int page);
}

public class RickAndMortyApiService : IRickAndMortyApiService
{
	private readonly IMemoryCache _IMemoryCache;
	private readonly ISimpleWebClient _SimpleWebClient;

	public RickAndMortyApiService(
		IMemoryCache memoryCache,
		ISimpleWebClient simpleWebClient)
	{
		_IMemoryCache = memoryCache;
		_SimpleWebClient = simpleWebClient;
	}

	public async Task<CharacterResponse> FilterCachedAsync(string name, int page)
	{
		var key =
			$"Filter:{name?.Trim().ToLower()}:{page}";

		if (_IMemoryCache.TryGetValue(key, out CharacterResponse cachedResponse))
			return cachedResponse;

		var response =
			await _SimpleWebClient.GetUrlAsStringAsync(
				$"https://rickandmortyapi.com/api/character/",
				new Dictionary<string, string>
				{
					{ "name", name },
					{ "page", page.ToString() }
				});

		if (response.Status != HttpStatusCode.OK)
			return default;

		var result =
			response.Content.ToObjectByJson<CharacterResponse>();

		_IMemoryCache.Set(key, result);
		return result;
	}
}
