namespace hw1
{
    public class ReversePolishBuilder
    {
        private List<string> polished_ = new List<string>();

        void handle_binary(ref Stack<string> stack, string lexem)
        {
            while (!(stack.Count == 0) &&
                (Utils.is_prefix(stack.Peek()) || Utils.get_priority(stack.Peek()) >= Utils.get_priority(lexem)))
            {
                polished_.Add(stack.Pop());
            }
            stack.Push(lexem);
        }

        public List<string> parse_to_polish(ref List<string> lexems)
        {
            polished_.Clear();

            Stack<string> operations_stack = new Stack<string>();
            foreach (var lexem in lexems)
            {
                if (Utils.is_prefix(lexem) || lexem == "(")
                {
                    operations_stack.Push(lexem);
                }
                else if (Utils.is_num(lexem))
                {
                    polished_.Add(lexem);
                }
                else if (lexem == ")")
                {
                    while (!(operations_stack.Count == 0) && operations_stack.Peek() != "(")
                    {
                        polished_.Add(operations_stack.Pop());
                    }

                    if (operations_stack.Count == 0)
                    {
                        throw new Exception("Error while parsing to polish notation");
                    }

                    operations_stack.Pop();
                }
                else // is binary
                {
                    handle_binary(ref operations_stack, lexem);
                }
            }

            while (!(operations_stack.Count == 0))
            {
                polished_.Add(operations_stack.Pop());
            }

            return polished_;
        }
    }
}
