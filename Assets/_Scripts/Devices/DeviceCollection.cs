using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using Hash17.Programs;
using Newtonsoft.Json;
using UnityEngine;

namespace Hash17.Devices
{
    [CreateAssetMenu(fileName = "Device Collection", menuName = "Hash17/Device collection")]
    public class DeviceCollection : ScriptableObject
    {
        [SerializeField]
        private string _serializedData;

        [NonSerialized]
        public List<Device> Devices;

        public void Save()
        {
            var data = JsonConvert.SerializeObject(Devices, new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
            });
            _serializedData = data;
        }

        public void Load()
        {
            Devices = JsonConvert.DeserializeObject<List<Device>>(_serializedData, new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
            });
        }
    }
}