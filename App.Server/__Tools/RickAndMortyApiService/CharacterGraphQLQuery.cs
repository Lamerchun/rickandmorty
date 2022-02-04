using System.Linq;
using System.Threading.Tasks;
using HotChocolate;

namespace us;

public class CharacterGraphQLQuery
{
	public async Task<CharacterResponse> GetCharacters(
		[Service] IRickAndMortyApiService rickAndMortyApiService,
		int page,
		Filter filter)
	{
		var response =
			await rickAndMortyApiService.FilterCachedAsync(filter.Name, page);

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
