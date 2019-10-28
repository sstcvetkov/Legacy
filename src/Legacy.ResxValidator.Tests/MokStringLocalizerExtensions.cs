using Microsoft.Extensions.Localization;
using Moq;

namespace Legacy.ResxValidator.Tests
{
	public static class MokStringLocalizerExtensions
	{
		public static Mock<IStringLocalizer> SetUpDisplayName(this Mock<IStringLocalizer> localizer,
			string memberName, string value)
		{
			if (value != null)
				localizer.SetUpString(memberName, value, 
					ResxValidator.Keywords.DisplayName.ToString());

			return localizer;
		}

		public static Mock<IStringLocalizer> SetUpRule(this Mock<IStringLocalizer> localizer,
			string memberName, object value, ResxValidator.Keywords suffix, string message)
		{
			localizer
				.SetUpString(memberName, value?.ToString(), suffix.ToString())
				.SetUpString(memberName, message,
					suffix + ResxValidator.Keywords.Message.ToString());

			return localizer;
		}

		private static Mock<IStringLocalizer> SetUpString(this Mock<IStringLocalizer> localizer,
			string memberName, string value, string suffix)
		{
			var key = $"{memberName}{suffix}";
			localizer.SetupGet(p => p[It.Is<string>(x => x == key)])
				.Returns(new LocalizedString(memberName, value ?? string.Empty, false));

			return localizer;
		}
	}
}
