using System;
using System.Collections;
using System.Collections.Generic;
using Hash17.Blackboard_;
using Hash17.Terminal_;
using Hash17.Utils;
using UnityEngine;

namespace Hash17.Programs
{
    [Serializable]
    public class Program
    {
        #region Properties
        
        public ProgramId Id;
        public int UnitqueId;
        public string Command;
        public string Description;
        public string Usage;
        public string[] KnownParametersAndOptions;
        public bool Global = true;

        public ProgramParameter Parameters { get; set; }
        public bool Running { get; private set; }
        protected Coroutine ExecCoroutine { get; set; }

        #endregion

        #region Events

        public event Action<Program> OnStart;
        public event Action<Program> OnFinish;

        #endregion

        #region Program

        public void Execute(string parameters)
        {
            Parameters = new ProgramParameter(parameters);
            
            Running = true;
            ExecCoroutine = StartCoroutine(InnerExecute());
            StartCoroutine(WaitToFinish());

            if (OnStart != null)
                OnStart(this);
        }

        public virtual void Stop()
        {
            if (ExecCoroutine != null)
                StopCoroutine(ExecCoroutine);
            ExecCoroutine = null;
            FinishExecution();
        }

        public Program Clone()
        {
            return MemberwiseClone() as Program;
        }
        
        protected bool AskedForHelp(bool showHelpIftrue)
        {
            bool result = Parameters.ContainParam("h");
            if (showHelpIftrue && result)
            {
                ShowHelp();
            }

            return result;
        }

        public void ShowHelp()
        {
            Terminal.Instance.BeginIdentation();
            Terminal.Instance.ShowText(Description);
            Terminal.Instance.ShowText(Usage, ident: true);
            Terminal.Instance.EndIdentation();
        }

        protected bool ValidateUnknowParameters(bool shouldShowUsage)
        {
            List<ProgramParameter.Param> unknownParams;
            bool result = Parameters.HasParamOtherThan(out unknownParams, KnownParametersAndOptions);
            if (result)
            {
                for (int i = 0; i < unknownParams.Count; i++)
                {
                    Terminal.Instance.ShowText(TextBuilder.WarningText(string.Format("Unknow parameter {0}.", unknownParams[i].Name)));
                }
                Terminal.Instance.ShowText(Usage);
            }

            return result;
        }

        public bool HelpOrUnknownParameters(bool shouldShowUsage)
        {
            if (AskedForHelp(shouldShowUsage))
            {
                return true;
            }

            if (ValidateUnknowParameters(shouldShowUsage))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Helpers

        protected virtual IEnumerator InnerExecute() { yield break; }

        protected void BlockInput()
        {
            Terminal.Instance.BlockInput = true;
        }

        protected void UnblockInput()
        {
            Terminal.Instance.BlockInput = false;
        }

        protected void FinishExecution()
        {
            Running = false;

            if (OnFinish != null)
                OnFinish(this);
        }

        private IEnumerator WaitToFinish()
        {
            yield return ExecCoroutine;

            if (Running)
                FinishExecution();
        }
        
        #endregion

        #region Coroutine

        protected Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return CoroutineHelper.Instance.StartCoroutine(coroutine);
        }

        protected void StopCoroutine(Coroutine coroutine)
        {
            CoroutineHelper.Instance.StopCoroutine(coroutine);
        }

        #endregion
    }
}
