using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using RickAndMorty.Net.Api.Factory;
using RickAndMorty.Net.Api.Service;
using RickAndMorty.Net.Api.Models.Domain;

namespace us
{
	[ApiController]
	public class RickAndMortyApiController : ControllerBase
	{
		private readonly IMemoryCache _IMemoryCache;
		private readonly IRickAndMortyService _IRickAndMortyService;

		public RickAndMortyApiController(IMemoryCache memoryCache)
		{
			_IMemoryCache = memoryCache;
			_IRickAndMortyService = RickAndMortyApiFactory.Create();
		}

		private Task<IEnumerable<Character>> FilterCachedAsync(string name)
			=>
			_IMemoryCache.GetOrCreateAsync(
				$"Filter:{name?.Trim().ToLower()}",
				async _ => await _IRickAndMortyService.FilterCharacters(name)
			);

		[HttpGet("Api/Character")]
		public async Task<IActionResult> Characters(string name, int page)
		{
			try
			{
				var characters =
					await FilterCachedAsync(name);

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
			catch
			{
				return NotFound();
			}
		}
	}
}
