using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Titlehead
{
    class PartListModel : UserControl
    {
        public static DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(DataModel));
        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        public static DependencyProperty PartListPartNumberProperty =
            DependencyProperty.Register("PartListPartNumber", typeof(string), typeof(DataModel));
        public string PartListPartNumber
        {
            get { return (string)GetValue(PartListPartNumberProperty); }
            set { SetValue(PartListPartNumberProperty, value); }
        }
    }
}
