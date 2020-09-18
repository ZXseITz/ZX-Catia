using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using INFITF;
using MECMOD;

namespace CATmain
{

    public static class Bootstrap
    {
        private const string programID = "CATIA.Application";

        public static Application Init()
        {
            try
            {
                return (Application) Marshal.GetActiveObject(programID);
            }
            catch
            {
                throw new COMException("CATIA is not running.");
            }
        }

        public static bool CheckDocument(Document document, params DocType[] docTypes)
        {
            return docTypes.Any(docType => docType.Check(document));
        }
    }
}
