using Microsoft.Extensions.Localization;
using Moq;
using System;
using Xunit;

namespace Legacy.ResxValidator.Tests
{
	public class ResxValidatorTests
	{
		private const string MemberName = "Name";
		private const string MinLengthRuleMessage = "{0} length should be greater then {1}";
		private const int MinLengthRuleValue = 2;
		private const string MaxLengthRuleMessage = "Value length should be less then {1}";
		private const int MaxLengthRuleValue = 10;
		private const string RequiredRuleMessage = "{0} required";
		private const string ValuesRuleFirstValue = "Adam";
		private const string ValuesRuleLastValue = "Eve";
		private readonly string _valuesRuleValue =
			$"{ValuesRuleFirstValue}{ResxValidator.ValuesSeparator} {ValuesRuleLastValue} ";
		private const string ValuesRuleMessage = "{0} is not valid. Possible values: {1}";
		private const string PatternRuleValue = @".+@.+";
		private const string PatternRuleMessage = "{0} is not valid.";
		private const string MinValueRuleMessage = "{0} should be greater then {1}";
		private const int MinValueRuleValue = 18;
		private const string MaxValueRuleMessage = "Value length should be less then {1}";
		private const int MaxValueRuleValue = 150;

		private readonly string _lengthRuleValue = 
			$"{MinLengthRuleValue}{ResxValidator.RangeSeparator} {MaxLengthRuleValue} ";
		private const string LengthRuleMessage = "Length of the name must be {1} characters";

		private const int MinRangeRuleValue = 2;
		private const int MaxRangeRuleValue = 50;
		private readonly string _rangeRuleValue = 
			$"{MinRangeRuleValue}{ResxValidator.RangeSeparator} {MaxRangeRuleValue} ";
		private const string RangeRuleMessage = "{0} must be in range: {1}";

		[Fact]
		public void Min_length_rule_negative()
		{
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, null)
				.SetUpRule(MemberName, MinLengthRuleValue, 
					ResxValidator.Keywords.MinLength, MinLengthRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value null
			var result = validator.IsValid(MemberName, null, out var gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(MinLengthRuleMessage,
				MemberName + ResxValidator.Keywords.DisplayName, MinLengthRuleValue));

			// value empty
			var value = string.Empty;
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(MinLengthRuleMessage,
				MemberName + ResxValidator.Keywords.DisplayName, MinLengthRuleValue));

