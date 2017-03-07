using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Socket_Server
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        private static string[] args;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                args = Environment.GetCommandLineArgs();
                Application.Run(new Main(args));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                Application.Exit();
            }
        }
    }
}
