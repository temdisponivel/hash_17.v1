using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Blackboard_;
using Hash17.Terminal_;

namespace Hash17.Programs.Implementation
{
    public class Connect : Program
    {
        protected override IEnumerator InnerExecute()
        {
            if (AskedForHelp(true))
                yield break;

            ProgramParameter.Param param;
            if (Parameters.ContainParam("s"))
            {
                for (int i = 0; i < Blackboard.Instance.Devices.Count; i++)
                {
                    Terminal.Showtext(string.Format("{0}: {1}", Blackboard.Instance.Devices[i].UniqueId, Blackboard.Instance.Devices[i].Name));
                }
            }
            else if (Parameters.TryGetParam("c", out param))
            {
                var deviceId = param.Value;
                var device = Blackboard.Instance.Devices.Find(d => d.UniqueId == deviceId);

                if (device == null)
                {

                }
                else
                {
                    yield return device.TryAccess((b, device1) =>
                    {
                        if (b)
                            Blackboard.Instance.CurrentDevice = device1;
                        else
                            Terminal.Showtext("Access denied.");
                    });
                }
            }
        }
    }
}
