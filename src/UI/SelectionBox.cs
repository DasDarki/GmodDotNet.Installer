using System;

namespace GmodDotNet.Installer.UI
{
    internal class SelectionBox : Control<int>
    {
        private readonly string[] _items;
        private int _currentIndex;
        
        public SelectionBox(string text, params string[] items) : base(text)
        {
            _items = items;
        }

        protected override void OnDraw()
        {
            for (int i = 0; i < _items.Length; i++)
            {
                Console.Write("\t");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write((_currentIndex == i ? ">" : " ") + " ");
                Console.ResetColor();
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(Result == i ? "x" : " ");
                Console.ResetColor();
                Console.Write("] ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(_items[i]);
            }
            
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\nPlease press twice to confirm selection!");
        }

        protected override void OnKeyInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.DownArrow:
                    _currentIndex++;
                    if (_currentIndex >= _items.Length)
                        _currentIndex = 0;
                    break;
                case ConsoleKey.UpArrow:
                    _currentIndex--;
                    if (_currentIndex < 0)
                        _currentIndex = _items.Length - 1;
                    break;
                case ConsoleKey.Enter:
                    if (Result == _currentIndex)
                    {
                        SetFinished();
                    }
                    else
                    {
                        Result = _currentIndex;
                    }
                    break;
            }
        }
    }
}