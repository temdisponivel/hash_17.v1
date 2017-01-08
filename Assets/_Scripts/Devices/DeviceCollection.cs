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
        private List<string> _serializedData;
        
        [NonSerialized]
        public List<Device> Devices;

        public void Save()
        {
            _serializedData = new List<string>();
            for (int i = 0; i < Devices.Count; i++)
            {
                var current = Devices[i];
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
            Devices = new List<Device>();
            for (int i = 0; i < _serializedData.Count; i++)
            {
                var current = _serializedData[i];
                Devices.Add(JsonConvert.DeserializeObject<Device>(current, new JsonSerializerSettings()
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                }));
            }
        }
    }
}