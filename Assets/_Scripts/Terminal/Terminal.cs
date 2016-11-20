using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hash17.Blackboard_;
using Hash17.Programs;
using Hash17.Utils;
using UnityEditor;

namespace Hash17.Terminal_
{
    public class Terminal : PersistentSingleton<Terminal>
    {
        #region Properties

        public GameObject TextEntryPrefab;
        public UITable TextTable;
        public UIInput Input;
        public UILabel LabelUserNameLocation;

        private readonly  List<IProgram> _runningPrograms = new List<IProgram>();
        public List<IProgram> RunningPrograms
        {
            get
            {
                return _runningPrograms;
            }
        }

        private bool _blockInput;
        public bool BlockInput
        {
            get
            {
                return _blockInput;
            }
            set
            {
                _blockInput = value;
                Input.enabled = !_blockInput;
            }
        }
        
        public bool TreatInput { get; set; }

        private string _currentLocation;
        public string CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                UpdateUserNameLocation();
            }
        }

        private string _currentUserName;
        public string CurrentUserName
        {
            get { return _currentUserName; }
            set
            {
                _currentUserName = value; 
                UpdateUserNameLocation();
            }
        }

        public string CurrentLocationAndUserName
        {
            get { return string.Format("{0}:{1}>", CurrentUserName, CurrentLocation); }
        }

        public event Action<IProgram> OnRunProgram;
        public event Action<string> OnInputSubmited;
        public event Action OnInputValueChange;

        #endregion

        #region Input

        public void InputValueChange()
        {
            if (OnInputValueChange != null)
                OnInputValueChange();
        }

        public void OnInputSubmit()
        {
            string value = Input.value;
            if (TreatInput)
            {
                TreatInputText(Input.value);
                Input.value = string.Empty;

                if (OnInputSubmited != null)
                    OnInputSubmited(value);
            }
        }

        private void TreatInputText(string text)
        {
            text = text.Replace("\n", string.Empty);
            AddText(text);

            string programName, programParams;
            if (Interpreter.GetProgram(text, out programName, out programParams))
            {
                IProgram program;
                if (Blackboard.Instance.Programs.TryGetValue(programName, out program))
                {
                    var programInstance = program.Clone();
                    RunningPrograms.Add(programInstance);
                    programInstance.Execute(programParams);

                    if (OnRunProgram != null)
                        OnRunProgram(programInstance);
                }
            }
        }

        private void UpdateUserNameLocation()
        {
            LabelUserNameLocation.text = CurrentLocationAndUserName;
        }

        #endregion

        #region Interface

        public void AddText(string text)
        {
            var newText = NGUITools.AddChild(TextTable.gameObject, TextEntryPrefab);
            newText.transform.SetAsFirstSibling();
            newText.GetComponent<TextEntry>().Setup(CurrentLocationAndUserName, text);
            TextTable.Reposition();
        }

        public void ClearAll()
        {
            Clear(TextTable.children.Count);
        }

        public void Clear(int quantity)
        {
            for (int i = TextTable.children.Count - 1; i >= 0 && quantity > 0; i--)
            {
                quantity--;
                Destroy(TextTable.children[i].gameObject);
            }

            TextTable.Reposition();
        }

        #endregion
    }
}