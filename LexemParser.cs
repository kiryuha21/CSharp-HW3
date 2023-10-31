using System.Text.RegularExpressions;

namespace hw1
{
    public class LexemParser
    {
        List<string> lexems_ = new List<string>();

        bool is_unary()
        {
            return lexems_.Count == 0 || lexems_.Last() == "(";
        }

        int handle_lexem(string line, int index)
        {
            if (Utils.is_solo_char(line[index].ToString()))
            {
                if (line[index] == '-' && is_unary())
                {
                    lexems_.Add("~");
                }
                else if (line[index] != '+' || !is_unary())
                {
                    lexems_.Add(line[index].ToString());
                }
                return 1;
            }

            if (char.IsDigit(line[index]) || line[index] == ',')
            {
                string num_string = line.Substring(index);
                Match match = Regex.Match(num_string, @"\d*,?\d*");
                double num = double.Parse(match.Value);
                lexems_.Add(num.ToString());
                return num.ToString().Length;
            }

            throw new Exception("Unknown symbol!");
        }

        public List<string> parse_line(string line)
        {
            lexems_.Clear();
            int line_size = line.Length;
            for (int i = 0; i < line_size;)
            {
                if (line[i] != ' ')
                {
                    i += handle_lexem(line, i);
                }
                else
                {
                    ++i;
                }
            }
            return lexems_;
        }
    }
}
