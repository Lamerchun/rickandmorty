using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

namespace R4;

public static class ServiceCollectionExtensions
{
	public static void AddCompression(this IServiceCollection services)
	{
		services.AddResponseCompression(options =>
		{
			var MimeTypes = new[]
			{
					 "text/plain",
					 "text/html",
					 "text/json",
					 "text/css",
					 "font/woff2",
					 "image/png",
					 "image/x-icon",
					 "image/svg+xml",
					 "application/json",
					 "application/javascript",
					 "application/octet-stream"
				 };

			options.EnableForHttps = true;
			options.MimeTypes = MimeTypes;
			options.Providers.Add<GzipCompressionProvider>();
			options.Providers.Add<BrotliCompressionProvider>();
		});

		services.Configure<GzipCompressionProviderOptions>(options =>
		{
			options.Level = CompressionLevel.Optimal;
		});

		services.Configure<BrotliCompressionProviderOptions>(options =>
		{
			options.Level = CompressionLevel.Optimal;
		});
	}
}
