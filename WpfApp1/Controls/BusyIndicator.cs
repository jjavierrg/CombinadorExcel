using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Controls
{
    public class BusyIndicator : UserControl
    {
        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty BusyTextProperty = DependencyProperty.Register("BusyText", typeof(string), typeof(BusyIndicator), new PropertyMetadata("Loading ...."));

        public string BusyText
        {
            get => (string) GetValue(BusyTextProperty);
            set => SetValue(BusyTextProperty, value);
        }

        public bool IsBusy
        {
            get => (bool) GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }
    }
}
