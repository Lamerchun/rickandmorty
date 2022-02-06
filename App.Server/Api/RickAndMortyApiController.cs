using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace us;

[ApiController]
public partial class RickAndMortyApiController : ControllerBase
{
	public IRickAndMortyApiService IRickAndMortyApiService { get; set; }

	[HttpGet("Api/Character")]
	public async Task<IActionResult> Characters(string name, int page)
	{
		try
		{
			if (page < 1)
				page = 1;

			var response =
				await IRickAndMortyApiService.FilterCachedAsync(name, page);

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
