using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hash17.Utils
{
    public static class Interpreter
    {
        public static bool GetProgram(string input, out string program, out string parameters)
        {
            var parts = input.Split(' ');
            program = parts[0];
            parameters = input.Substring(program.Length).Trim();
            return true;
        }
    }
}
