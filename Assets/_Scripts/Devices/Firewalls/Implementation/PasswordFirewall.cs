using System;
using System.Collections;
using Hash17.Data;
using MockSystem;
using Hash17.Utils;
using UnityEngine;

namespace Hash17.Devices.Firewalls.Implementation
{
    public class PasswordFirewall : IFirewall
    {
        public Device Device;
        public Action<bool, Device> Callback;

        private int _tries = 1;

        public IEnumerator Access(Action<bool, Device> callback, Device device)
        {
            yield return null;

            if (Alias.Campaign.Info.CrackedDevices.Contains(device.UniqueId))
                callback(true, device);

            BlockTerminal();

            Callback = callback;
            Device = device;

            Alias.Term.ShowText(TextBuilder.MessageText(string.Format("Password for {0}:", device.UniqueId)));
        }

        void InputSubmited(string input)
        {
            var passDevice = Device as PasswordedDevice;
            var result = passDevice.Password.Trim() == input.Trim();
            if (Callback != null)
            {
                if (result)
                {
                    UnblockTerminal();
                    Alias.Campaign.Info.CrackedDevices.Add(Device.UniqueId);
                    Callback(true, Device);
                }
                else
                {
                    if (_tries == 3)
                    {
                        UnblockTerminal();
                        Callback(false, null);
                    }
                    else
                    {
                        Alias.Term.ShowText(string.Format("Invalid password. You have {0} more tries.", 3 - _tries));
                        Alias.Term.ShowText(TextBuilder.MessageText("Password:"));
                    }
                }
            }
            _tries++;
        }

        public IFirewall Clone()
        {
            return MemberwiseClone() as IFirewall;
        }

        private void BlockTerminal()
        {
            Alias.Term.TreatInput = false;
            Alias.Term.ShowTextWhenNotTreatingInput = true;
            Alias.Term.ShowUserLocationLabel = false;
            Alias.Term.OnInputSubmited += InputSubmited;
        }

        private void UnblockTerminal()
        {
            Alias.Term.TreatInput = true;
            Alias.Term.ShowTextWhenNotTreatingInput = false;
            Alias.Term.ShowUserLocationLabel = true;
            Alias.Term.OnInputSubmited -= InputSubmited;
        }
    }
}