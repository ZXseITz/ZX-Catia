using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProductStructureTypeLib;

namespace EditProperties
{
    class PropertiesProduct
    {
        public Product product { get; set; }

        public PropertiesProduct(Product product)
        {
            this.product = product;
        }
        public override string ToString()
        {
            return product.get_PartNumber() + "; " + product.get_DescriptionRef();
        }

        /// <summary>
        /// single mode: Writes the specified propeties into the product attribute
        /// multi mode: Writes the specified properties, without Description into the product attrubute
        /// multi mode: specified part number and product attribute part number with legal format are merged
        /// multi mode: specified part number, Project & Assembly filter and product attribute part number, which ends with [#0000, #9999] are merged too
        /// </summary>
        /// <param specified properties="propertyStrings"></param>
        /// <param name="saveFilter"></param>
        /// <param name="singleMode"></param>
        public void writeProperties(string[] propertyStrings, bool[] saveFilter, bool singleMode)
        {
            if (singleMode)
            {
                exportProperties(propertyStrings, saveFilter);
            }
            else
            {
                Regex implizitChangeNumber = new Regex(".*" + Regex.Escape("#") + "\\d{4}");
                string currentPartNum = product.get_PartNumber();
                if (MainWindow.CheckPartNum(currentPartNum) && MainWindow.CheckPartNum(propertyStrings[0]))
                {
                    //overwtrite Project number and/or Assembly number if possible and take current item number
                    string[] currentNums = currentPartNum.Split('.');
                    string[] nums = propertyStrings[0].Split('.');
                    if (saveFilter[0])
                    {
                        currentNums[0] = nums[0];
                    }
                    if (saveFilter[1])
                    {
                        currentNums[1] = nums[1];
                    }
                    propertyStrings[0] = String.Join(".", currentNums);
                }
                else if (saveFilter[0] && saveFilter[1] &&
                         implizitChangeNumber.IsMatch(currentPartNum) && MainWindow.CheckPartNum(propertyStrings[0]))
                {
                    //new Project number, new Assembly number and take current temp #number
                    string[] nums = propertyStrings[0].Split('.');
                    string[] currentNums = propertyStrings[0].Split('#');
                    nums[2] = currentNums[currentNums.Length - 1];
                    propertyStrings[0] = String.Join(".", nums);
                }
                else
                {
                    //take temp number
                    propertyStrings[0] = currentPartNum;
                }
                saveFilter[2] = false;
                exportProperties(propertyStrings, saveFilter);
            }
        }

        /// <summary>
        /// Writes the specified properties into the product attribute
        /// </summary>
        /// <param specified properties="propertyStrings"></param>
        /// <param name="saveFilter"></param>
        private void exportProperties(string[] propertyStrings, bool[] saveFilter)
        {
            //part number
            product.set_PartNumber(propertyStrings[0]);
            //Project
            if (saveFilter[0])
            {
                product.set_Definition(propertyStrings[1]);
            }
            //Assembly
            if (saveFilter[1])
            {
                product.set_Nomenclature(propertyStrings[2]);
            }
            //Description
            if (saveFilter[2])
            {
                product.set_DescriptionRef(propertyStrings[3]);
            }
            //Revision
            if (saveFilter[3])
            {
                product.set_Revision(propertyStrings[4]);
            }
        }
    }
}
