using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EditProperties
{
    class PresentationModel : UserControl
    {
        public class Savemodi
        {
            public const string SaveModeUndef = "undefined";
            public const string SaveModeSingle = "single";
            public const string SaveModeMulti = "multi";
        }

        public static DependencyProperty EnableDescriptionProperty =
            DependencyProperty.Register("EnableDescription", typeof(bool), typeof(PresentationModel));
        public bool EnableDescription
        {
            get { return (bool) GetValue(EnableDescriptionProperty); }
            set { SetValue(EnableDescriptionProperty, value);}
        }

        public static DependencyProperty SavemodeProperty =
            DependencyProperty.Register("Savemode", typeof(string), typeof(PresentationModel));
        public string Savemode
        {
            get { return (string) GetValue(SavemodeProperty); }
            set { SetValue(SavemodeProperty, value);}
        }

        public NewPropertiesModel NewProperties { get; set; }
        public PropertiesProductCollection List { get; set; }
        public FilterModel ImportFilter { get; set; }
        public FilterModel SaveFilter { get; set; }
        public FilterModel DefaultImportFilter { get; set; }
        public FilterModel DefaultSaveFilter { get; set; }

        public void ChangeSavemode(string newSavemode)
        {
            Savemode = newSavemode;
            switch (newSavemode)
            {
                case Savemodi.SaveModeUndef:
                    EnableDescription = true;
                    break;
                case Savemodi.SaveModeSingle:
                    EnableDescription = true;
                    break;
                case Savemodi.SaveModeMulti:
                    EnableDescription = false;
                    break;
            }
        }
    }
}
