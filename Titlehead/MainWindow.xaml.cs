using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using CATmain;
using INFITF;
using ProductStructureTypeLib;
using DRAFTINGITF;

namespace Titlehead
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private string title = "Titlehead";
        private PresentationModel model;
        private CATB catia;
        private DrawingDocument activeDocument;
        private DrawingSheet activeSheet;
        private Product propertiesProduct;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] filter = { CATB.filterDraw};
                catia = new CATB(filter);
                model = InitModel();
                activeDocument = (DrawingDocument) catia.getActiveDoc();
                if (activeDocument.Sheets.Count > 0)
                {
                    activeSheet = activeDocument.Sheets.ActiveSheet;
                    model.Drawing.Format = activeSheet.get_PaperName().Substring(0, 2);
                    model.Drawing.Sheet = InitSheetNumber();
                    for (int i = 3; i <= activeSheet.Views.Count; i++)
                    {
                        model.Views.Add(activeSheet.Views.Item(i).get_Name());
                    }

                }
                else
                {
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

        }

        private PresentationModel InitModel()
        {
            model = new PresentationModel();
            model.Data = (DataModel) FindResource("Data");
            model.Drawing = (DrawingModel) FindResource("Drawing");
            model.PartList = (PartListModel) FindResource("PartList");
            model.Views = (ViewCollection) FindResource("Views");
            model.Drawing.View += new EventHandler(View_Changed);
            return model;
        }

        private string InitSheetNumber()
        {
            int activeSheetNumber = 1;
            while (activeDocument.Sheets.Item(activeSheetNumber) != activeSheet)
            {
                activeSheetNumber++;
            }
            return activeSheetNumber + "/" + activeDocument.Sheets.Count;
        }

        private void View_Changed(object sender, EventArgs e)
        {
            DrawingView view = ((DrawingView) activeSheet.Views.GetItem(model.Drawing.View));
            if (view.Scale2 < 1)
            {
                model.Drawing.Scale = "1:" + (int) 1/view.Scale2;
            }
            else
            {
                model.Drawing.Scale = (int) view.Scale2 + ":1";
            }
            
        }
    }
}
