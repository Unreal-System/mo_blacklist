using System;
using System.Drawing;
using System.Windows.Forms;

namespace BlackList
{
    static class Program
    {

        public static IrcClient Client = null;
        public static string Name = "ctOS_Calibration";
        public static string ID = $"7f7f{new Random().Next(10, 99)}7f7";

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmBL());
        }
    }

    public struct Server
    {
        public Server(string host, string name, int[] ports)
        {
            Host = host;
            Name = name;
            Ports = ports;
        }

        public string Host;
        public string Name;
        public int[] Ports;
    }

    public struct ColorX
    {
        public ColorX(string name, Brush x, int index)
        {
            Name = name;
            Color = x;
            Index = index;
        }
        public string Name;
        public Brush Color;
        public int Index;
    }
}