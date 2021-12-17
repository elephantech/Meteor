using System;
using System.Windows.Forms;

namespace MeteorInkJet.SdCardTestApp {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (!MeteorPath.LocatePrinterInterface()) {
                MessageBox.Show("Failed to find Meteor Printer Interface!!", "SdCardTestApp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.Run(new FormSdCardTestApp());
        }
    }
}
