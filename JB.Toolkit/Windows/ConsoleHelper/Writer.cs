using Colorful;
using JBToolkit.AssemblyHelper;
using JBToolkit.Images;
using JBToolkit.Windows.ConsoleHelper;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace System
{
    /// <summary>
    /// Console extension methods, particularly writing out text and lines easily in a particular colour and setting background text.
    /// Also sses the nuget package ColorFul.Console so you can utilise System.Drawing.Color rather just the standard few ConsoleColor list.
    /// You can also use hex strings and even convert text to ASCII art.
    /// </summary>
    public class ConsoleEx
    {
        #region P/Invoke declarations 
        private struct RECT { public int left, top, right, bottom; }
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT rc);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int w, int h, bool repaint);

        internal const int MF_BYCOMMAND = 0x00000000;
        internal const int SC_CLOSE = 0xF060;

        #endregion

        /// <summary>
        /// Write line in a colour as of a ConsoleColor (limited range of colours)
        /// </summary>
        public static void WriteColouredLine(ConsoleColor consoleColor, string text)
        {
            WriteColouredLine(ColourHelper.ConsoleColorToColor(consoleColor), text);
        }

        /// <summary>
        /// Write line in a colour as of a hex colour (with or without the #). Full range of colours.
        /// </summary>
        public static void WriteColouredLine(string colorHex, string text)
        {
            WriteColouredLine(ColourHelper.HexStringToColor(colorHex), text);
        }

        /// <summary>
        /// Write line in a colour as of a System.Drawing.Color (full range of colours)
        /// </summary>
        public static void WriteColouredLine(Color color, string text)
        {
            var currentColour = Colorful.Console.BackgroundColor;

            Colorful.Console.WriteLine(text, color);
            Console.ResetColor();
            Colorful.Console.BackgroundColor = currentColour;
        }

        /// <summary>
        /// Write text in a colour as of a ConsoleColor (limited range of colours)
        /// </summary>
        public static void WriteColoured(ConsoleColor consoleColor, string text)
        {
            WriteColoured(ColourHelper.ConsoleColorToColor(consoleColor), text);
        }

        /// <summary>
        /// Write text in a colour as of a hex colour (with or without the #). Full range of colours.
        /// </summary>
        public static void WriteColoured(string colorHex, string text)
        {
            WriteColoured(ColourHelper.HexStringToColor(colorHex), text);
        }

        /// <summary>
        /// Write text in a colour as of a System.Drawing.Color (full range of colours)
        /// </summary>
        public static void WriteColoured(Color color, string text)
        {
            var currentColour = Colorful.Console.BackgroundColor;

            Colorful.Console.Write(text, color);
            Console.ResetColor();
            Colorful.Console.BackgroundColor = currentColour;
        }

        /// <summary>
        /// Write text in a colour as of a ConsoleColor (limited range of colours)
        /// </summary>
        public static void WriteColouredText(ConsoleColor consoleColor, string text)
        {
            WriteColouredText(ColourHelper.ConsoleColorToColor(consoleColor), text);
        }

        /// <summary>
        /// Write text in a colour as of a hex colour (with or without the #). Full range of colours.
        /// </summary>
        public static void WriteColouredText(string colorHex, string text)
        {
            WriteColouredText(ColourHelper.HexStringToColor(colorHex), text);
        }

        /// <summary>
        /// Write text in a colour as of a System.Drawing.Color (full range of colours)
        /// </summary>
        public static void WriteColouredText(Color color, string text)
        {
            var currentColour = Colorful.Console.BackgroundColor;

            Colorful.Console.Write(text, color);
            Console.ResetColor();
            Colorful.Console.BackgroundColor = currentColour;
        }

        /// <summary>
        /// Set the console background colour. Typically you have to clear the console to replace the whole window however if you already have text
        /// on the console this will clear as well. You're best off calling SetBackgroundColour(color, true) at the start of your application.
        /// Colour as of a ConsoleColor (limited range of colours)
        /// </summary>
        /// <param name="clearConsole"></param>
        public static void SetBackgroundColour(ConsoleColor consoleColor, bool clearConsole = false)
        {
            SetBackgroundColour(ColourHelper.ConsoleColorToColor(consoleColor), clearConsole);
        }

        /// <summary>
        /// Set the console background colour. Typically you have to clear the console to replace the whole window however if you already have text
        /// on the console this will clear as well. You're best off calling SetBackgroundColour(color, true) at the start of your application.
        /// Colour as of a hex string (with or without the #). Full range of colours.
        /// </summary>
        /// <param name="clearConsole"></param>
        public static void SetBackgroundColour(string hexColor, bool clearConsole = false)
        {
            SetBackgroundColour(ColourHelper.HexStringToColor(hexColor), clearConsole);
        }

        /// <summary>
        /// Set the console background colour. Typically you have to clear the console to replace the whole window however if you already have text
        /// on the console this will clear as well. You're best off calling SetBackgroundColour(color, true) at the start of your application.
        /// Colour as of a System.Drawing.Color (full range of colours)
        /// </summary>
        /// <param name="clearConsole"></param>
        public static void SetBackgroundColour(Color color, bool clearConsole = false)
        {
            Colorful.Console.BackgroundColor = color;

            if (clearConsole)
            {
                Console.Clear();
            }
        }

        /// <summary>
        /// Set the background colour to a nice dark grey, centre the console and disable the cursor
        /// </summary>
        public static void SetupBetterConsole()
        {
            Colorful.Console.BackgroundColor = ColourHelper.HexStringToColor("191919");
            Console.Clear();
            CentreConsole();
            DisableCursor();
        }

        /// <summary>
        /// Produce ASCII art text. Use a range of FIGlet fonts easily set via enum list.
        /// </summary>
        /// <param name="font">FIGlet Font enum</param>
        public static void WriteAsciiArt(FigFontEnum font, ConsoleColor consoleColor, string text)
        {
            WriteAsciiArt(font, ColourHelper.ConsoleColorToColor(consoleColor), text);
        }

        /// <summary>
        /// Produce ASCII art text. Use a range of FIGlet fonts easily set via enum list.
        /// </summary>
        /// <param name="font">FIGlet Font enum</param>
        public static void WriteAsciiArt(FigFontEnum font, string hexColor, string text)
        {
            WriteAsciiArt(font, ColourHelper.HexStringToColor(hexColor), text);
        }

        /// <summary>
        /// Produce ASCII art text. Use a range of FIGlet fonts easily set via enum list.
        /// </summary>
        /// <param name="font">FIGlet Font enum</param>
        public static void WriteAsciiArt(FigFontEnum font, Color color, string text)
        {
            string fontPath = EmbeddedResourceHelper.GetEmbeddedResourcePath(
                font.ToString() + ".flf",
                "Dependencies_Embedded.FIGLetFonts",
                EmbeddedResourceHelper.TargetAssemblyType.Executing, true);

            var figFont = FigletFont.Load(fontPath);
            var figlet = new Figlet(figFont);

            Colorful.Console.WriteLine(figlet.ToAscii(text), color);
        }

        /// <summary>
        /// Produce ASCII art text (default font).
        /// </summary>
        public static void WriteAsciiArt(ConsoleColor consoleColor, string text)
        {
            WriteAsciiArt(ColourHelper.ConsoleColorToColor(consoleColor), text);
        }

        /// <summary>
        /// Produce ASCII art text (default font).
        /// </summary>
        public static void WriteAsciiArt(string hexColor, string text)
        {
            WriteAsciiArt(ColourHelper.HexStringToColor(hexColor), text);
        }

        /// <summary>
        /// Produce ASCII art text (default font).
        /// </summary>
        public static void WriteAsciiArt(Color color, string text)
        {
            Colorful.Console.WriteAscii(text, color);
        }

        public static void DisableCursor()
        {
            Console.CursorVisible = false;
        }

        public static void EnableCursor()
        {
            Console.CursorVisible = true;
        }

        /// <summary>
        /// Shows an animated spinner
        /// </summary>
        public static void ShowSpinner()
        {
            ConsoleSpinner.ShowSpinner();
        }

        /// <summary>
        /// Stops the animated spinner
        /// </summary>
        public static void StopSpinner()
        {
            ConsoleSpinner.StopSpinner();
        }

        /// <summary>
        /// Centres the console in the the middle of the screen. You'll have to add a reference to System.Windows.Forms.
        /// </summary>
        public static void CentreConsole()
        {
            IntPtr hWin = GetConsoleWindow();
            GetWindowRect(hWin, out RECT rc);
            Screen scr = Screen.FromPoint(new Point(rc.left, rc.top));

            int x = scr.WorkingArea.Left + (scr.WorkingArea.Width - (rc.right - rc.left)) / 2;
            int y = scr.WorkingArea.Top + (scr.WorkingArea.Height - (rc.bottom - rc.top)) / 2;
            MoveWindow(hWin, x, y, rc.right - rc.left, rc.bottom - rc.top, false);
        }

        /// <summary>
        /// List of available FIGLet fonts embedded into assembly
        /// </summary>
        public enum FigFontEnum
        {
            ThreeDAscii,
            ThreeDDiagonal,
            Threepoint,
            ThreeX5,
            Ticks,
            Ticksslant,
            Tiles,
            Tinkertoy,
            Tombstone,
            Train,
            Trek,
            Tsalagi,
            Tubular,
            Twisted,
            Twopoint,
            Univers,
            Usaflag,
            Varsity,
            Wavy,
            Weird,
            Wetletter,
            Whimsy,
            Wow,
            Acrobatic,
            Alligator,
            Alligator2,
            Alligator3,
            Alpha,
            Alphabet,
            Amc3Line,
            Amc3Liv1,
            Amcaaa01,
            Amcneko,
            Amcrazo2,
            Amcrazor,
            Amcrazor2,
            Amcslash,
            Amcslder,
            Amcslider,
            Amcthin,
            Amctubes,
            Amcun,
            Amcuntitled,
            Ansiregular,
            Ansishadow,
            Arrows,
            AsciiNewRoman,
            Avatar,
            B1ff,
            Banner,
            Banner3,
            Banner3d,
            Banner4,
            Barbwire,
            Basic,
            Bear,
            Bell,
            Benjamin,
            Big,
            Bigchief,
            Bigfig,
            Bigmoneyne,
            Bigmoneynw,
            Bigmoneyse,
            Bigmoneysw,
            Binary,
            Block,
            Blocks,
            Bloody,
            Bolger,
            Braced,
            Bright,
            Broadway,
            Broadway_Kb,
            Broadwaykb,
            Bubble,
            Bulbhead,
            Calgphy2,
            Caligraphy,
            Caligraphy2,
            Calvins,
            Cards,
            Catwalk,
            Chiseled,
            Chunky,
            Coinstak,
            Cola,
            Colossal,
            Computer,
            Contessa,
            Contrast,
            Cosmic,
            Cosmike,
            Crawford,
            Crawford2,
            Crazy,
            Cricket,
            Cursive,
            Cyberlarge,
            Cybermedium,
            Cybersmall,
            Cygnet,
            Danc4,
            Dancingfont,
            Decimal,
            Defleppard,
            Deltacorpspriest1,
            Diamond,
            Dietcola,
            Digital,
            Doh,
            Doom,
            Dosrebel,
            Dotmatrix,
            Double,
            Doubleshorts,
            Drpepper,
            Dwhistled,
            Eftichess,
            Eftifont,
            Eftiitalic,
            Eftipiti,
            Eftirobot,
            Eftitalic,
            Eftiwall,
            Eftiwater,
            Electronic,
            Elite,
            Epic,
            Fender,
            Filter,
            Fire_Fontk,
            Fire_Fonts,
            Firefontk,
            Firefonts,
            FiveLineOlique,
            Flipped,
            Flowerpower,
            Fourtops,
            Fraktur,
            Funface,
            Funfaces,
            Fuzzy,
            Georgi16,
            Georgia11,
            Ghost,
            Ghoulish,
            Glenyn,
            Goofy,
            Gothic,
            Graceful,
            Gradient,
            Graffiti,
            Greek,
            Heart_Left,
            Heart_Right,
            Heartleft,
            Heartright,
            Henry3d,
            Hex,
            Hieroglyphs,
            Hollywood,
            Horizontalleft,
            Horizontalright,
            Icl1900,
            Impossible,
            Invita,
            Isometric1,
            Isometric2,
            Isometric3,
            Isometric4,
            Italic,
            Ivrit,
            Jacky,
            Jazmine,
            Jerusalem,
            Jsblockletters,
            Jsbracketletters,
            Jscapitalcurves,
            Jscursive,
            Jsstickletters,
            Katakana,
            Kban,
            Keyboard,
            Knob,
            Konto,
            Kontoslant,
            Larry3d,
            Larry3d2,
            Lcd,
            Lean,
            Letters,
            Lildevil,
            Lineblocks,
            Linux,
            Lockergnome,
            Madrid,
            Marquee,
            Max4,
            Maxfour,
            Maxiwi,
            Merlin1,
            Merlin2,
            Mike,
            Mini,
            Miniwi,
            Mirror,
            Mnemonic,
            Modular,
            Morse,
            Morse2,
            Moscow,
            Mshebrew210,
            Muzzle,
            Nancyj,
            Nancyjfancy,
            Nancyjimproved,
            Nancyjunderlined,
            Nipples,
            Nscript,
            Ntgreek,
            Nvscript,
            O8,
            Octal,
            Ogre,
            Oldbanner,
            OneRow,
            Os2,
            PatorjkHex,
            PatorjkCheese,
            Pawp,
            Peaks,
            Peaksslant,
            Pebbles,
            Pepper,
            Poison,
            Puffy,
            Puzzle,
            Pyramid,
            Rammstein,
            Rectangles,
            RedPhoenix,
            Relief,
            Relief2,
            Rev,
            Reverse,
            Roman,
            Rot13,
            Rotated,
            Rounded,
            Rowancap,
            Rozzo,
            Runic,
            Runyc,
            Santaclara,
            Sblood,
            Script,
            Serifcap,
            Shadow,
            Shimrod,
            Short,
            Slant,
            Slantrelief,
            Slide,
            Slscript,
            Small,
            Smallcaps,
            Smallisometric1,
            Smallkeyboard,
            Smallpoison,
            Smallscript,
            Smallshadow,
            Smallslant,
            Smalltengwar,
            Smisome1,
            Smkeyboard,
            Smpoison,
            Smscript,
            Smshadow,
            Smslant,
            Smtengwar,
            Soft,
            Speed,
            Spliff,
            Srelief,
            Stacey,
            Stampate,
            Stampatello,
            Standard,
            Starstrips,
            Starwars,
            Stellar,
            Stforek,
            Stickletters,
            Stop,
            Straight,
            Strongerthanall,
            Subzero,
            Swampland,
            Swan,
            Sweet,
            Tanja,
            Tengwar,
            Term,
            Test1,
            Theedge,
            Thick,
            Thin,
            This,
            Thorned,
            ThreeD
        }
    }
}
