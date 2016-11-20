using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hash17.Utils
{
    public static class Interpreter
    {
        public static bool ContainsParameter(string parameters, bool useSlash, string parameter, out string parameterValue)
        {
            bool containParameter = false;

            int state = 0;

            const int lookForSlash = 0;
            const int readParameter = 1;
            const int readParameterValue = 2;

            char toLook = useSlash ? '/' : '-';

            StringBuilder parameterValueBuilder = new StringBuilder();

            for (int i = 0; i < parameters.Length; i++)
            {
                if (state == lookForSlash)
                {
                    if (parameters[i] == toLook)
                        state = readParameter;
                }
                else if (state == readParameter)
                {
                    int j = 0;
                    bool match = true;
                    while (i < parameters.Length && parameters[i] != ' ')
                    {
                        if (j >= parameter.Length || parameter[j] != parameters[i])
                        {
                            state = lookForSlash;
                            match = false;
                            break;
                        }

                        i++;
                        j++;
                    }

                    if (match)
                    {
                        containParameter = true;
                        state = readParameterValue;
                    }
                }
                else if (state == readParameterValue)
                {
                    while (i < parameters.Length && parameters[i] != ' ')
                    {
                        parameterValueBuilder = parameterValueBuilder.Append(parameters[i++]);
                    }
                }
            }

            parameterValue = parameterValueBuilder.ToString();

            return containParameter;
        }

        public static bool GetProgram(string input, out string program, out string parameters)
        {
            var parts = input.Split(' ');
            program = parts[0];
            parameters = input.Substring(program.Length).Trim();
            return true;
        }
    }
}
