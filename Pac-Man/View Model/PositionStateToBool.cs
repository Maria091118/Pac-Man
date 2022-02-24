using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Pac_Man.View_Model
{
    public class PositionStateToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(parameter==null)
            {
                return false;
            }
            Model.PositionState p = (Model.PositionState)value;
           if(parameter == "Wall" && p== Model.PositionState.Wall )
            {
                return true;
            }
            if (parameter == "Common" && p == Model.PositionState.CommonBean)
            {
                return true;
            }
            if (parameter == "Special" && p == Model.PositionState.SpecialBean)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            if(parameter == "Wall" && b==true)
            {
                return Model.PositionState.Wall;
            }
            if (parameter == "Common" && b == true)
            {
                return Model.PositionState.CommonBean;
            }
            if (parameter == "Special" && b == true)
            {
                return Model.PositionState.SpecialBean;
            }
            return Model.PositionState.Space;
        }
    }
}
