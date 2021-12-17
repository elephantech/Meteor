using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Ttp.Meteor.MeteorSwath;

namespace MeteorSwathTestApp
{
    class SwathGeometryToStringConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }

            SwathGeometryModel sgm = value as SwathGeometryModel;
            if (sgm != null)
            {
                if (sgm.Exists)
                {
                    return String.Format("{0}: {1}", sgm.Index, sgm.Name);
                }
                else
                {
                    return String.Format("{0}: -----", sgm.Index);
                }
            }
            else if (value is AutoSwathGeometry)
            {
                return "AutoGeometry";
            }
            else
            {
                throw new ArgumentException("Arguement to converter must be SwathGeometryModel or AutoSwathGeometry");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
