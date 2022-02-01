using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using RickAndMorty.Net.Api.Factory;

namespace us
{
	[ApiController]
	public class RickAndMortyApiController : ControllerBase
	{
		[HttpGet("Api/Character")]
		public async Task<IActionResult> Characters(string name, int page)
		{
			var service =
				RickAndMortyApiFactory.Create();

			try
			{
				var characters =
					await service.FilterCharacters(name);

				var pageSize = 20;

				var total =
					characters.Count();

				var pageCount =
					(int)Math.Ceiling(total / (double)pageSize);

				if (page < 1)
					page = 1;

				if (page > pageCount)
					page = pageCount;

				var pagedCharacters =
					characters
						.Skip((page - 1) * pageSize)
						.Take(pageSize);

				return Ok(
					new
					{
						info = new
						{
							count = total,
							pages = pageCount,
							prev = page > 1,
							next = page < pageCount
						},
						results = pagedCharacters
					});
			}
			catch (NullReferenceException)
			{
				return NotFound();
			}
		}
	}
}
