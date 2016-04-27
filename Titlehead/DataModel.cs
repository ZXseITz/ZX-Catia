using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Titlehead
{
    class DataModel : UserControl
    {
        public static DependencyProperty PartNumberProperty =
            DependencyProperty.Register("PartNumber", typeof(string), typeof(DataModel));
        public string PartNumber
        {
            get { return (string)GetValue(PartNumberProperty); }
            set { SetValue(PartNumberProperty, value); }
        }

        public static DependencyProperty ProjectProperty =
            DependencyProperty.Register("Project", typeof(string), typeof(DataModel));
        public string Project
        {
            get { return (string)GetValue(ProjectProperty); }
            set { SetValue(ProjectProperty, value); }
        }

        public static DependencyProperty AssemblyProperty =
            DependencyProperty.Register("Assembly", typeof(string), typeof(DataModel));
        public string Assembly
        {
            get { return (string)GetValue(AssemblyProperty); }
            set { SetValue(AssemblyProperty, value); }
        }

        public static DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(DataModel));
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static DependencyProperty RevisionProperty =
            DependencyProperty.Register("Revision", typeof(string), typeof(DataModel));
        public string Revision
        {
            get { return (string)GetValue(RevisionProperty); }
            set { SetValue(RevisionProperty, value); }
        }

        public static DependencyProperty MaterialProperty =
            DependencyProperty.Register("Material", typeof(string), typeof(DataModel));
        public string Material
        {
            get { return (string)GetValue(MaterialProperty); }
            set { SetValue(MaterialProperty, value); }
        }
    }
}
