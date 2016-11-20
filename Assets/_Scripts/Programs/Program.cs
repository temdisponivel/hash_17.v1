using System;
using System.Collections;
using Hash17.Terminal_;
using Hash17.Utils;
using UnityEngine;

namespace Hash17.Programs
{
    public abstract class Program : MonoBehaviour, IProgram
    {
        #region Properties

        public event Action<IProgram> OnStart;
        public event Action<IProgram> OnFinish;
        public string Parameters { get; set; }

        public bool Running { get; private set; }

        protected Coroutine ExecCoroutine { get; set; }

        #endregion

        #region IProgram

        public void Execute(string parameters)
        {
            Parameters = parameters;
            
            Running = true;
            ExecCoroutine = StartCoroutine(InnerExecute(parameters));
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
        
        #endregion

        #region Helpers

        protected abstract IEnumerator InnerExecute(string parameters);

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

            Terminal.Instance.ShowText(TextBuilder.ErrorText("FINISHING TERMINAL"));

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

        #endregion
    }
}
