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
        }

        #endregion

        public string RawData;
        public List<Param> Params = new List<Param>();

        public ProgramParameter(string parameters)
        {
            RawData = parameters;
            InterpreteParameters();
        }

        #region Interprete

        public void InterpreteParameters()
        {
            char paramPrefix = '\0';
            string paramName = string.Empty;
            string paramValue = string.Empty;

            const int lookForPrefix = 0;
            const int readParameter = 1;
            const int readParameterValue = 2;
            int state = lookForPrefix;

            for (int i = 0; i < RawData.Length; i++)
            {
                if (state == lookForPrefix)
                {
                    if (RawData[i] == '-')
                    {
                        paramPrefix = RawData[i];
                        state = readParameter;
                    }
                }
                else if (state == readParameter)
                {
                    StringBuilder paramNameBuilder = new StringBuilder();

                    for (; i < RawData.Length && RawData[i] != ' ';)
                        paramNameBuilder = paramNameBuilder.Append(RawData[i++]);

                    paramName = paramNameBuilder.ToString();

                    state = readParameterValue;
                }
                else if (state == readParameterValue)
                {
                    StringBuilder parameterValueBuilder = new StringBuilder();
                    bool parameterAdded = false;
                    bool ignoreSpecialCharacter = false;
                    bool onQuots = false;
                    
                    while (i < RawData.Length)
                    {
                        // if we should ignore special character
                        if (!ignoreSpecialCharacter && RawData[i] == '\\')
                        {
                            ignoreSpecialCharacter = true;
                            i++;
                            continue;
                        }

                        if (!ignoreSpecialCharacter && RawData[i] == '\'')
                        {
                            onQuots = !onQuots;
                            i++;
                            continue;
                        }

                        if (RawData[i] == '-')
                        {
                            // if we should NOT ignore special character
                            if (!ignoreSpecialCharacter && !onQuots)
                            {
                                paramValue = parameterValueBuilder.ToString();
                                AddParam(ref paramName, ref paramValue, ref paramPrefix);
                                paramPrefix = RawData[i];
                                state = readParameter;
                                parameterAdded = true;
                                break;
                            }
                        }

                        ignoreSpecialCharacter = false;
                        parameterValueBuilder = parameterValueBuilder.Append(RawData[i++]);
                    }

                    if (!parameterAdded)
                    {
                        paramValue = parameterValueBuilder.ToString();
                        AddParam(ref paramName, ref paramValue, ref paramPrefix);
                        state = lookForPrefix;
                    }
                }
            }

            if (!string.IsNullOrEmpty(paramName))
                AddParam(paramName, paramValue, paramPrefix);
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
