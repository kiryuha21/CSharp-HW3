namespace hw1
{
    internal class Utils
    {
        private static readonly List<string> prefixes = new List<string>() { "~" };
        private static readonly List<string> binaries = new List<string>() { "+", "*", "/", "-" };
        private static readonly List<string> solo_chars = new List<string>() { "+", "-", "/", "*", "(", ")" };
        private enum Priorities { LOW_PRIORITY = 1, HIGH_PRIORITY };


        static public bool is_prefix(string lexem)
        {
            return prefixes.Contains(lexem);
        }

        static public bool is_binary(string lexem)
        {
            return binaries.Contains(lexem);
        }

        static public bool is_solo_char(string lexem)
        {
            return solo_chars.Contains(lexem);
        }

        static public int get_priority(string lexem)
        {
            if (lexem == "+" || lexem == "-")
            {
                return (int)Priorities.LOW_PRIORITY;
            }

            if (lexem == "*" || lexem == "/")
            {
                return (int)Priorities.HIGH_PRIORITY;
            }

            return 0;
        }

        static public bool is_num(string lexem)
        {
            return double.TryParse(lexem, out double _);
        }

    }
}
