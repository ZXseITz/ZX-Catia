using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DRAFTINGITF;
using INFITF;
using MECMOD;
using ProductStructureTypeLib;

namespace CATmain
{
    public class CATB
    {
        private INFITF.Application catia;
        private Documents docs;
        private Document aDoc;
        private Selection sel;

        public const string filterPart = "part";
        public const string filterProd = "prod";
        public const string filterDraw = "draw";

        public CATB(string[] filter)
        {
            try
            {
                this.catia = (INFITF.Application)Marshal.GetActiveObject("CATIA.Application");
            }
            catch (Exception)
            {
                throw new COMException("CATIA is not stated");
            }
            this.docs = catia.Documents;
            try
            {
                this.aDoc = catia.ActiveDocument;
                this.sel = aDoc.Selection;
            }
            catch (Exception)
            {
                throw new NullReferenceException("No document is loaded");
            }
            //filter
            if (filter.Length > 0)
            {
                bool flag = false;
                foreach (string doctype in filter)
                {
                    switch (doctype)
                    {
                        case filterPart:
                            if (aDoc is PartDocument)
                            {
                                flag = true;
                            }
                            break;
                        case filterProd:
                            if (aDoc is ProductDocument)
                            {
                                flag = true;
                            }
                            break;
                        case filterDraw:
                            if (aDoc is DrawingDocument)
                            {
                                flag = true;
                            }
                            break;
                    }
                }
                if (!flag)
                {
                    throw new Exception("Actice document doesn't match to filter");
                }
            }
        }

        public INFITF.Application getCatia()
        {
            return this.catia;
        }

        public Documents getDocs()
        {
            return this.docs;
        }

        public Document getActiveDoc()
        {
            return this.aDoc;
        }

        public Selection getSelection()
        {
            return this.sel;
        }
    }
}
