using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void writeProperties(string[] propertyStrings)
        {

        }
    }
}
