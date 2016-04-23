using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EditProperties
{
    class NewPropertiesModel : UserControl
    {
        public static DependencyProperty PartNumberProperty =
            DependencyProperty.Register("PartNumber", typeof(string), typeof(NewPropertiesModel));
        public string PartNumber {
            get {return (string) GetValue(PartNumberProperty); }
            set {SetValue(PartNumberProperty, value); }
        }

        public static DependencyProperty ProjectProperty =
            DependencyProperty.Register("Project", typeof(string), typeof(NewPropertiesModel));
        public string Project
        {
            get { return (string)GetValue(ProjectProperty); }
            set { SetValue(ProjectProperty, value); }
        }

        public static DependencyProperty AssemblyProperty =
            DependencyProperty.Register("Assembly", typeof(string), typeof(NewPropertiesModel));
        public string Assembly
        {
            get { return (string)GetValue(AssemblyProperty); }
            set { SetValue(AssemblyProperty, value); }
        }

        public static DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(NewPropertiesModel));
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static DependencyProperty RevisionProperty =
            DependencyProperty.Register("Revision", typeof(string), typeof(NewPropertiesModel));

        public string Revision
        {
            get { return (string)GetValue(RevisionProperty); }
            set { SetValue(RevisionProperty, value); }
        }

        public void SetProperties(string partNumber, string project, string assembly, string description,
            string revision)
        {
            this.PartNumber = partNumber;
            this.Project = project;
            this.Assembly = assembly;
            this.Description = description;
            this.Revision = revision;
        }

        public string[] GetProperties()
        {
            return new string[]
            {
                    PartNumber,
                    Project,
                    Assembly,
                    Description,
                    Revision
            };
        }
    }
}
