using System.Collections;
using System.Collections.Generic;
using Hash17.MockSystem;
using Hash17.Programs;
using Hash17.Utils;
using UnityEngine;

namespace Hash17.Campaign
{
    public class CampaignMissionReward
    {
        public List<int> FilesToUnlock;
        public List<int> DevicesToUnlock;
        public List<int> ProgramsToUnlock;
        public List<string> CommandsToRun;

        public virtual Coroutine Execute()
        {
            for (int i = 0; i < FilesToUnlock.Count; i++)
            {
                Alias.Campaign.Info.UnlockedFiles.Add(FilesToUnlock[i]);
            }

            for (int i = 0; i < DevicesToUnlock.Count; i++)
            {
                Alias.Campaign.Info.UnlockedDevices.Add(DevicesToUnlock[i]);
            }

            for (int i = 0; i < ProgramsToUnlock.Count; i++)
            {
                Alias.Campaign.Info.UnlockedPrograms.Add(ProgramsToUnlock[i]);
            }

            return CoroutineHelper.Instance.StartCoroutine(RunCommands());
        }

        protected virtual IEnumerator RunCommands()
        {
            for (int i = 0; i < CommandsToRun.Count; i++)
            {
                Program prog;
                string param;
                if (Alias.Programs.GetProgramAndParameters(CommandsToRun[i], out prog, out param) ==
                    ProgramCollection.ProgramRequestResult.Ok)
                {
                    yield return Alias.Term.RunProgram(prog, param).ExecCoroutine;
                }
            }
        } 
    }
}