			// value too short
			value = "a";
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(MinLengthRuleMessage,
				MemberName + ResxValidator.Keywords.DisplayName, MinLengthRuleValue));
		}

		[Fact]
		public void Min_length_rule_positive()
		{
			const string displayName = "User name";
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, displayName)
				.SetUpRule(MemberName, MinLengthRuleValue, 
					ResxValidator.Keywords.MinLength, MinLengthRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value min
			var value = new string('a', MinLengthRuleValue);
			var result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);

			// value more than min
			value = new string('a', MinLengthRuleValue + 1);
			result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);
		}

		[Fact]
		public void Max_length_rule_negative()
		{
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, null)
				.SetUpRule(MemberName, MaxLengthRuleValue, 
					ResxValidator.Keywords.MaxLength, MaxLengthRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value null
			var result = validator.IsValid(MemberName, null, out var gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(
				MaxLengthRuleMessage, null, MaxLengthRuleValue));

			// value empty
			var value = string.Empty;
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(
				MaxLengthRuleMessage, null, MaxLengthRuleValue));

			// value too long
			value = new string('a', MaxLengthRuleValue + 1);
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(
				MaxLengthRuleMessage, null, MaxLengthRuleValue));
		}

		[Fact]
		public void Max_length_rule_positive()
		{
			const string displayName = "User name";
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, displayName)
				.SetUpRule(MemberName, MaxLengthRuleValue,
					ResxValidator.Keywords.MaxLength, MaxLengthRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value max
			var value = new string('a', MaxLengthRuleValue);
			var result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);

			// value less than max
			value = new string('a', MaxLengthRuleValue - 1);
			result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);
		}

		[Fact]
		public void Required_rule_negative()
		{
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, null)
				.SetUpRule(MemberName, null, 
					ResxValidator.Keywords.Required, RequiredRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value null
			var result = validator.IsValid(MemberName, null, out var gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(
				RequiredRuleMessage, MemberName + ResxValidator.Keywords.DisplayName));

			// value empty
			var value = string.Empty;
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(
				RequiredRuleMessage, MemberName + ResxValidator.Keywords.DisplayName));
		}

		[Fact]
		public void Required_rule_positive()
		{
			const string displayName = "User name";
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, displayName)
				.SetUpRule(MemberName, null,
					ResxValidator.Keywords.Required, RequiredRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value
			const string value = "a";
			var result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);
		}

		[Fact]
		public void Values_rule_negative()
		{
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, null)
				.SetUpRule(MemberName, _valuesRuleValue, 
					ResxValidator.Keywords.Values, ValuesRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value null
			var result = validator.IsValid(MemberName, null, out var gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(ValuesRuleMessage,
				MemberName + ResxValidator.Keywords.DisplayName, _valuesRuleValue));

			// value empty
			var value = string.Empty;
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(ValuesRuleMessage,
				MemberName + ResxValidator.Keywords.DisplayName, _valuesRuleValue));

			// value incorrect
			value = "a";
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(ValuesRuleMessage,
				MemberName + ResxValidator.Keywords.DisplayName, _valuesRuleValue));
		}

		[Fact]
		public void Values_rule_positive()
		{
			const string displayName = "User name";
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, displayName)
				.SetUpRule(MemberName, _valuesRuleValue,
					ResxValidator.Keywords.Values, ValuesRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value
			var value = ValuesRuleFirstValue;
			var result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);

			// value with spaces
			value = ValuesRuleLastValue;
			result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);
		}

		[Fact]
		public void Pattern_rule_negative()
		{
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, null)
				.SetUpRule(MemberName, PatternRuleValue, 
					ResxValidator.Keywords.Pattern, PatternRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value null
			var result = validator.IsValid(MemberName, null, out var gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(
				PatternRuleMessage, MemberName + ResxValidator.Keywords.DisplayName));

			// value empty
			var value = string.Empty;
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(
				PatternRuleMessage, MemberName + ResxValidator.Keywords.DisplayName));

			// value incorrect
			value = "a";
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(PatternRuleMessage,
				MemberName + ResxValidator.Keywords.DisplayName));
		}

		[Fact]
		public void Pattern_rule_positive()
		{
			const string displayName = "User email";
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, displayName)
				.SetUpRule(MemberName, PatternRuleValue, 
					ResxValidator.Keywords.Pattern, ValuesRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value
			const string value = "a@a.com";
			var result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);
		}

		[Fact]
		public void Min_value_rule_negative()
		{
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, null)
				.SetUpRule(MemberName, MinValueRuleValue, 
					ResxValidator.Keywords.MinValue, MinValueRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value null
			var result = validator.IsValid(MemberName, null, out var gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(MinValueRuleMessage,
				MemberName + ResxValidator.Keywords.DisplayName, MinValueRuleValue));

			// value empty
			var value = string.Empty;
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(MinValueRuleMessage,
				MemberName + ResxValidator.Keywords.DisplayName, MinValueRuleValue));

			// value is not a number
			value = "a";
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(MinValueRuleMessage,
				MemberName + ResxValidator.Keywords.DisplayName, MinValueRuleValue));

			// value too small
			value = (MinValueRuleValue - 1).ToString();
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(MinValueRuleMessage,
				MemberName + ResxValidator.Keywords.DisplayName, MinValueRuleValue));
		}

		[Fact]
		public void Min_value_rule_positive()
		{
			var displayName = "User name";
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, displayName)
				.SetUpRule(MemberName, MinValueRuleValue, 
					ResxValidator.Keywords.MinValue, MinValueRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value min
			var value = MinValueRuleValue.ToString();
			var result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);

			// value more than min
			value = (MinValueRuleValue + 1).ToString();
			result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);
		}

		[Fact]
		public void Max_value_rule_negative()
		{
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, null)
				.SetUpRule(MemberName, MaxValueRuleValue, 
					ResxValidator.Keywords.MaxValue, MaxValueRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value null
			var result = validator.IsValid(MemberName, null, out var gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(
				MaxValueRuleMessage, null, MaxValueRuleValue));

			// value empty
			var value = string.Empty;
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(
				MaxValueRuleMessage, null, MaxValueRuleValue));

			// value is not a number
			value = "a";
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(
				MaxValueRuleMessage, null, MaxValueRuleValue));

			// value too small
			value = (MaxValueRuleValue + 1).ToString();
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(
				MaxValueRuleMessage, null, MaxValueRuleValue));
		}

		[Fact]
		public void Max_value_rule_positive()
		{
			const string displayName = "User name";
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, displayName)
				.SetUpRule(MemberName, MaxValueRuleValue, 
					ResxValidator.Keywords.MaxValue, MaxValueRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value max
			var value = MaxValueRuleValue.ToString();
			var result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);

			// value more than max
			value = (MaxValueRuleValue - 1).ToString();
			result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);
		}

		[Fact]
		public void Length_rule_negative()
		{
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, null)
				.SetUpRule(MemberName, _lengthRuleValue, 
					ResxValidator.Keywords.Length, LengthRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value null
			var result = validator.IsValid(MemberName, null, out var gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(
				LengthRuleMessage, null, _lengthRuleValue));

			// value empty
			var value = string.Empty;
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(
				LengthRuleMessage, null, _lengthRuleValue));

			// value too short
			value = new string('a', MinLengthRuleValue - 1);
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(
				LengthRuleMessage, null, _lengthRuleValue));

			// value too long
			value = new string('a', MaxLengthRuleValue + 1);
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(
				LengthRuleMessage, null, _lengthRuleValue));
		}

		[Fact]
		public void Length_rule_positive()
		{
			const string displayName = "User name";
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, displayName)
				.SetUpRule(MemberName, _lengthRuleValue, 
					ResxValidator.Keywords.Length, LengthRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value min
			var value = new string('a', MinLengthRuleValue);
			var result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);

			// value middle
			value = new string('a',  new Random()
				.Next(MinLengthRuleValue,MaxLengthRuleValue));
			result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);

			// value max
			value = new string('a', MaxLengthRuleValue);
			result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);
		}

		[Fact]
		public void Range_rule_negative()
		{
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, null)
				.SetUpRule(MemberName, _rangeRuleValue,
					ResxValidator.Keywords.Range, RangeRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value null
			var result = validator.IsValid(MemberName, null, out var gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(RangeRuleMessage,
				MemberName + ResxValidator.Keywords.DisplayName, _rangeRuleValue));

			// value empty
			var value = string.Empty;
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(RangeRuleMessage,
				MemberName + ResxValidator.Keywords.DisplayName, _rangeRuleValue));

			// value too small
			value = (MinRangeRuleValue - 1).ToString();
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(RangeRuleMessage,
				MemberName + ResxValidator.Keywords.DisplayName, _rangeRuleValue));

			// value too big
			value = (MaxRangeRuleValue + 1).ToString();
			result = validator.IsValid(MemberName, value, out gotMessage);
			Assert.False(result);
			Assert.Equal(gotMessage, string.Format(RangeRuleMessage,
				MemberName + ResxValidator.Keywords.DisplayName, _rangeRuleValue));
		}

		[Fact]
		public void Range_rule_positive()
		{
			const string displayName = "User name";
			var mockLocalizer = new Mock<IStringLocalizer>()
				.SetUpDisplayName(MemberName, displayName)
				.SetUpRule(MemberName, _rangeRuleValue,
					ResxValidator.Keywords.Range, RangeRuleMessage);
			var validator = new ResxValidator(mockLocalizer.Object);

			// value min
			var value = MinRangeRuleValue.ToString();
			var result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);

			// value middle
			value = new Random().Next(MinRangeRuleValue, MaxRangeRuleValue).ToString();
			result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);

			// value max
			value = MaxRangeRuleValue.ToString();
			result = validator.IsValid(MemberName, value, out _);
			Assert.True(result);
		}
	}
}
