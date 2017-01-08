using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Hash17.Programs
{
    [CreateAssetMenu(fileName = "Device Collection", menuName = "Hash17/Program collection")]
    public class ProgramCollection : ScriptableObject
    {
        [SerializeField]
        private List<string> _serializedData;

        [NonSerialized]
        public List<Program> Programs;

        public void Save()
        {
            _serializedData = new List<string>();
            for (int i = 0; i < Programs.Count; i++)
            {
                var current = Programs[i];
                var data = JsonConvert.SerializeObject(current, new JsonSerializerSettings()
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                });
                _serializedData.Add(data);
            }
        }

        public void Load()
        {
            Programs = new List<Program>();
            for (int i = 0; i < _serializedData.Count; i++)
            {
                var current = _serializedData[i];
                Programs.Add(JsonConvert.DeserializeObject<Program>(current, new JsonSerializerSettings()
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                }));
            }
        }
    }
}