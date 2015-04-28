using CommandLine;

namespace Edison.ConsoleApp
{
    internal class Options
    {
        [Option('z', "zone")]
        public string Zone { get; set; }

        [Option("white")]
        public bool White { get; set; }

        [Option('b', "brightness")]
        public string Brightness { get; set; }

        [Option('c', "color")]
        public string Color { get; set; }
    }
}
