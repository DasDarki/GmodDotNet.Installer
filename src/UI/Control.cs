using System;

namespace GmodDotNet.Installer.UI
{
    internal abstract class Control<T>
    {
        private readonly string _text;
        private bool _isFinished;
        protected T Result;

        protected Control(string text, T @default = default)
        {
            _text = text;
            Result = @default;
        }
            
        protected void SetFinished()
        {
            _isFinished = true;
        }

        protected abstract void OnDraw();

        protected virtual void OnKeyInput(ConsoleKey key) {}
        
        public T Await()
        {
            while (!_isFinished)
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("\n" + _text + "\n");
                OnDraw();
                Console.ResetColor();
                ConsoleKey key = Console.ReadKey().Key;
                OnKeyInput(key);
            }
            
            return Result;
        }
    }
}