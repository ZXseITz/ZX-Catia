using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Titlehead
{
    class DrawingModel : UserControl
    {
        public static DependencyProperty DrawedDateProperty =
            DependencyProperty.Register("DrawedDate", typeof(string), typeof(DrawingModel));
        public string DrawedDate
        {
            get { return (string)GetValue(DrawedDateProperty); }
            set { SetValue(DrawedDateProperty, value); }
        }

        public static DependencyProperty DrawedNameProperty =
            DependencyProperty.Register("DrawedName", typeof(string), typeof(DrawingModel));
        public string DrawedName
        {
            get { return (string)GetValue(DrawedNameProperty); }
            set { SetValue(DrawedNameProperty, value); }
        }

        public static DependencyProperty CheckedDateProperty =
            DependencyProperty.Register("CheckedDate", typeof(string), typeof(DrawingModel));
        public string CheckedDate
        {
            get { return (string)GetValue(CheckedDateProperty); }
            set { SetValue(CheckedDateProperty, value); }
        }

        public static DependencyProperty CheckedNameProperty =
            DependencyProperty.Register("CheckedName", typeof(string), typeof(DrawingModel));
        public string CheckedName
        {
            get { return (string)GetValue(CheckedNameProperty); }
            set { SetValue(CheckedNameProperty, value); }
        }

        public static DependencyProperty ReleasedDateProperty =
            DependencyProperty.Register("ReleasedDate", typeof(string), typeof(DrawingModel));
        public string ReleasedDate
        {
            get { return (string)GetValue(ReleasedDateProperty); }
            set { SetValue(ReleasedDateProperty, value); }
        }

        public static DependencyProperty ReleasedNameProperty =
            DependencyProperty.Register("ReleasedName", typeof(string), typeof(DrawingModel));
        public string ReleasedName
        {
            get { return (string)GetValue(ReleasedNameProperty); }
            set { SetValue(ReleasedNameProperty, value); }
        }

        public static DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(DrawingModel));
        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        public static DependencyProperty ChangeSheetProperty =
            DependencyProperty.Register("ChangeSheet", typeof(bool), typeof(DrawingModel));
        public bool ChangeSheet
        {
            get { return (bool)GetValue(ChangeSheetProperty); }
            set { SetValue(ChangeSheetProperty, value); }
        }

        public static DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(string), typeof(DrawingModel));
        public string Scale
        {
            get { return (string)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static DependencyProperty ViewProperty =
            DependencyProperty.Register("View", typeof(string), typeof(DrawingModel));
        public string View
        {
            get { return (string)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }

        public static DependencyProperty SheetProperty =
            DependencyProperty.Register("Sheet", typeof(string), typeof(DrawingModel));
        public string Sheet
        {
            get { return (string)GetValue(SheetProperty); }
            set { SetValue(SheetProperty, value); }
        }
    }
}
