using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Blackboard_;
using Hash17.Devices;
using Hash17.Terminal_;
using Hash17.Utils;
using UnityEngine;

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
            else if (Parameters.TryGetParam("c", out param) || Parameters.TryGetParam("", out param))
            {
                var deviceId = param.Value;
                Device device;
                if (!Blackboard.Instance.DevicesById.TryGetValue(deviceId, out device))
                {
                    Terminal.Showtext(TextBuilder.WarningText("Device not found."));
                }
                else
                {
                    yield return device.TryAccess((b, device1) =>
                    {
                        if (b)
                        {
                            Terminal.Showtext(TextBuilder.BuildText(string.Format("Access granted. You are now on {0}.", device1.UniqueId), Color.green));
                            Blackboard.Instance.CurrentDevice = device1;
                        }
                        else
                        {
                            Terminal.Showtext(TextBuilder.ErrorText("Access denied."));
                        }
                    });
                }
            }
        }
    }
}
