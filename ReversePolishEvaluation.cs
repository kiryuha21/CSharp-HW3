namespace hw1
{
    public class ReversePolishEvaluation
    {
        double apply_binary(ref Stack<double> stack, string lexem)
        {
            double result;

            double first = stack.Pop();
            double second = stack.Pop();

            if (lexem == "-")
            {
                result = second - first;
            }
            else if (lexem == "+")
            {
                result = second + first;
            }
            else if (lexem == "/")
            {
                result = second / first;
            }
            else // lexem == *
            {
                result = second * first;
            }

            return result;
        }

        double apply_prefix(ref Stack<double> stack, string lexem)
        {
            double result;

            double first = stack.Pop();

            if (lexem == "~")
            {
                result = -first;
            }
            else
            {
                throw new Exception("This coundn't happen");
            }

            return result;
        }

        public double apply_polish(ref List<string> polished)
        {
            double result = 0;
            Stack<double> stack = new Stack<double>();

            foreach (var lexem in polished)
            {

                if (Utils.is_num(lexem))
                {

                    result = double.Parse(lexem);

                }
                else if (Utils.is_binary(lexem))
                {
                    if (stack.Count < 2)
                    {
                        throw new Exception("Error while applying polish notation(not enough operands)");
                    }

                    result = apply_binary(ref stack, lexem);
                }
                else if (Utils.is_prefix(lexem))
                {
                    if (stack.Count == 0)
                    {
                        throw new Exception("Error while applying polish notation(not enough arguments)");
                    }

                    result = apply_prefix(ref stack, lexem);
                }
                else
                {
                    throw new Exception("Error while applying polish notation(unknown operation)");
                }
                stack.Push(result);
            }

            if (stack.Count == 0)
            {
                throw new Exception("Error while applying polish notation(no result)");
            }

            result = stack.Pop();
            if (!(stack.Count == 0))
            {
                throw new Exception("Error while applying polish notation(extra values)");
            }

            return result;
        }

    }
}
