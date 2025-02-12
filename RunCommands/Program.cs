using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace RunCommands
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Enter command to execute:");
            //string command = "Xcopy " + "C:\\BrbADev\\Bin\\License\\*.* " + "C:\\BrbADEVBACKUP\\Bin\\License\\*.*";//Console.ReadLine().Trim(); //
            //ExecuteCommands cmd = new ExecuteCommands();
            //cmd.ExecuteCommand(command);
            ////   net time \\maa202089 /set /y
            //Console.WriteLine("Done... Press enter to exit");
            //Console.Read();
            Application.SetCompatibleTexcharteringDefault(false);
            Application.Run(new Form1());
        }
    }
}
