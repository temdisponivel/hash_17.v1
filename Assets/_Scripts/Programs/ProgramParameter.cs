using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using Debug = UnityEngine.Debug;

namespace Hash17.Programs
{
    public class ProgramParameter
    {
        #region Inner classes

        public class Param
        {
            public string Name;
            public string Value;
            public char Prefix;
        }

        #endregion

        public string RawData;
        public List<Param> Params = new List<Param>();
        public static List<char> Prefixes = new List<char>() { '-' };

        public ProgramParameter(string parameters)
        {
            RawData = parameters.Trim();
            InterpreteParameters();
        }

        #region Interprete
        
        private void InterpreteParameters()
        {
            string paramName = string.Empty;
            string paramValue = string.Empty;
            char paramPrefix = '\0';

            List<Param> paramsFound = new List<Param>();

            const int readParameterName = 0;
            const int readParameterValue = 1;
            const int lookForParam = 2;

            int state = readParameterValue;

            for (int i = 0; i < Prefixes.Count; i++)
            {
                if (RawData.StartsWith(Prefixes[i].ToString()))
                {
                    state = lookForParam;
                    break;
                }
            }

            StringBuilder paramNameBuilder = new StringBuilder();
            StringBuilder paramValueBuilder = new StringBuilder();
            bool onQuot = false;
            bool treatSpecialCharAsString = false;
            for (int i = 0; i < RawData.Length; i++)
            {
                var currentChar = RawData[i];
                bool isParamPrefix = Prefixes.Contains(currentChar);

                if (!treatSpecialCharAsString && currentChar == '\\')
                {
                    treatSpecialCharAsString = true;
                    continue;
                }

                if (currentChar == '"')
                {
                    if (!treatSpecialCharAsString)
                    {
                        onQuot = !onQuot;
                        continue;
                    }
                }

                if (onQuot)
                    treatSpecialCharAsString = true;
                
                if (state == lookForParam)
                {
                    if (isParamPrefix && !treatSpecialCharAsString)
                    {
                        paramPrefix = currentChar;
                        state = readParameterName;
                    }
                }
                else if (state == readParameterName)
                {
                    if (!treatSpecialCharAsString && currentChar == ' ')
                    {
                        paramName = paramNameBuilder.ToString().Trim();
                        state = readParameterValue;
                    }
                    else
                    {
                        paramNameBuilder = paramNameBuilder.Append(currentChar);
                    }
                }
                else if (state == readParameterValue)
                {
                    if (!treatSpecialCharAsString && isParamPrefix)
                    {
                        paramValue = paramValueBuilder.ToString().Trim();

                        paramValueBuilder = new StringBuilder();
                        paramNameBuilder = new StringBuilder();

                        paramsFound.Add(new Param()
                        {
                            Name = paramName,
                            Value = paramValue,
                            Prefix = paramPrefix,
                        });

                        state = readParameterName;
                        paramPrefix = currentChar;
                    }
                    else
                    {
                        paramValueBuilder = paramValueBuilder.Append(currentChar);
                    }
                }

                treatSpecialCharAsString = false;
            }

            paramValue = paramValueBuilder.ToString().Trim();
            paramName = paramNameBuilder.ToString().Trim();

            if (!string.IsNullOrEmpty(paramValue) || !string.IsNullOrEmpty(paramName))
            {
                paramsFound.Add(new Param()
                {
                    Name = paramName,
                    Value = paramValue,
                    Prefix = paramPrefix,
                });
            }

            Params = new List<Param>(paramsFound);
        }
        
        public void AddParam(string name, string value, char prefix)
        {
            Params.Add(new Param()
            {
                Name = name,
                Value = value,
            });
        }

        #endregion

        #region Helpers

        public bool HasParamOtherThan(out List<Param> unknownParameters, params string[] parameters)
        {
            unknownParameters = new List<Param>();
            for (int i = 0; i < Params.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < parameters.Length; j++)
                {
                    if (!string.IsNullOrEmpty(parameters[i]) && Params[i].Name == parameters[j])
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    unknownParameters.Add(Params[i]);
            }

            return unknownParameters.Count > 0;
        }

        public bool ContainParam(string paramToSearch)
        {
            return Params.Exists(p => p.Name == paramToSearch);
        }

        public bool TryGetParam(string paramToSearch, out Param param)
        {
            param = Params.Find(p => p.Name == paramToSearch);
            return param != null;
        }

        public Param GetFirstParamWithValue()
        {
            for (int i = 0; i < Params.Count; i++)
            {
                if (!string.IsNullOrEmpty(Params[i].Value))
                    return Params[i];
            }

            return null;
        }

        #endregion
    }
}

