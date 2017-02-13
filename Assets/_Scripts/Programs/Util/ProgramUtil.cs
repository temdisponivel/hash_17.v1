using Hash17.Files;
using Hash17.Utils;

namespace Hash17.Programs.Util
{
    public static class ProgramUtil
    {
        #region Devices

        public static void ShowDevices()
        {
            for (int i = 0; i < Alias.Devices.Count; i++)
            {
                var deviceId = TextBuilder.BuildText(Alias.Devices[i].Id, Alias.Config.DeviceIdColor);
                var deviceName = Alias.Devices[i].Name;
                Alias.Term.ShowText(string.Format("ID: {0} | NAME: {1}", deviceId.PadRight(10), deviceName));
            }
        }

        #endregion

        #region Files

        public static void ShowFiles(Directory dir)
        {
            const int typePad = 6;
            const int namePad = 50;
            const int statusPad = 10;
            const int colorTagSize = 11;
            const int total = typePad + namePad + statusPad - colorTagSize;
            Alias.Term.ShowText("{0}{1}{2}".InLineFormat("TYPE".PadRight(typePad),
                "NAME".PadRight(namePad - colorTagSize), "STATUS".PadRight(statusPad)));
            Alias.Term.ShowText("".PadRight(total, '-'));

            var childs = dir.GetAvailableChilds();
            for (int i = 0; i < childs.Count; i++)
            {
                Alias.Term.ShowText("{0}{1}".InLineFormat("DIR:".PadRight(typePad), childs[i].PrettyName));
            }

            var files = dir.GetAvailableFiles();
            for (int i = 0; i < files.Count; i++)
            {
                string status;
                if (files[i].IsProtected)
                {
                    if (files[i].CanBeRead)
                        status = "Descrypted";
                    else
                        status = "Encrypted";
                }
                else
                {
                    status = "Normal";
                }
                var fileState = status.PadRight(statusPad);
                Alias.Term.ShowText("{0}{1}{2}".InLineFormat("FILE:".PadRight(typePad),
                    files[i].PrettyName.PadRight(namePad), fileState));
            }
        }

        #endregion
    }
}