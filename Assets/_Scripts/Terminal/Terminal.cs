using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Hash17.Blackboard_;
using Hash17.Programs;
using Hash17.Programs.Implementation;
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
            get { return string.Format("{0}:{1}{2}", Blackboard.Instance.FileSystem.CurrentDirectory.Path, Blackboard.Instance.CurrentDevice.Id, CurrentUserName); }
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
            RunProgram(Blackboard.Instance.SpecialPrograms[ProgramId.Init], string.Empty);
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
                IProgram programInstance = null;

                Program deviceProgram;
                if ((deviceProgram = Blackboard.Instance.CurrentDevice.Programs.Find(p => p.Definition.Command == programName)) != null)
                {
                    programInstance = RunProgram(deviceProgram, programParams);
                }
                else if (!program.DeviceIndependent)
                {
                    ShowText(TextBuilder.WarningText(string.Format("Unknow command \"{0}\"", text)));
                    ShowText(TextBuilder.WarningText("Type \"help\" to get some help"));
                    return;
                }
                else
                {
                    programInstance = RunProgram(program, programParams);
                }


                if (OnProgramExecuted != null)
                    OnProgramExecuted(programInstance);

                programInstance.OnFinish += ProgramFinished;
            }
            else
            {
                ShowText(TextBuilder.WarningText(string.Format("Unknow command \"{0}\"", text)));
                ShowText(TextBuilder.WarningText("Type \"help\" to get some help"));
            }
        }

        private IProgram RunProgram(IProgram program, string param)
        {
            var programInstance = program.Clone();
            RunningPrograms.Add(programInstance);
            programInstance.Execute(param);
            return programInstance;
        }

        private void ClearInput()
        {
            Input.value = string.Empty;
            _currentNavigationCommandIndex = -1;
            Input.isSelected = true;
        }

        public void UpdateUserNameLocation()
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

        public void ShowText(string text, bool asNewLine = true)
        {
            TextEntry entry;
            if (asNewLine)
            {
                var newText = NGUITools.AddChild(TextTable.gameObject, TextEntryPrefab);
                newText.transform.SetAsFirstSibling();
                entry = newText.GetComponent<TextEntry>();
            }
            else
            {
                if (TextTable.transform.childCount == 0)
                {
                    ShowText(text);
                    return;
                }

                entry = TextTable.transform.GetChild(0).GetComponent<TextEntry>();
            }

            entry.Setup(CurrentLocationAndUserName, text);
            TextTable.Reposition();
        }

        public static void Showtext(string text)
        {
            Instance.ShowText(text);
        }

        public Coroutine ShowTextWithInterval(string text, float intervalBetweenChars = .02f, Action callback = null)
        {
            StringBuilder textBuilder = new StringBuilder();
            return CoroutineHelper.Instance.WaitAndCallTimes((index) =>
            {
                textBuilder = textBuilder.Append(text[index]);
                ShowText(textBuilder.ToString(), false);
            }, text.Length, intervalBetweenChars, callback);
        }

        public void ClearAll()
        {
            Clear(TextTable.GetChildList().Count);
        }

        public void Clear(int quantity)
        {
            if (quantity == 0)
                return;

            for (int i = 0; quantity > 0 && i < TextTable.GetChildList().Count; i++)
            {
                quantity--;

                if (TextTable.GetChildList()[i])
                    Destroy(TextTable.GetChildList()[i].gameObject);
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