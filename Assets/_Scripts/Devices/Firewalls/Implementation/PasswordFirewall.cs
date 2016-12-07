using System;
using System.Collections;
using Hash17.Terminal_;

namespace Hash17.Devices.Firewalls.Implementation
{
    public class PasswordFirewall : IFirewall
    {
        public Device Device;
        public Action<bool, Device> Callback;

        private int _tries;

        public IEnumerator Access(Action<bool, Device> callback, Device device)
        {
            yield return null;

            Callback = callback;
            Device = device;

            Terminal.Instance.BlockInput = false;
            Terminal.Instance.OnInputSubmited += InputSubmited;
        }

        void InputSubmited(string input)
        {
            var passDevice = Device as PasswordedDevice;
            var result = passDevice.Password == input;
            if (Callback != null)
            {
                if (result)
                {
                    Callback(true, Device);
                }
                else
                {
                    if (_tries == 3)
                        Callback(false, null);
                    else
                        Terminal.Showtext(string.Format("Invalid password. You have {0} more tries.", 3 - _tries));
                }
            }
            _tries++;
        }

        public IFirewall Clone()
        {
            return MemberwiseClone() as IFirewall;
        }
    }
}