using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Legacy.JsonLocalization
{
	public static class StringLocalizerExtensions
    {
        public static LocalizedString GetString<TResource>(
            this IStringLocalizer stringLocalizer,
            Expression<Func<TResource, string>> propertyExpression)
            => stringLocalizer[(propertyExpression.Body as MemberExpression).Member.Name];
    }
}