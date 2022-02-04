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
		return 
			await rickAndMortyApiService.FilterCachedAsync(filter.Name, page);
	}

	public class Filter
	{
		public string Name { get; set; }
	}
}
