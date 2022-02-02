using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace us
{
	[ApiController]
	public partial class RickAndMortyApiController : ControllerBase
	{
		private readonly IMemoryCache _IMemoryCache;
		private readonly SimpleWebClient _SimpleWebClient = new();

		public RickAndMortyApiController(IMemoryCache memoryCache)
		{
			_IMemoryCache = memoryCache;
		}

		private Task<CharacterResponse> FilterCachedAsync(string name, int page)
			=>
			_IMemoryCache.GetOrCreateAsync(
				$"Filter:{name?.Trim().ToLower()}:{page}",
				async _ =>
				{
					var rawResponse =
						await _SimpleWebClient.GetUrlAsync(
							$"https://rickandmortyapi.com/api/character/",
							new Dictionary<string, string>
							{
								{ "name", name },
								{ "page", page.ToString() }
							});

					return rawResponse.ToObjectByJson<CharacterResponse>();
				}
			);

		[HttpGet("Api/Character")]
		public async Task<IActionResult> Characters(string name, int page)
		{
			try
			{
				if (page < 1)
					page = 1;

				var response =
					await FilterCachedAsync(name, page);

				return Ok(
					new
					{
						info = new
						{
							count = response.Info.Count,
							pages = response.Info.Pages,
							prev = !string.IsNullOrWhiteSpace(response.Info.Prev),
							next = !string.IsNullOrWhiteSpace(response.Info.Next)
						},
						results = response.Results
					});
			}
			catch
			{
				return NotFound();
			}
		}
	}
}
