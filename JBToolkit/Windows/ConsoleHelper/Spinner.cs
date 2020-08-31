using System;
using System.Threading;

namespace JBToolkit.Windows.ConsoleHelper
{
    /// <summary>
    /// Console wait or progress spinner and methods for manipulating the spinner
    /// </summary>
    public class ConsoleSpinner
    {
        private int _currentAnimationFrame;

        public ConsoleSpinner()
        {
            SpinnerAnimationFrames = new[]
                                     {
                                         '|',
                                         '/',
                                         '-',
                                         '\\'
                                     };
        }

        private char[] SpinnerAnimationFrames { get; set; }

        public void UpdateProgress()
        {
            // Store the current position of the cursor
            var originalX = Console.CursorLeft;
            var originalY = Console.CursorTop;
            Console.CursorVisible = false;

            // Write the next frame (character) in the spinner animation
            Console.Write(SpinnerAnimationFrames[_currentAnimationFrame]);

            // Keep looping around all the animation frames
            _currentAnimationFrame++;
            if (_currentAnimationFrame == SpinnerAnimationFrames.Length)
            {
                _currentAnimationFrame = 0;
            }

            // Restore cursor to original position
            Console.SetCursorPosition(originalX, originalY);
        }

        private static ConsoleSpinner Spinner { get; set; } = new ConsoleSpinner();
        private static Thread SpinnerThread { get; set; } = new Thread(RunSpinnerThread);

        /// <summary>
        /// Show / start the spinner
        /// </summary>
        public static void ShowSpinner()
        {
            SpinnerThread.IsBackground = true;
            SpinnerThread.Start();
        }

        /// <summary>
        /// Stop / hide the spinner
        /// </summary>
        public static void StopSpinner()
        {
            try
            {
                if (SpinnerThread.IsAlive)
                {
                    SpinnerThread.Abort();
                }
            }
            catch { }
            finally
            {
                try
                {
                    Console.CursorVisible = false;
                    Console.SetCursorPosition(Console.CursorLeft > 0 ? Console.CursorLeft - 1 : 0, Console.CursorTop);
                    Console.Write("  ");
                }
                catch { }
            }
        }

        private static void RunSpinnerThread()
        {
            Thread.CurrentThread.IsBackground = true;

            while (true)
            {
                Thread.Sleep(70);
                Spinner.UpdateProgress();
            }
        }
    }
}
