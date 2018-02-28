using IMSContract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DispatcherApp.View.Resources.Converters
{
    public class CrewTypeToCrews : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<Crew> res = new ObservableCollection<Crew>();
            ObservableCollection<Crew> crews = new ObservableCollection<Crew>();
            IncidentReport report = new IncidentReport();
            CrewType type;

            try
            {
                crews = (ObservableCollection<Crew>)values[0];
                report = (IncidentReport)values[1];
                type = (CrewType)values[2];
            }
            catch { return null; }

            foreach (Crew crew in crews)
            {
                if (type == crew.Type && !crew.Working)
                {
                    res.Add(crew);
                }
            }

            return res;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
