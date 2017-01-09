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
            if (HelpOrUnknownParameters(true))
                yield break;

            ProgramParameter.Param param;
            if (Parameters.ContainParam("s"))
            {
                for (int i = 0; i < Alias.Board.Devices.Count; i++)
                {
                    Alias.Term.ShowText(string.Format("{0}: {1}", Alias.Board.Devices[i].UniqueId, Alias.Board.Devices[i].Name));
                }
            }
            else if (Parameters.TryGetParam("c", out param) || Parameters.TryGetParam("", out param))
            {
                var deviceId = param.Value;
                Device device;
                if (!Alias.Board.DevicesById.TryGetValue(deviceId, out device))
                {
                    Alias.Term.ShowText(TextBuilder.WarningText("Device not found."));
                }
                else
                {
                    yield return device.TryAccess((b, device1) =>
                    {
                        if (b)
                        {
                            Alias.Term.ShowText(
                                TextBuilder.MessageText(string.Format("Access granted. You are now on {0}.",
                                    device1.UniqueId)));
                            Alias.Board.CurrentDevice = device1;
                        }
                        else
                        {
                            Alias.Term.ShowText(TextBuilder.ErrorText("Access denied."));
                        }
                    });
                }
            }
            else
            {
                ShowHelp();
            }
        }
    }
}
