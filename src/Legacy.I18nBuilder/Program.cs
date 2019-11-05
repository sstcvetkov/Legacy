using Legacy.JsonLocalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Legacy.I18nBuilder
{
	class Program
	{
		/// <summary>
		/// Main
		/// </summary>
		/// <param name="args">json/resx inputDir outputDir exceptionDir1 ...</param>
		static void Main(string[] args)
		{
			if (args == null || args.Length < 3)
				throw new ArgumentException("Valid arguments: {json/resx} {input directory} {output directory}");

			if (!Directory.Exists(args[1]))
				throw new ArgumentException("Input directory does not exist.");

			if (!Directory.CreateDirectory(args[2]).Exists)
				throw new ArgumentException("Failed to create output directory.");

			var exclude = args.Length > 3 
				? args.Skip(3) : Array.Empty<string>();

			CheckResourceType(args[0]);

			var resources = Directory.GetFiles(args[1], $"*.{args[0]}", SearchOption.AllDirectories)
				.Where(x => !exclude.Any(z => x.StartsWith(z)))
				.Select(x => x.Replace(args[1] + Path.DirectorySeparatorChar, string.Empty))
				.Select(x => x.Substring(0, x.IndexOf('.'))).Distinct();

			var cultures = Directory.GetFiles(args[1], $"*.{args[0]}", SearchOption.AllDirectories)
				.Where(x => !exclude.Any(z => x.StartsWith(z)))
				.Select(x => x.Replace(args[1] + Path.DirectorySeparatorChar, string.Empty))
				.Select(x => x.Replace($".{args[0]}", string.Empty))
				.Where(x => x.Contains( "."))
				.Select(x => x.Substring(x.LastIndexOf('.') + 1, 5)).Distinct();

			foreach (var culture in cultures)
			{
				CultureInfo.CurrentCulture = new CultureInfo(culture);
				CultureInfo.CurrentUICulture = new CultureInfo(culture);
				var result = new Dictionary<string, Dictionary<string, string>>();
				foreach (var resource in resources)
				{
					var section = new JsonStringLocalizer(args[1], resource, NullLogger.Instance)
						.GetAllStrings()
						.OrderBy(x => x.Name)
						.ToDictionary(x => x.Name, x => x.Value);
					result.Add(resource, section);
				}
				var resultString = JsonConvert.SerializeObject(result, Formatting.Indented);
				var path = Path.Combine(args[2], $"{culture}.json");
				File.WriteAllText(path, resultString, System.Text.Encoding.UTF8);
			}
		}

		static void CheckResourceType(string resourceType)
		{
			switch (resourceType)
			{
				case "json":
					break;
				case "resx":
					break;
				default:
					throw new ArgumentException($"Unsupported resource type: {resourceType}");
			}
		}
	}
}
