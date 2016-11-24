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

        private readonly List<IProgram> _runningPrograms = new List<IProgram>();
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

        private bool _treatInput = true;
        public bool TreatInput
        {
            get { return _treatInput; }
            set { _treatInput = value; }
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
            get { return string.Format("{0}:{1}", Blackboard.Instance.FileSystem.CurrentDirectory.Path, CurrentUserName); }
        }

        public event Action<IProgram> OnProgramExecuted;
        public event Action<IProgram> OnProgramFinished;
        public event Action<string> OnInputSubmited;
        public event Action OnInputValueChange;

        public readonly List<string> AllCommandsTyped = new List<string>();
        private int _currentNavigationCommandIndex = -1;

        #endregion

        #region Unity events

        protected override void Awake()
        {
            base.Awake();
            ClearInput();
            CurrentUserName = "#17";
        }

        protected void Start()
        {
            Blackboard.Instance.FileSystem.OnChangeCurrentDirectory += OnCurrentDirChanged;
        }

        #endregion

        #region Programs

        public void InputValueChange()
        {
            if (OnInputValueChange != null)
                OnInputValueChange();
        }

        public void OnInputSubmit()
        {
            string value = Input.value.Trim();

            if (TreatInput)
            {
                TreatInputText(Input.value);

                ClearInput();

                value = value.Replace("\n", string.Empty);
                if (!string.IsNullOrEmpty(value))
                    AllCommandsTyped.Insert(0, value);
            }

            if (OnInputSubmited != null)
                OnInputSubmited(value);
        }

        private void TreatInputText(string text)
        {
            text = text.Replace("\n", string.Empty);
            ShowText(text);

            string programName, programParams;
            Interpreter.GetProgram(text, out programName, out programParams);
            IProgram program;
            if (Blackboard.Instance.Programs.TryGetValue(programName, out program))
            {
                var programInstance = program.Clone();
                RunningPrograms.Add(programInstance);
                programInstance.Execute(programParams);

                if (OnProgramExecuted != null)
                    OnProgramExecuted(programInstance);

                programInstance.OnFinish += ProgramFinished;
            }
            else
            {
                ShowText(TextBuilder.WarningText(string.Format("Unknow command \"{0}\"", text)));
            }
        }

        private void ClearInput()
        {
            Input.value = string.Empty;
            _currentNavigationCommandIndex = -1;
            Input.isSelected = true;
        }

        private void UpdateUserNameLocation()
        {
            LabelUserNameLocation.text = CurrentLocationAndUserName;
        }

        public void OnUpPressed()
        {
            _currentNavigationCommandIndex = Math.Min(AllCommandsTyped.Count - 1, _currentNavigationCommandIndex + 1);
            UpdateCommandLineToCommandIndex();
        }

        public void OnDownPressed()
        {
            _currentNavigationCommandIndex = Math.Max(0, _currentNavigationCommandIndex - 1);
            UpdateCommandLineToCommandIndex();
        }

        public void OnEscPressed()
        {
            ClearInput();
        }

        public void UpdateCommandLineToCommandIndex()
        {
            if (AllCommandsTyped.Count != 0)
                Input.value = AllCommandsTyped[_currentNavigationCommandIndex];
        }

        #endregion

        #region Interface

        public void ShowText(string text)
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
            if (quantity == 0)
                return;

            for (int i = 0; quantity > 0 && i < TextTable.children.Count; i++)
            {
                quantity--;

                if (TextTable.children[i])
                    Destroy(TextTable.children[i].gameObject);
            }

            TextTable.Reposition();
        }

        #endregion

        #region Callbacks

        private void ProgramFinished(IProgram program)
        {
            RunningPrograms.Remove(program);

            program.OnFinish -= ProgramFinished;

            if (OnProgramFinished != null)
                OnProgramFinished(program);
        }

        private void OnCurrentDirChanged()
        {
            UpdateUserNameLocation();
        }

        #endregion
    }
}