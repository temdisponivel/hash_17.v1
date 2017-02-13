namespace Hash17.Devices
{
    public class RunningSystem
    {
        public int SystemId { get { return SystemName.GetHashCode(); } }
        public string SystemName { get; set; }
        public bool Status { get; set; }
        public int RunningPort { get; set; }
    }
}