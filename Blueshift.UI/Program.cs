using System;

namespace Blueshift.UI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var windowParams = new EmulatorWindowParameters()
            {
                Title = "Blueshift Emulator",
                Width = 800,
                Height = 400,
                MajorGLVersion = 4,
                MinorGLVersion = 6,
            };

            using (var window = new EmulatorWindow(windowParams))
            {
                window.Run();
            }
        }
    }
}
