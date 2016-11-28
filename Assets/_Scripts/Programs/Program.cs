using System;
using System.Collections;
using System.Collections.Generic;
using Hash17.Blackboard_;
using Hash17.Terminal_;
using Hash17.Utils;
using UnityEngine;

namespace Hash17.Programs
{
    public abstract class Program : MonoBehaviour, IProgram
    {
        #region Properties

        public ProgramId Id;
        public event Action<IProgram> OnStart;
        public event Action<IProgram> OnFinish;
        public ProgramParameter Parameters { get; set; }

        public bool Running { get; private set; }

        protected Coroutine ExecCoroutine { get; set; }

        private ProgramScriptableObject _definition;
        public ProgramScriptableObject Definition
        {
            get
            {
                if (_definition == null)
                    _definition = Blackboard.Instance.ProgramDefinitionById[Id];

                return _definition;

            }
        }

        #endregion
        
        #region IProgram

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

        public IProgram Clone()
        {
            return Instantiate(this).GetComponent<IProgram>();
        }

        public virtual string GetDescription()
        {
            if (Definition != null)
                return Definition.Description;

            return string.Empty;
        }

        public virtual string GetUsage()
        {
            if (Definition != null)
                return Definition.Usage;

            return string.Empty;
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
            Terminal.Instance.ShowText(GetDescription());
            Terminal.Instance.ShowText(GetUsage());
        }

        protected bool ValidateUnknowParameters(bool shouldShowUsage)
        {
            List<ProgramParameter.Param> unknownParams;
            bool result = Parameters.HasParamOtherThan(out unknownParams, Definition.KnownParametersAndOptions);
            if (result)
            {
                for (int i = 0; i < unknownParams.Count; i++)
                {
                    Terminal.Instance.ShowText(TextBuilder.WarningText(string.Format("Unknow parameter {0}.", unknownParams[i].Name)));
                }
                Terminal.Instance.ShowText(GetUsage());
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

        protected abstract IEnumerator InnerExecute();

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

            Destroy(this.gameObject);
        }

        private IEnumerator WaitToFinish()
        {
            yield return ExecCoroutine;

            if (Running)
                FinishExecution();
        }

        protected void LoadUsageFromAsset()
        {
            
        }

        #endregion
    }
}
