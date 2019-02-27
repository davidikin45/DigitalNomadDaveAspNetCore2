using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Drawing;
using System.Linq.Expressions;

namespace AspNetCore.Base.Data.Converters
{
    public class ColorToStringValueConverter : ValueConverter<Color, string>
    {
        public ColorToStringValueConverter()
        : base(ColorString, ColorStruct)
        {
        }

        private static Expression<Func<Color, string>>
            ColorString = v => new string(v.Name);

        private static Expression<Func<string, Color>>
            ColorStruct = x => Color.FromName(x);
    }
}
