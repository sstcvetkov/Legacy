using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Legacy.ResxValidator
{
	public class ResxValidator
	{
		public const char ValuesSeparator = ',';
		public const char RangeSeparator = '-';

		public enum Keywords
		{
			DisplayName,
			Message,
			Required,
			Pattern, 
			Length,
			MinLength,
			MaxLength,
			Range,
			MinValue,
			MaxValue,
			Values,
			Compare
		}

		private readonly Dictionary<Keywords, Func<string, string, bool>> _rules = 
			new Dictionary<Keywords, Func<string, string, bool>>()
		{
			[Keywords.Required] = (v, arg) => 
				!string.IsNullOrEmpty(v),
			[Keywords.Pattern] = (v, arg) => 
				!string.IsNullOrWhiteSpace(v) && Regex.IsMatch(v, arg),
			[Keywords.Range] = (v, arg) => 
				!string.IsNullOrWhiteSpace(v) && long.TryParse(v, out var vLong) &&
				long.TryParse(arg.Split(RangeSeparator)[0].Trim(), out var vMin) &&
				long.TryParse(arg.Split(RangeSeparator)[1].Trim(), out var vMax) &&
				vLong >= vMin && vLong <= vMax,
			[Keywords.Length] = (v, arg) => 
				!string.IsNullOrWhiteSpace(v) &&
				long.TryParse(arg.Split(RangeSeparator)[0].Trim(), out var vMin) &&
				long.TryParse(arg.Split(RangeSeparator)[1].Trim(), out var vMax) &&
				v.Length >= vMin && v.Length <= vMax,
			[Keywords.MinLength] = (v, arg) => 
				!string.IsNullOrWhiteSpace(v) && v.Length >= int.Parse(arg),
			[Keywords.MaxLength] = (v, arg) => 
				!string.IsNullOrWhiteSpace(v) && v.Length <= int.Parse(arg),
			[Keywords.Values] = (v, arg) => 
				!string.IsNullOrWhiteSpace(v) && 
				arg.Split(ValuesSeparator).Select(x => x.Trim()).Contains(v),
			[Keywords.MinValue] = (v, arg) => 
				!string.IsNullOrEmpty(v) && long.TryParse(v, out var vLong) &&
				long.TryParse(arg, out var argLong) && vLong >= argLong,
			[Keywords.MaxValue] = (v, arg) => 
				!string.IsNullOrEmpty(v) && long.TryParse(v, out var vLong) &&
				long.TryParse(arg, out var argLong) && vLong <= argLong
		};
		
		private readonly IStringLocalizer _localizer;

		public ResxValidator(IStringLocalizer localizer)
		{
			_localizer = localizer;
		}

		public bool IsValid(string memberName, string value, out string message)
		{
			var rules = _rules.Select(x => new 
				{ 
					Name = x.Key,
					Check = x.Value,
					String = _localizer.GetString(memberName + x.Key)
				}).Where(x => x.String != null && !x.String.ResourceNotFound);
			foreach (var rule in rules)
			{
				if (!rule.Check(value, rule.String?.Value))
				{
					var messageResourceKey = $"{memberName}{rule.Name}{Keywords.Message}";
					var messageResource = _localizer[messageResourceKey];
					var displayNameResourceKey = $"{memberName}{Keywords.DisplayName}";
					var displayNameResource = _localizer[displayNameResourceKey] ?? displayNameResourceKey;

					message = messageResource != null && !messageResource.ResourceNotFound
						? string.Format(messageResource.Value, displayNameResource, rule.String?.Value)
						: messageResourceKey;
					return false;
				}
			}

			message = null;
			return true;
		}
	}
}