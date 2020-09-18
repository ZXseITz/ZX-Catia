using System;
using CATmain;
using DRAFTINGITF;
using System.Windows.Forms;

namespace FgBgv
{
    class Program
    {
        static void Main(string[] args)
        {
            const string caption = "Foreground - Background view";
            const int foreground = 1;
            const int background = 2;
            try
            {
                var catia = Bootstrap.Init();
                var activeDoc = catia.ActiveDocument;
                if (!Bootstrap.CheckDocument(activeDoc, DocType.Drawing))
                {
                    MessageBox.Show("Expected drawing document", caption,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                var activeSheet = ((DrawingDocument) activeDoc).Sheets.ActiveSheet; // Cannot be null
                var views = activeSheet.Views;
                views.Item(views.ActiveView == views.Item(foreground) ? background : foreground).Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}