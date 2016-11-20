using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hash17.Programs
{
    public class ProgramParameter
    {
        #region Inner classes

        public class Param
        {
            public string Name;
            public string Value;
            public bool IsOption;
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

            int state = 0;
            const int lookForPrefix = 0;
            const int readParameter = 1;
            const int readParameterValue = 2;

            for (int i = 0; i < RawData.Length; i++)
            {
                if (state == lookForPrefix)
                {
                    if (RawData[i] == '/' || RawData[i] == '-')
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

                    // if it's a option, doesn't look for value
                    if (paramPrefix == '-')
                    {
                        AddParam(ref paramName, ref paramValue, ref paramPrefix);
                        state = lookForPrefix;
                        continue;
                    }

                    state = readParameterValue;
                }
                else if (state == readParameterValue)
                {
                    if (RawData[i] == '/' || RawData[i] == '-')
                    {
                        AddParam(ref paramName, ref paramValue, ref paramPrefix);
                        paramPrefix = RawData[i];
                        state = readParameter;
                        continue;
                    }

                    StringBuilder parameterValueBuilder = new StringBuilder();

                    while (i < RawData.Length && RawData[i] != ' ')
                        parameterValueBuilder = parameterValueBuilder.Append(RawData[i++]);

                    paramValue = parameterValueBuilder.ToString();

                    AddParam(ref paramName, ref paramValue, ref paramPrefix);
                    state = lookForPrefix;
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
                IsOption = prefix == '-',
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
