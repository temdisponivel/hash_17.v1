using System;
using Hash17.Blackboard_;
using Hash17.Devices.Firewalls;
using System.Collections;
using UnityEditor;

namespace Hash17.Devices
{
    public class PasswordedDevice : Device
    {
        public string Password;

        public override DeviceType DeviceType
        {
            get
            {
                return DeviceType.Passworded;
            }
        }

        public override IEnumerator TryAccess(Action<bool, Device> callback)
        {
            yield return Blackboard.Instance.Firewalls[FirewallType.Password].Clone().Access(callback, this);
        }

#if UNITY_EDITOR
        public override void DrawDeviceInspector()
        {
            base.DrawDeviceInspector();
            Password = EditorGUILayout.TextField("Password", Password);
        }
#endif
    }
}