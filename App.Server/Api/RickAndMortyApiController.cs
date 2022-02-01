using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using RickAndMorty.Net.Api.Factory;

namespace us
{
	[ApiController]
	public class RickAndMortyApiController : ControllerBase
	{
		[HttpGet("Api/Character")]
		public async Task<IActionResult> Characters(string name)
		{
			var service =
				RickAndMortyApiFactory.Create();

			try
			{
				var characters =
					await service.FilterCharacters(name);

				return Ok(
					new
					{
						results = characters
					});
			}
			catch (NullReferenceException)
			{
				return NotFound();
			}
		}
	}
}
