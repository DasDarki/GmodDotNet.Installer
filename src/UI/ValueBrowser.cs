using System;

namespace GmodDotNet.Installer.UI
{
    internal class ValueBrowser : Control<string>
    {
        private readonly string _valueName;
        private readonly string _valueAction;
        private readonly Func<string, bool> _pathCheck;
        private bool _isBrowseSelected = true;
        private string _nextError;
        
        public ValueBrowser(string valueName, string valueAction, string text, string path, Func<string, bool> pathCheck) : base(text, path ?? "")
        {
            _pathCheck = pathCheck;
            _valueName = valueName;
            _valueAction = valueAction;
        }

        protected override void OnDraw()
        {
            Console.WriteLine($" Selected {_valueName}: ");
            Console.Write(" \n\t");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(Result + "\n");
            Console.ResetColor();
            if (!string.IsNullOrEmpty(_nextError))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(_nextError + "\n");
                _nextError = null;
            }
            
            Console.ResetColor();
            Console.Write(" ");
            if (_isBrowseSelected)
            {
                Console.Write("<");
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" {_valueAction.ToUpper()} ");
                Console.ResetColor();
                Console.Write(">    ");
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" CONFIRM ");
                Console.ResetColor();
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" {_valueAction.ToUpper()} ");
                Console.ResetColor();
                Console.Write("    <");
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" CONFIRM ");
                Console.ResetColor();
                Console.WriteLine(">");
            }
        }

        protected override void OnKeyInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.LeftArrow:
                case ConsoleKey.RightArrow:
                    _isBrowseSelected = !_isBrowseSelected;
                    break;
                case ConsoleKey.Enter:
                    if (_isBrowseSelected)
                    {
                        string path = BrowsePath();
                        if (_pathCheck.Invoke(path))
                        {
                            Result = path;
                        }
                        else
                        {
                            _nextError = $"The entered {_valueName} was invalid!";
                        }
                    }
                    else
                    {
                        if (_pathCheck.Invoke(Result))
                        {
                            SetFinished();
                        }
                        else
                        {
                            _nextError = $"The entered {_valueName} was invalid!";
                        }
                    }
                    break;
            }
        }

        private string BrowsePath()
        {
            Console.CursorVisible = true;
            Console.Clear();
            Console.WriteLine($"Enter {_valueName} here:");
            Console.Write(" > ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            string path = Console.ReadLine();
            Console.ResetColor();
            Console.CursorVisible = false;
            return path;
        }
    }
}