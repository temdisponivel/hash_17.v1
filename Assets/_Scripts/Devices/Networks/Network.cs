using System;
using System.Collections.Generic;
using Hash17.Devices.Security;
using UnityEngine;

namespace Hash17.Devices.Networks
{
    public class Network : INetwork
    {
        public string Name { get; set; }
        public string Identifier { get; set; }
        public List<IProtected> Protecteds { get; set; }

        public void Access(string firewallPassCode, Action<bool, string, INetwork> callback)
        {
            
        }
    }
}