using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hash17.Terminal_;
using UnityEngine;

namespace Hash17.Programs.Implementation
{
    public class Init : Program
    {
        public string InitTextPath;

        protected override IEnumerator InnerExecute()
        {
            BlockInput();
            var text = Resources.Load<TextAsset>(InitTextPath);
            yield return Terminal.Instance.ShowTextWithInterval(text.text, callback: UnblockInput);
        }
    }
}
