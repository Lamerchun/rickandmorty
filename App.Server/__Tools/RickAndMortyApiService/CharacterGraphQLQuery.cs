using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using HotChocolate;

namespace us;

public class CharacterGraphQLQuery
{
	public async Task<CharacterResponse> GetCharacters(
		[Service] ILogger<CharacterGraphQLQuery> logger,
		[Service] IRickAndMortyApiService rickAndMortyApiService,
		int page,
		Filter filter)
	{
		var response =
			await rickAndMortyApiService.FilterCachedAsync(filter.Name, page);

		if (response.Results.Count == 0)
			logger.LogInformation($"GraphQL no results: name: {filter.Name}, page: {page}");

		foreach (var result in response.Results)
		{
			if (result.Episodes == null)
				continue;

			result.Episode =
				result.Episodes.Select(x => new CharacterEpisode { Name = x });
		}

		return response;
	}

	public class Filter
	{
		public string Name { get; set; }
	}
}
