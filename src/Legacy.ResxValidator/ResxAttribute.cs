using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace Legacy.ResxValidator
{
	public sealed class ResxAttribute : ValidationAttribute
	{
		private readonly string _baseName;
		private string _resourceName;

		public ResxAttribute(string sectionName, string resourceName = null)
		{
			_baseName = sectionName;
			_resourceName = resourceName;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)  
		{
			if (_resourceName == null)
				_resourceName = validationContext.MemberName;
			
			var factory = validationContext
				.GetService(typeof(IStringLocalizerFactory)) as IStringLocalizerFactory;
			var localizer = factory?.Create(_baseName,
				System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);

			ErrorMessage = ErrorMessageString;
			var currentValue = value as string;
			var validator = new ResxValidator(localizer);

			return validator.IsValid(_resourceName, currentValue, out var message)
				? ValidationResult.Success
				: new ValidationResult(message);
		}
	}
}
