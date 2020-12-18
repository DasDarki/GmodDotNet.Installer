using System;
using System.Text;
using System.Threading;

namespace GmodDotNet.Installer.UI
{
    internal class ProgressBar : IDisposable, IProgress<double>
    {
        private const int BlockCount = 10;
        private const string Animation = @"|/-\";
        private readonly TimeSpan _animationInterval = TimeSpan.FromSeconds(1.0 / 8);

        private readonly Timer _timer;
        private int _animationIndex;

        private double _currentProgress;
        private string _currentText = string.Empty;
        private bool _disposed;

        public ProgressBar()
        {
            _timer = new Timer(TimerHandler);
            if (!Console.IsOutputRedirected) ResetTimer();
        }

        public void Stop()
        {
            lock (_timer)
            {
                _disposed = true;
            }
        }
        
        public void Dispose()
        {
            lock (_timer)
            {
                _disposed = true;
                UpdateText(string.Empty);
            }
        }

        public void Report(double value)
        {
            value = Math.Max(0, Math.Min(1, value));
            Interlocked.Exchange(ref _currentProgress, value);
        }

        private void TimerHandler(object state)
        {
            lock (_timer)
            {
                if (_disposed) return;
                var progressBlockCount = (int) (_currentProgress * BlockCount);
                var percent = (int) (_currentProgress * 100);
                var text =
                    $"[{new string('#', progressBlockCount)}{new string('-', BlockCount - progressBlockCount)}] {percent,3}% " +
                    $"{Animation[_animationIndex++ % Animation.Length]}";
                UpdateText(text);

                ResetTimer();
            }
        }

        private void UpdateText(string text)
        {
            var commonPrefixLength = 0;
            var commonLength = Math.Min(_currentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == _currentText[commonPrefixLength])
                commonPrefixLength++;

            var outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', _currentText.Length - commonPrefixLength);
            outputBuilder.Append(text.Substring(commonPrefixLength));
            var overlapCount = _currentText.Length - text.Length;
            if (overlapCount > 0)
            {
                outputBuilder.Append(' ', overlapCount);
                outputBuilder.Append('\b', overlapCount);
            }

            Console.Write(outputBuilder);
            _currentText = text;
        }

        private void ResetTimer()
        {
            _timer.Change(_animationInterval, TimeSpan.FromMilliseconds(-1));
        }
    }
}