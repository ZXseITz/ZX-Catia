using System;
using DRAFTINGITF;
using INFITF;
using MECMOD;
using ProductStructureTypeLib;

namespace CATmain
{
    public class DocType
    {
        public static DocType Part = new DocType(typeof(PartDocument));
        public static DocType Product = new DocType(typeof(ProductDocument));
        public static DocType Drawing = new DocType(typeof(DrawingDocument));

        public readonly Type Type;

        private DocType(Type type)
        {
            Type = type;
        }

        public bool Check(Document document)
        {
            return document != null && Type.IsInstanceOfType(document);
        }
    }
}