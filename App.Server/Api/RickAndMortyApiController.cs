using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace us;

[ApiController]
public partial class RickAndMortyApiController : ControllerBase
{
	private readonly IRickAndMortyApiService _RickAndMortyApiService;

	public RickAndMortyApiController(IRickAndMortyApiService rickAndMortyApiService)
	{
		_RickAndMortyApiService = rickAndMortyApiService;
	}

	[HttpGet("Api/Character")]
	public async Task<IActionResult> Characters(string name, int page)
	{
		try
		{
			if (page < 1)
				page = 1;

			var response =
				await _RickAndMortyApiService.FilterCachedAsync(name, page);

			if (response == null)
				return NotFound();

			return Ok(response);
		}
		catch
		{
			return NotFound();
		}
	}
}
