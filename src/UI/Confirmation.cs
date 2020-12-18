using System;

namespace GmodDotNet.Installer.UI
{
    internal class Confirmation : Control<bool>
    {
        private readonly string _os;
        private readonly int _type;
        private readonly string _path;
        private readonly string _tag;
        private bool _isYesSelected;
        
        public Confirmation(string os, int type, string path, string tag) : base("Please confirm the installation:")
        {
            _os = os;
            _type = type;
            _path = path;
            _tag = tag;
        }

        protected override void OnDraw()
        {
            Console.Write("\tDetected OS: ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(_os);
            Console.ResetColor();
            Console.Write("\tSelected Release-Tag: ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(_tag);
            Console.ResetColor();
            Console.Write("\tSelected Type: ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(Program.Types[_type]);
            Console.ResetColor();
            Console.Write("\tSelected Path: ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(_path + "\n");
            Console.ResetColor();

            if (_isYesSelected)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" NO ");
                Console.ResetColor();
                Console.Write("    ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("<");
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" YES ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(">");
                
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("<");
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" NO ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(">");
                Console.ResetColor();
                Console.Write("    ");
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" YES ");
            }
            Console.ResetColor();
        }

        protected override void OnKeyInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.Enter:
                    Result = _isYesSelected;
                    SetFinished();
                    break;
                case ConsoleKey.LeftArrow:
                case ConsoleKey.RightArrow:
                    _isYesSelected = !_isYesSelected;
                    break;
            }
        }
    }
}