using System;
using System.Collections;
using Hash17.Terminal_;
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

            BlockTerminal();

            Callback = callback;
            Device = device;

            Terminal.Showtext(TextBuilder.BuildText(string.Format("Password for {0}:", device.UniqueId), Color.gray));
        }

        void InputSubmited(string input)
        {
            var passDevice = Device as PasswordedDevice;
            var result = passDevice.Password == input;
            if (Callback != null)
            {
                if (result)
                {
                    UnblockTerminal();
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
                        Terminal.Showtext(string.Format("Invalid password. You have {0} more tries.", 3 - _tries));
                        Terminal.Showtext(TextBuilder.BuildText("Password:", Color.gray));
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
            Terminal.Instance.TreatInput = false;
            Terminal.Instance.ShowTextWhenNotTreatingInput = true;
            Terminal.Instance.OnInputSubmited += InputSubmited;
        }

        private void UnblockTerminal()
        {
            Terminal.Instance.TreatInput = true;
            Terminal.Instance.ShowTextWhenNotTreatingInput = false;
            Terminal.Instance.OnInputSubmited -= InputSubmited;
        }
    }
}