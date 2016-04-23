using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EditProperties
{
    class FilterModel : UserControl
    {
        public static DependencyProperty ProjectProperty =
            DependencyProperty.Register("Project", typeof(bool), typeof(FilterModel));
        public bool Project
        {
            get { return (bool) GetValue(ProjectProperty);}
            set { SetValue(ProjectProperty, value);}
        }

        public static DependencyProperty AssemlbyProperty =
            DependencyProperty.Register("Assembly", typeof(bool), typeof(FilterModel));
        public bool Assembly
        {
            get { return (bool) GetValue(AssemlbyProperty);}
            set { SetValue(AssemlbyProperty, value);}
        }

        public static DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(bool), typeof(FilterModel));
        public bool Description
        {
            get { return (bool)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static DependencyProperty RevisionProperty =
            DependencyProperty.Register("Revision", typeof(bool), typeof(FilterModel));
        public bool Revision
        {
            get { return (bool)GetValue(RevisionProperty); }
            set { SetValue(RevisionProperty, value); }
        }

        public void SetFilter(bool project, bool assembly, bool description, bool revision)
        {
            this.Project = project;
            this.Assembly = assembly;
            this.Description = description;
            this.Revision = revision;
        }

        public bool[] GetFilter()
        {
            return new bool[]
            {
                    Project,
                    Assembly,
                    Description,
                    Revision
            };
        }

        public void CopyTo(FilterModel fm)
        {
            bool[] array = GetFilter();
            fm.SetFilter(array[0], array[1], array[2], array[3]);
        }
    }
}
