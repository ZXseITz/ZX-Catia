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
        private static readonly string filePath = Directory.GetCurrentDirectory() + "\\" + title + "Settings.txt";
        private PropertiesProductCollection list;
        private const string saveModeUndef = "undefined";
        private const string saveModeSingle = "single";
        private const string saveModeMulti = "multi";

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
                list = FindResource("ressource") as PropertiesProductCollection;
                list.CollectionChanged += new NotifyCollectionChangedEventHandler(list_CollectionChanged);
                //load settings
                if (File.Exists(filePath))
                {
                    Regex regex = new Regex(".*=([Tt]rue|[Ff]alse)");
                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        while (sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            if (line != null && regex.IsMatch(line))
                            {
                                string[] setStrings = line.Split('=');
                                try
                                {
                                    switch (setStrings[0])
                                    {
                                        case "DefaultImportProduct":
                                            ChBdImportProject.IsChecked = Convert.ToBoolean(setStrings[1]);
                                            break;
                                        case "DefaultImportAssembly":
                                            ChBdImportProject.IsChecked = Convert.ToBoolean(setStrings[1]);
                                            break;
                                        case "DefaultImportDescription":
                                            ChBdImportProject.IsChecked = Convert.ToBoolean(setStrings[1]);
                                            break;
                                        case "DefaultImportRevision":
                                            ChBdImportProject.IsChecked = Convert.ToBoolean(setStrings[1]);
                                            break;

                                        case "DefaultSaveProduct":
                                            ChBdImportProject.IsChecked = Convert.ToBoolean(setStrings[1]);
                                            break;
                                        case "DefaultSaveAssembly":
                                            ChBdImportProject.IsChecked = Convert.ToBoolean(setStrings[1]);
                                            break;
                                        case "DefaultSaveDescription":
                                            ChBdImportProject.IsChecked = Convert.ToBoolean(setStrings[1]);
                                            break;
                                        case "DefaultSaveRevision":
                                            ChBdImportProject.IsChecked = Convert.ToBoolean(setStrings[1]);
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

        private void LoadDefaultSettings()
        {
            ChBdImportProject.IsChecked = true;
            ChBdImportAssem.IsChecked = true;
            ChBdImportDesc.IsChecked = true;
            ChBdImportRev.IsChecked = false;

            ChBdSaveProject.IsChecked = true;
            ChBdSaveAssem.IsChecked = true;
            ChBdSaveDesc.IsChecked = true;
            ChBdSaveRev.IsChecked = true;
        }

        /// <summary>
        /// Copies the default setting to the normal settings
        /// </summary>
        private void CopySettings()
        {
            ChBImportProject.IsChecked = ChBdImportProject.IsChecked;
            ChBImportAssem.IsChecked = ChBdImportAssem.IsChecked;
            ChBImportDesc.IsChecked = ChBdImportDesc.IsChecked;
            ChBImportRev.IsChecked = ChBdImportRev.IsChecked;

            ChBSaveProject.IsChecked = ChBdSaveProject.IsChecked;
            ChBSaveAssem.IsChecked = ChBdSaveAssem.IsChecked;
            ChBSaveDesc.IsChecked = ChBdSaveDesc.IsChecked;
            ChBSaveRev.IsChecked = ChBdSaveRev.IsChecked;
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
        /// Adds a product to list, like set
        /// </summary>
        /// <param product="product"></param>
        private void AddToList(Product product)
        {
            if (list.All(p => p.product != product))
            {
                list.Add(new PropertiesProduct(product));
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
                TbPartNum.Text = properties[0];
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
                TbPartNum.Text = string.Join(".", newPNums);
            }
            //project
            if (importFilter[0])
            {
                TbProject.Text = properties[1];
            }
            //assembly
            if (importFilter[1])
            {
                TbAssem.Text = properties[2];
            }
            //description
            if (importFilter[2])
            {
                TbDesc.Text = properties[3];
            }
            //revision
            if (importFilter[3])
            {
                CbRev.Text = properties[4];
            }
        }

        /// <summary>
        /// Changes the savemode if the list has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void list_CollectionChanged(object sender, EventArgs e)
        {
            if (list.Count == 0)
            {
                TbSaveMode.Text = saveModeUndef;
                ChBSaveDesc.IsEnabled = true;
            }
            else if (list.Count == 1)
            {
                TbSaveMode.Text = saveModeSingle;
                ChBSaveDesc.IsEnabled = true;
            }
            else
            {
                TbSaveMode.Text = saveModeMulti;
                ChBSaveDesc.IsEnabled = false;
            }
        }

        private bool[] getImportFilter()
        {
            return new bool[]
            {
                ChBImportProject.IsChecked.Value,
                ChBImportAssem.IsChecked.Value,
                ChBImportDesc.IsChecked.Value,
                ChBImportRev.IsChecked.Value,
            };
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
                    ImportProperties(ConvertToProduct((AnyObject)sel.Item2(1).Value), getImportFilter());
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
                ImportProperties(prod, getImportFilter());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Adds a new Item to the save list
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
        /// Removes items from the save list
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
                    list.RemoveAt(LbItems.Items.IndexOf(LbItems.SelectedItems[i]));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Saves the specified propeties in each product in the save list
        /// No specified description and item number can be saved in the multi savemode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (TbSaveMode.Text)
                {
                    case saveModeSingle:
                        list[0].writeProperties(ReadSpecifiedProperties(), getSaveFilter(), true);
                        break;
                    case saveModeMulti:
                        foreach (PropertiesProduct prod in list)
                        {
                            prod.writeProperties(ReadSpecifiedProperties(), getSaveFilter(), false);
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
        
        private string[] ReadSpecifiedProperties()
        {
            return new string[] { TbPartNum.Text, TbProject.Text, TbAssem.Text, TbDesc.Text, CbRev.Text };
        }
        
        private bool[] getSaveFilter()
        {
            return new bool[]
            {
                ChBSaveProject.IsChecked.Value,
                ChBSaveAssem.IsChecked.Value,
                ChBSaveDesc.IsChecked.Value,
                ChBSaveRev.IsChecked.Value
            };
        }
    }
}
