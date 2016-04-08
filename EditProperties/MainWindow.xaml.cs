using System;
using System.Collections.Generic;
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
        private static string filePath = Directory.GetCurrentDirectory() + "\\" + title + "Settings.conf";
        private PropertiesProductCollection list;

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
                //load settings
                if (File.Exists(filePath))
                {
                    Regex regex = new Regex(".*=.*");
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
                    loadDefaultSettings();
                }
                //Copy settings

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
                    addToList(firstProduct);
                    importProperties(firstProduct, new bool[] { true, true, true, true });
                }
                else
                {
                    bool flag = true;
                    for (int i = 1; i <= sel.Count2; i++)
                    {
                        if (sel.Item2(i).Value is Part || sel.Item2(i).Value is Product)
                        {
                            firstProduct = convertToProduct((AnyObject)sel.Item2(i).Value);
                            if (flag)
                            {
                                importProperties(firstProduct, new bool[] { true, true, true, true });
                                flag = false;
                            }
                            addToList(firstProduct);
                        }
                    }
                }
            }
            catch (InvalidCastException ice)
            {
                MessageBox.Show(ice.Message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
                loadDefaultSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }

        private void loadDefaultSettings()
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
        /// Casts a part into product or it's already a product
        /// </summary>
        /// <param partOrProduct="obj"></param>
        /// <returns>product</returns>
        private Product convertToProduct(AnyObject obj)
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

        private void addToList(Product product)
        {
            if (list.All(p => p.product != product))
            {
                list.Add(new PropertiesProduct(product));
            }
        }

        private string[] readProperties(Product prod)
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

        private void importProperties(Product prod, bool[] filterBools)
        {
            string[] properties = readProperties(prod);
            //part number
            if (!checkPartNum(properties[0]))
            {
                TbPartNum.Text = properties[0];
            }
            else
            {
                string[] pNums = properties[0].Split('.');
                string[] newPNums = new string[3];
                for (int i = 0; i < filterBools.Length - 1; i++)
                {
                    if (filterBools[i])
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
            if (filterBools[0])
            {
                TbProject.Text = properties[1];
            }
            //assembly
            if (filterBools[1])
            {
                TbAssem.Text = properties[2];
            }
            //description
            if (filterBools[2])
            {
                TbDesc.Text = properties[3];
            }
            //revision
            if (filterBools[3])
            {
                CbRev.Text = properties[4];
            }
        }

        /// <summary>
        /// Checks if the part number matches the public format
        /// </summary>
        /// <param partnumber="partNum"></param>
        /// <returns> match </returns>
        private bool checkPartNum(String partNum)
        {
            Regex regex = new Regex("\\d{3}" + Regex.Escape(".") + "\\d{3}" + Regex.Escape(".") + "\\d{4}");
            return regex.IsMatch(partNum);
        }

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
                    importProperties(convertToProduct((AnyObject)sel.Item2(1).Value), new bool[]
                    {
                        ChBImportProject.IsChecked.Value,
                        ChBImportAssem.IsChecked.Value,
                        ChBImportDesc.IsChecked.Value,
                        ChBImportRev.IsChecked.Value,
                    });
                }
                sel.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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
                importProperties(prod, new bool[]
                    {
                        ChBImportProject.IsChecked.Value,
                        ChBImportAssem.IsChecked.Value,
                        ChBImportDesc.IsChecked.Value,
                        ChBImportRev.IsChecked.Value,
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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
                    addToList(convertToProduct((AnyObject)sel.Item2(i).Value));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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

        private void BSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
