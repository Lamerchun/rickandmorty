using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using Moq;
using us;

namespace Tests.Unit;

public class RickAndMortyApiServiceTests : UnitTestBase<RickAndMortyApiService>
{
	private const string _FakeResponse =
		"{\"info\":{\"count\":1,\"pages\":1,\"next\":null,\"prev\":null},\"results\":[{\"id\":10,\"name\":\"Alan Rails\",\"status\":\"Dead\",\"species\":\"Human\",\"type\":\"Superhuman (Ghost trains summoner)\",\"gender\":\"Male\",\"origin\":{\"name\":\"unknown\",\"url\":\"\"},\"location\":{\"name\":\"Worldender's lair\",\"url\":\"https://rickandmortyapi.com/api/location/4\"},\"image\":\"https://rickandmortyapi.com/api/character/avatar/10.jpeg\",\"episode\":[\"https://rickandmortyapi.com/api/episode/25\"],\"url\":\"https://rickandmortyapi.com/api/character/10\",\"created\":\"2017-11-04T20:19:09.017Z\"}]}";

	[Test]
	public async Task FilterCachedAsync_InvokesApi_And_SetsCache_If_Uncached()
	{
		// Arrange

		Container.GetOrCreateMock<ISimpleWebClient>()
			.Setup(x => x.GetUrlAsStringAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
			.ReturnsAsync(new SimpleHttpResponse<string> { Status = HttpStatusCode.OK, Content = _FakeResponse });

		var memoryCache =
			new MemoryCache(new MemoryCacheOptions());

		Service.IMemoryCache =
			memoryCache;

		// Act
		await Service.FilterCachedAsync("x", 1);

		// Assert

		Container.GetOrCreateMock<ISimpleWebClient>()
			.Verify(x => x.GetUrlAsStringAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Once);

		Assert.AreEqual(1, memoryCache.Count);
	}

	[Test]
	public async Task FilterCachedAsync_DoesNotInvokeApi_If_Cached()
	{
		// Arrange

		var memoryCache =
			new MemoryCache(new MemoryCacheOptions());

		Service.IMemoryCache =
			memoryCache;

		Container.GetOrCreateMock<ISimpleWebClient>()
			.Setup(x => x.GetUrlAsStringAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
			.ReturnsAsync(new SimpleHttpResponse<string> { Status = HttpStatusCode.OK, Content = _FakeResponse });

		await Service.FilterCachedAsync("x", 1);
		Container.GetOrCreateMock<ISimpleWebClient>().Reset();

		// Act
		await Service.FilterCachedAsync("x", 1);

		// Assert

		Container.GetOrCreateMock<ISimpleWebClient>()
			.Verify(x => x.GetUrlAsStringAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);

		Assert.AreEqual(1, memoryCache.Count);
	}

	[Test]
	public async Task FilterCachedAsync_Parses_ApiResponse_Correctly()
	{
		// Arrange

		var memoryCache =
			new MemoryCache(new MemoryCacheOptions());

		Service.IMemoryCache =
			memoryCache;

		Container.GetOrCreateMock<ISimpleWebClient>()
			.Setup(x => x.GetUrlAsStringAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
			.ReturnsAsync(new SimpleHttpResponse<string> { Status = HttpStatusCode.OK, Content = _FakeResponse });

		// Act
		var response = await Service.FilterCachedAsync("x", 1);

		// Assert
		Assert.AreEqual(1, response.Info.Count);
		Assert.AreEqual(1, response.Info.Pages);
		Assert.AreEqual(1, response.Results.Count);
		Assert.AreEqual("Alan Rails", response.Results[0].Name);
		Assert.AreEqual("Dead", response.Results[0].Status);
		Assert.AreEqual("Human", response.Results[0].Species);
		Assert.AreEqual("Superhuman (Ghost trains summoner)", response.Results[0].Type);
		Assert.AreEqual("Male", response.Results[0].Gender);
		Assert.AreEqual("https://rickandmortyapi.com/api/character/avatar/10.jpeg", response.Results[0].Image);
	}
}
