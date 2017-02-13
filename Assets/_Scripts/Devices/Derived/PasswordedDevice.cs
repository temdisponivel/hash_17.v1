namespace Hash17.Devices
{
    public class PasswordedDevice : Device
    {
        public string Password;
        public override DeviceType DeviceType { get { return DeviceType.Passworded; } }
    }
}