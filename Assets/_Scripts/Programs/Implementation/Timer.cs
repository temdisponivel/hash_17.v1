using System;
using UnityEngine;
using System.Collections;
using Hash17.Programs;
using Hash17.Utils;
using Object = UnityEngine.Object;

namespace Hash17.Programs.Implementation
{

    public class Timer : Program
    {
        public override string AditionalData
        {
            get { return base.AditionalData; }
            set
            {
                base.AditionalData = value;
                LabelPrefabPath = base.AditionalData;
            }
        }

        public string LabelPrefabPath;
        private Window _windowInstance;

        public event Action OnTimerFinish;

        protected override IEnumerator InnerExecute()
        {
            if (HelpOrUnknownParameters(true))
                yield break;

            var param = Parameters.GetFirstParamWithValue();

            int seconds = 0;
            if (int.TryParse(param.Value, out seconds))
            {
                _windowInstance = Window.Create();
                _windowInstance.LosesFocus = true;

                var label = Object.Instantiate(Resources.Load<GameObject>(LabelPrefabPath)).GetComponent<UILabel>();
                label.SetupWithHash17Settings();
                label.overflowMethod = UILabel.Overflow.ResizeHeight;

                label.rightAnchor.target = _windowInstance.ContentPanel.transform;
                label.leftAnchor.target = _windowInstance.ContentPanel.transform;
                label.text = seconds.ToString();
                label.AssumeNaturalSize();

                _windowInstance.Setup("Timer", label, true, false, startClosed: true);
                _windowInstance.Size = new Vector2(label.width + 20, label.height + 50);
                _windowInstance.SizeLocked = true;

                yield return CoroutineHelper.Instance.WaitAndCallTimes((i) =>
                {
                    if (label != null)
                        label.text = (seconds - i).ToString();
                }, seconds, 1, () =>
                {
                    if (_windowInstance != null)
                        _windowInstance.Close(null);

                    if (OnTimerFinish != null)
                        OnTimerFinish();
                });
            }
            else
            {
                Alias.Term.ShowText("The parameter should be a integer representing seconds to count.");
                yield break;
            }
        }
    }

}