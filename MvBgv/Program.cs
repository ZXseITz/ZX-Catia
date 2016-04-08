using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CATmain;
using DRAFTINGITF;
using System.Windows.Forms;

namespace MvBgv
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string[] filter = { CATB.filterDraw };
                CATB catia = new CATB(filter);

                DrawingSheet aSheet = ((DrawingDocument)catia.getActiveDoc()).Sheets.ActiveSheet;
                // change view
                if (aSheet.Views.ActiveView != aSheet.Views.Item(2))
                {
                    //background
                    aSheet.Views.Item(2).Activate();
                }
                else
                {
                    //main view
                    aSheet.Views.Item(1).Activate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Main view - Background view", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }
    }
}
