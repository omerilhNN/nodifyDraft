using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator
{
    public class StringToVisibilityConverter : IValueConverter
    {
        //ViewModelda olan değişikliği UI tarafına bildirmek için Convert fonksiyonu kullanıldı
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            //Eğer string boşsa bir şey gösterme değilse o zaman hata varsa onu göster
            return !string.IsNullOrEmpty(value as string);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
