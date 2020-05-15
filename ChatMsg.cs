using System.Drawing;

namespace BlackList
{
    public class ChatMsg
    {
        public ChatMsg(string msg)
        {
            Color = Brushes.White;
            Name = "";
            ID = "";
            IP = "";
            Msg = msg;
        }

        public ChatMsg(Brush color, string msg)
        {
            Color = color;
            Name = "";
            ID = "";
            IP = "";
            Msg = msg;
        }

        public ChatMsg(string name, string id, string ip, string msg)
        {
            Color = Brushes.White;
            Name = name;
            ID = id;
            IP = ip;
            Msg = msg;
        }

        public ChatMsg(Brush color, string name, string id, string ip, string msg)
        {
            Color = color;
            Name = name;
            ID = id;
            IP = ip;
            Msg = msg;
        }

        public Brush Color;
        public string Name;
        public string ID;
        public string IP;
        public string Msg;

        public override string ToString()
        {
            return Msg;
        }
    }
}