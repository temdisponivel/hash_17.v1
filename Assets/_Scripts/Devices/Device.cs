using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Blackboard_;
using Hash17.Devices.Firewalls;
using Hash17.Files;
using Hash17.Programs;

namespace Hash17.Devices
{
    [Serializable]
    public class Device
    {
        public string Name;
        public string Id;
        public List<Program> Programs;
        public FileSystem FileSystem;

        public virtual IEnumerator TryAccess(Action<bool, Device> callback)
        {
            yield return null;
            if (callback != null)
                callback(true, this);
        }
    }
}
