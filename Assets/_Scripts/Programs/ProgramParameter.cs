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

        public ProgramParameter(string parameters)
        {
            RawData = parameters.Trim();
            InterpreteParameters();
        }

        #region Interprete

        //public void InterpreteParameters()
        //{
        //    char paramPrefix = '\0';
        //    string paramName = string.Empty;
        //    string paramValue = string.Empty;

        //    const int lookForPrefix = 0;
        //    const int readParameter = 1;
        //    const int readParameterValue = 2;
        //    int state = readParameterValue;

        //    for (int i = 0; i < RawData.Length; i++)
        //    {
        //        if (state == lookForPrefix)
        //        {
        //            if (RawData[i] == '-')
        //            {
        //                paramPrefix = RawData[i];
        //                state = readParameter;
        //            }
        //        }
        //        else if (state == readParameter)
        //        {
        //            StringBuilder paramNameBuilder = new StringBuilder();

        //            for (; i < RawData.Length && RawData[i] != ' ';)
        //                paramNameBuilder = paramNameBuilder.Append(RawData[i++]);

        //            paramName = paramNameBuilder.ToString();

        //            state = readParameterValue;
        //        }
        //        else if (state == readParameterValue)
        //        {
        //            StringBuilder parameterValueBuilder = new StringBuilder();
        //            bool parameterAdded = false;
        //            bool ignoreSpecialCharacter = false;
        //            bool onQuots = false;

        //            while (i < RawData.Length)
        //            {
        //                // if we should ignore special character
        //                if (!ignoreSpecialCharacter && RawData[i] == '\\')
        //                {
        //                    ignoreSpecialCharacter = true;
        //                    i++;
        //                    continue;
        //                }

        //                if (!ignoreSpecialCharacter && RawData[i] == '\'')
        //                {
        //                    onQuots = !onQuots;
        //                    i++;
        //                    continue;
        //                }

        //                if (RawData[i] == '-')
        //                {
        //                    // if we should NOT ignore special character
        //                    if (!ignoreSpecialCharacter && !onQuots)
        //                    {
        //                        paramValue = parameterValueBuilder.ToString();
        //                        AddParam(ref paramName, ref paramValue, ref paramPrefix);
        //                        paramPrefix = RawData[i];
        //                        state = readParameter;
        //                        parameterAdded = true;
        //                        break;
        //                    }
        //                }

        //                ignoreSpecialCharacter = false;
        //                parameterValueBuilder = parameterValueBuilder.Append(RawData[i++]);
        //            }

        //            if (!parameterAdded)
        //            {
        //                paramValue = parameterValueBuilder.ToString();
        //                AddParam(ref paramName, ref paramValue, ref paramPrefix);
        //                state = lookForPrefix;
        //            }
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(paramName))
        //        AddParam(paramName, paramValue, paramPrefix);
        //}

        private void InterpreteParameters()
        {
            string paramName = string.Empty;
            string paramValue = string.Empty;
            char paramPrefix = '\0';

            List<Param> paramsFound = new List<Param>();

            List<char> prefixes = new List<char>() { '-', '/' };

            const int readParameterName = 0;
            const int readParameterValue = 1;
            const int lookForParam = 2;

            int state = readParameterValue;

            for (int i = 0; i < prefixes.Count; i++)
            {
                if (RawData.StartsWith(prefixes[i].ToString()))
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
                bool isParamPrefix = prefixes.Contains(currentChar);

                if (currentChar == '\\')
                {
                    treatSpecialCharAsString = !treatSpecialCharAsString;
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

        private void AddParam(ref string name, ref string value, ref char prefix)
        {
            AddParam(name, value, prefix);
            name = value = string.Empty;
            prefix = '\0';
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
                    if (Params[i].Name == parameters[j])
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

        #endregion
    }
}

