using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Data;
using Hash17.Devices;
using Hash17.MockSystem;
using Hash17.Programs.Util;
using MockSystem;
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

            ManuallyFinished = true;

            ProgramParameter.Param param;
            if (Parameters.ContainParam("s"))
            {
                ProgramUtil.ShowDevices();

                FinishExecution();
                yield break;
            }

            if (Parameters.TryGetParam("c", out param) || Parameters.TryGetParam("", out param))
            {
                var deviceId = param.Value;
                Device device;
                if (!Alias.Devices.GetDeviceById(deviceId.GetHashCode(), out device))
                {
                    Alias.Term.ShowText(TextBuilder.WarningText("Device not found."));

                    FinishExecution();
                }
                else
                {
                    yield return device.TryAccess((b, device1) =>
                    {
                        if (b)
                        {
                            Alias.Term.ShowText(
                                TextBuilder.MessageText(string.Format("Access granted. You are now on {0}.",
                                    device1.Id)));
                            Alias.Devices.ChangeCurrentDevice(device1);
                        }
                        else
                        {
                            Alias.Term.ShowText(TextBuilder.ErrorText("Access denied."));
                        }

                        FinishExecution();
                    });
                }

                yield break;
            }

            ShowHelp();

            FinishExecution();
        }
    }
}
