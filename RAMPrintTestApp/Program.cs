using System;
using System.Windows.Forms;

namespace Ttp.Meteor.RAMPrintTestApp {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (!MeteorPath.LocatePrinterInterface()) {
                MessageBox.Show("Failed to find Meteor Printer Interface!!", "RAMPrintTestApp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.Run(new FormRAMPrintTestApp());
        }
    }
}
