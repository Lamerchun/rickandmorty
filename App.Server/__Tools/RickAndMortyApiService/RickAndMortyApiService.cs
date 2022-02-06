using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using R4;

namespace us;

public interface IRickAndMortyApiService : ISingletonService
{
	Task<CharacterResponse> FilterCachedAsync(string name, int page);
}

public class RickAndMortyApiService : IRickAndMortyApiService
{
	public IMemoryCache IMemoryCache { get; set; }
	public ISimpleWebClient ISimpleWebClient { get; set; }

	public async Task<CharacterResponse> FilterCachedAsync(string name, int page)
	{
		var key =
			$"Filter:{name?.Trim().ToLower()}:{page}";

		if (IMemoryCache.TryGetValue(key, out CharacterResponse cachedResponse))
			return cachedResponse;

		var response =
			await ISimpleWebClient.GetUrlAsStringAsync(
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

		IMemoryCache.Set(key, result);
		return result;
	}
}
