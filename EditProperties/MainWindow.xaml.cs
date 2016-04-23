using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using MECMOD;
using Microsoft.Win32;
using PARTITF;
using ProductStructureTypeLib;
using File = System.IO.File;
using Window = System.Windows.Window;

namespace EditProperties
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string title = "Edit prperties";
        private CATB catia;
        private PresentationModel model;
        private static readonly string filePath = Directory.GetCurrentDirectory() + "\\" + title + "Settings.txt";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //CATIA
            try
            {
                string[] filter = { CATB.filterPart, CATB.filterProd };
                catia = new CATB(filter);
                model = initModel();
                model.List.CollectionChanged += new NotifyCollectionChangedEventHandler(list_CollectionChanged);
                //load settings
                if (File.Exists(filePath))
                {
                    Regex regex = new Regex(".*=([Tt]rue|[Ff]alse)");
                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            if (line != null && regex.IsMatch(line))
                            {
                                string[] setStrings = line.Split('=');
                                try
                                {
                                    switch (setStrings[0])
                                    {
                                        case "DefaultImportProject":
                                            model.DefaultImportFilter.Project = Convert.ToBoolean(setStrings[1]);
                                            break;
                                        case "DefaultImportAssembly":
                                            model.DefaultImportFilter.Assembly = Convert.ToBoolean(setStrings[1]);
                                            break;
                                        case "DefaultImportDescription":
                                            model.DefaultImportFilter.Description = Convert.ToBoolean(setStrings[1]);
                                            break;
                                        case "DefaultImportRevision":
                                            model.DefaultImportFilter.Revision = Convert.ToBoolean(setStrings[1]);
                                            break;

                                        case "DefaultSaveProject":
                                            model.DefaultSaveFilter.Project = Convert.ToBoolean(setStrings[1]);
                                            break;
                                        case "DefaultSaveAssembly":
                                            model.DefaultSaveFilter.Assembly = Convert.ToBoolean(setStrings[1]);
                                            break;
                                        case "DefaultSaveDescription":
                                            model.DefaultSaveFilter.Description = Convert.ToBoolean(setStrings[1]);
                                            break;
                                        case "DefaultSaveRevision":
                                            model.DefaultSaveFilter.Revision = Convert.ToBoolean(setStrings[1]);
                                            break;
                                    }
                                }
                                catch (Exception)
                                {
                                    throw new InvalidCastException("Value cannot be converted into boolean");
                                }
                            }
                        }
                    }
                }
                else
                {
                    LoadDefaultSettings();
                }
                //Copy settings
                CopySettings();
                //initialize text
                Selection sel = catia.getSelection();
                Product firstProduct;
                if (sel.Count2 == 0)
                {
                    if (catia.getActiveDoc() is PartDocument)
                    {
                        firstProduct = ((PartDocument)catia.getActiveDoc()).Product;
                    }
                    else
                    {
                        firstProduct = ((ProductDocument)catia.getActiveDoc()).Product;
                    }
                    AddToList(firstProduct);
                    ImportProperties(firstProduct, new bool[] { true, true, true, true });
                }
                else
                {
                    bool flag = true;
                    for (int i = 1; i <= sel.Count2; i++)
                    {
                        if (sel.Item2(i).Value is Part || sel.Item2(i).Value is Product)
                        {
                            firstProduct = ConvertToProduct((AnyObject)sel.Item2(i).Value);
                            if (flag)
                            {
                                ImportProperties(firstProduct, new bool[] { true, true, true, true });
                                flag = false;
                            }
                            AddToList(firstProduct);
                        }
                    }
                }
            }
            catch (InvalidCastException ice)
            {
                MessageBox.Show(ice.Message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
                LoadDefaultSettings();
                CopySettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }

        private PresentationModel initModel()
        {
            PresentationModel model = FindResource("Model") as PresentationModel;
            model.NewProperties = FindResource("NewProperties") as NewPropertiesModel;
            model.ImportFilter = FindResource("Import") as FilterModel;
            model.SaveFilter = FindResource("Save") as FilterModel;
            model.DefaultImportFilter = FindResource("DefaultImport") as FilterModel;
            model.DefaultSaveFilter = FindResource("DefaultSave") as FilterModel;
            model.List = FindResource("List") as PropertiesProductCollection;
            model.Savemode = PresentationModel.Savemodi.SaveModeUndef;
            model.EnableDescription = true;
            return model;
        }

        private void LoadDefaultSettings()
        {
            model.DefaultImportFilter.SetFilter(true, true, true, false);
            model.DefaultSaveFilter.SetFilter(true, true, true, true);
        }

        /// <summary>
        /// Copies the default setting to the normal settings
        /// </summary>
        private void CopySettings()
        {
            model.DefaultImportFilter.CopyTo(model.ImportFilter);
            model.DefaultSaveFilter.CopyTo(model.SaveFilter);
        }

        /// <summary>
        /// Casts a part into product or it's already a product
        /// </summary>
        /// <param partOrProduct="obj"></param>
        /// <returns>product</returns>
        private static Product ConvertToProduct(AnyObject obj)
        {
            if (obj is Part)
            {
                return ((PartDocument)((Part)(obj)).Parent).Product;
            }
            else
            {
                return (Product)obj;
            }
        }

        /// <summary>
        /// Adds a product to List, like set
        /// </summary>
        /// <param product="product"></param>
        private void AddToList(Product product)
        {
            if (model.List.All(p => p.product != product))
            {
                model.List.Add(new PropertiesProduct(product));
            }
        }

        /// <summary>
        /// Reads the properties of a product
        /// </summary>
        /// <param product="prod"></param>
        /// <returns>properties</returns>
        private static string[] ReadProperties(Product prod)
        {
            return new string[]
            {
                prod.get_PartNumber(),
                prod.get_Definition(),
                prod.get_Nomenclature(),
                prod.get_DescriptionRef(),
                prod.get_Revision()
            };
        }

        /// <summary>
        /// Imports each property, which is accepted by the filter
        /// </summary>
        /// <param product="prod"></param>
        /// <param name="importFilter"></param>
        private void ImportProperties(Product prod, bool[] importFilter)
        {
            string[] properties = ReadProperties(prod);
            //part number
            if (!CheckPartNum(properties[0]))
            {
                model.NewProperties.PartNumber = properties[0];
            }
            else
            {
                string[] pNums = properties[0].Split('.');
                string[] newPNums = new string[3];
                for (int i = 0; i < importFilter.Length - 1; i++)
                {
                    if (importFilter[i])
                    {
                        newPNums[i] = pNums[i];
                    }
                    else
                    {
                        newPNums[i] = String.Empty;
                    }
                }
                model.NewProperties.PartNumber = string.Join(".", newPNums);
            }
            //Project
            if (importFilter[0])
            {
                model.NewProperties.Project = properties[1];
            }
            //Assembly
            if (importFilter[1])
            {
                model.NewProperties.Assembly = properties[2];
            }
            //Description
            if (importFilter[2])
            {
                model.NewProperties.Description = properties[3];
            }
            //Revision
            if (importFilter[3])
            {
                model.NewProperties.Revision = properties[4];
            }
        }

        /// <summary>
        /// Changes the Savemode if the List has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void list_CollectionChanged(object sender, EventArgs e)
        {
            if (model.List.Count == 0)
            {
                model.ChangeSavemode(PresentationModel.Savemodi.SaveModeUndef);
            }
            else if (model.List.Count == 1)
            {
                model.ChangeSavemode(PresentationModel.Savemodi.SaveModeSingle);
            }
            else
            {
                model.ChangeSavemode(PresentationModel.Savemodi.SaveModeMulti);
            }
        }

        /// <summary>
        /// Checks if the part number matches the public format
        /// </summary>
        /// <param partnumber="partNum"></param>
        /// <returns>match</returns>
        public static bool CheckPartNum(String partNum)
        {
            Regex regex = new Regex("\\d{3}" + Regex.Escape(".") + "\\d{3}" + Regex.Escape(".") + "\\d{4}");
            return regex.IsMatch(partNum);
        }

        /// <summary>
        /// Imports the properties of a part or product, which is loaded in CATIA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BImportCat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Selection sel = catia.getSelection();
                sel.Clear();
                Object[] selFilter = { "Product", "Part" };
                sel.SelectElement2(selFilter, "Select part or product", false);
                if (sel.Count2 > 0)
                {
                    ImportProperties(ConvertToProduct((AnyObject)sel.Item2(1).Value), model.ImportFilter.GetFilter());
                }
                sel.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Imports the properties of a .CATPart or .CATProduct file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BImportFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog fDialog = new OpenFileDialog();
                fDialog.Filter =
                    "CATIA part or product files (*.CATPart; *.CATProduct)|*.CATPart;*.CATProduct";
                fDialog.Title = title;
                fDialog.Multiselect = false;
                bool? res = fDialog.ShowDialog();
                if (!res.GetValueOrDefault(false))
                {
                    throw new ArgumentNullException("No file selected");
                }
                //Load document in CATIA, works also if documet is already loadad in CATIA
                Document doc = catia.getDocs().Read(fDialog.FileName);
                //Import properties
                Product prod;
                if (doc is ProductDocument)
                {
                    prod = ((PartDocument)doc).Product;
                }
                else
                {
                    prod = ((ProductDocument)doc).Product;
                }
                ImportProperties(prod, model.ImportFilter.GetFilter());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Adds a new Item to the save List
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BAddItems_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Selection sel = catia.getSelection();
                sel.Clear();
                Object[] selFilter = { "Product", "Part" };
                sel.SelectElement3(selFilter, "Select part or product", false, CATMultiSelectionMode.CATMultiSelTriggWhenSelPerf, true);
                for (int i = 1; i <= sel.Count2; i++)
                {
                    AddToList(ConvertToProduct((AnyObject)sel.Item2(i).Value));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Removes items from the save List
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BRemoveItems_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //backward for-loop required
                int[] indices = new int[LbItems.SelectedItems.Count];
                for (int i = LbItems.SelectedItems.Count - 1; i >= 0; i--)
                {
                    model.List.RemoveAt(LbItems.Items.IndexOf(LbItems.SelectedItems[i]));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Saves the specified propeties in each product in the save List
        /// No specified Description and item number can be saved in the multi Savemode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (model.Savemode)
                {
                    case PresentationModel.Savemodi.SaveModeSingle:
                        model.List[0].writeProperties(model.NewProperties.GetProperties(), model.SaveFilter.GetFilter(), true);
                        break;
                    case PresentationModel.Savemodi.SaveModeMulti:
                        foreach (PropertiesProduct prod in model.List)
                        {
                            prod.writeProperties(model.NewProperties.GetProperties(), model.SaveFilter.GetFilter(), false);
                        }
                        break;
                }
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void saveDefaultSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    var settings = File.Create(filePath);
                    settings.Close();
                }
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.WriteLine("DefaultImportProject=" + model.DefaultImportFilter.Project);
                    sw.WriteLine("DefaultImportAssembly=" + model.DefaultImportFilter.Assembly);
                    sw.WriteLine("DefaultImportDescription=" + model.DefaultImportFilter.Description);
                    sw.WriteLine("DefaultImportRevision=" + model.DefaultImportFilter.Revision);

                    sw.WriteLine("DefaultSaveProject=" + model.DefaultSaveFilter.Project);
                    sw.WriteLine("DefaultSaveAssembly=" + model.DefaultSaveFilter.Assembly);
                    sw.WriteLine("DefaultSaveDescription=" + model.DefaultSaveFilter.Description);
                    sw.WriteLine("DefaultSaveRevision=" + model.DefaultSaveFilter.Revision);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
