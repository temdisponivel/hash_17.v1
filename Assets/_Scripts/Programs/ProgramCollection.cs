﻿using System;
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
        private string _serializedData;

        [NonSerialized]
        public List<Program> Programs;

        public void Save()
        {
            var data = JsonConvert.SerializeObject(Programs, new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
            });
            _serializedData = data;
        }

        public void Load()
        {
            Programs = JsonConvert.DeserializeObject<List<Program>>(_serializedData, new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
            });
        }
    }
}