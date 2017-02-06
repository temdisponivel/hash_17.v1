using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Hash17.MockSystem;
using Hash17.Programs;
using Hash17.Utils;

namespace MockSystem.Term
{
    public class Terminal : Singleton<Terminal>
    {
        #region Properties

        public UIPanel RootPanel;
        public GameObject TextEntryPrefab;
        public UIScrollView TextScrollView;
        public UITable TextTable;
        public UIWidget TextTableWidget;
        public UIInput Input;
        public UILabel LabelUserNameLocation;
        public UILabel CarrotLabel;

        private readonly List<Program> _runningPrograms = new List<Program>();
        public List<Program> RunningPrograms
        {
            get
            {
                return _runningPrograms;
            }
        }

        #region Input

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
                Input.gameObject.SetActive(!_blockInput);
            }
        }

        private bool _treatInput = true;
        public bool TreatInput
        {
            get { return _treatInput; }
            set { _treatInput = value; }
        }

        private bool _showTextWhenNotTreatingInput = true;
        public bool ShowTextWhenNotTreatingInput
        {
            get { return _showTextWhenNotTreatingInput; }
            set { _showTextWhenNotTreatingInput = value; }
        }

        private bool _showUserLocationLabel = false;
        public bool ShowUserLocationLabel
        {
            get { return _showUserLocationLabel; }
            set
            {
                _showUserLocationLabel = value;

                var previousPivot = TextTableWidget.pivot;
                TextTableWidget.pivot = UIWidget.Pivot.Top;

                if (_showUserLocationLabel)
                {
                    LabelUserNameLocation.AssumeNaturalSize();
                    TextTableWidget.height -= (LabelUserNameLocation.height + 5);
                }
                else
                {
                    TextTableWidget.height += (LabelUserNameLocation.height + 5);
                    LabelUserNameLocation.height = 0;
                }

                TextTableWidget.pivot = previousPivot;

                RepositionText();
            }
        }

        private bool _showInputCarrot = false;
        public bool ShowInputCarrot
        {
            get { return _showInputCarrot; }
            set
            {
                _showInputCarrot = value;
                CarrotLabel.gameObject.SetActive(_showInputCarrot);
            }
        }

        private bool _handleSystemVariables = true;
        public bool HandleSystemVariables { get { return _handleSystemVariables; } set { _handleSystemVariables = value; } }

        #endregion

        #region User name

        public string CurrentUserName
        {
            get { return Alias.Campaign.Info.PlayerName; }
            set
            {
                Alias.SysVariables[SystemVariableType.USERNAME] = value;
                UpdateUserNameLocation();
            }
        }

        public string CurrentLocationAndUserName
        {
            get
            {
                var userName = TextBuilder.BuildText(CurrentUserName, Alias.Config.UserNameColor);
                var deviceId = TextBuilder.BuildText(DeviceCollection.CurrentDevice.Id, Alias.Config.DeviceIdColor);
                var currentDir = TextBuilder.BuildText(DeviceCollection.FileSystem.CurrentDirectory.Path, Alias.Config.DirectoryColor);

                return string.Format("{0} @ {1} : {2}", userName, deviceId, currentDir);
            }
        }

        #endregion

        #region Commands

        public readonly List<string> AllCommandsTyped = new List<string>();
        private int _currentNavigationCommandIndex = -1;

        #endregion

        #region Identation

        private readonly StringBuilder _identationBuilder = new StringBuilder();
        private int _lastIdentationQuantity;

        #endregion

        #endregion

        #region Events

        public event Action<Program> OnProgramExecuted;
        public event Action<Program> OnProgramFinished;
        public event Action<string> OnInputSubmited;
        public event Action OnInputValueChange;

        #endregion

        #region Unity events

        protected void Start()
        {
            Alias.Devices.OnCurrentDeviceChange += OnCurrentDeviceChange;
            FileSystem.OnChangeCurrentDirectory += OnCurrentDirChanged;
            Alias.SysVariables.OnSystemVariableChange += OnSystemVariableChange;

            Input.label.SetupWithHash17Settings();
            LabelUserNameLocation.SetupWithHash17Settings();
            CarrotLabel.SetupWithHash17Settings();

            CarrotLabel.text = Alias.Config.CarrotChar;

            UpdateUserNameLocation();

            Program specialProgram;
            if (!Alias.Programs.GetSpecialProgramByType(ProgramType.Init, out specialProgram))
            {
                Debug.LogError("ERROR TRYING TO GET {0} SPECIAL PROGRAM".InLineFormat(ProgramType.Init));
            }

            RunProgram(specialProgram, string.Empty);
        }

        #endregion

        #region ProgramsByCommand

        public void InputValueChange()
        {
            if (Input.value.Contains("\n"))
            {
                OnInputSubmit();
            }
            else
            {
                if (OnInputValueChange != null)
                    OnInputValueChange();
            }
        }

        public void OnInputSubmit()
        {
            var value = Input.value.ClearInput();

            if (HandleSystemVariables)
                value = value.HandleSystemVariables();

            if (!string.IsNullOrEmpty(value))
            {
                if (TreatInput)
                {
                    TreatInputText(value);
                    ClearInput();
                    AllCommandsTyped.Insert(0, value);
                }
                else if (ShowTextWhenNotTreatingInput)
                {
                    ShowText(value);
                    ClearInput();
                }
            }
            else
            {
                ShowText(string.Empty);
                ClearInput();
            }

            RepositionText();

            if (OnInputSubmited != null)
                OnInputSubmited(value);
        }

        private void TreatInputText(string text)
        {
            ShowText(text, showLocation: true);
            string programParams;
            Program program;
            var result = Alias.Programs.GetProgramAndParameters(text, out program, out programParams);

            if (result == ProgramCollection.ProgramRequestResult.NonExisting)
            {
                ShowText(TextBuilder.WarningText(string.Format("Unknow command \"{0}\"", text)));
                ShowText(TextBuilder.WarningText("Type \"help\" to get some help"));
                ShowText(TextBuilder.WarningText(string.Format("Use \"search\" to search for {0} in all files.", text)));
                ShowText(string.Empty);
                return;
            }

            if (result == ProgramCollection.ProgramRequestResult.NonGlobal)
            {
                ShowText(TextBuilder.WarningText(string.Format("Unknow command \"{0}\"", text)));
                ShowText(TextBuilder.WarningText("Type \"help\" to get some help"));
                ShowText(string.Empty);
                return;
            }

            var programInstance = RunProgram(program, programParams);

            if (programInstance == null)
                return;

            if (OnProgramExecuted != null)
                OnProgramExecuted(programInstance);

            programInstance.OnFinish += ProgramFinished;
        }

        public Program RunProgram(Program program, string param)
        {
            var programInstance = program.Clone();
            RunningPrograms.Add(programInstance);
            programInstance.Execute(param);
            return programInstance;
        }

        private void ClearInput()
        {
            Input.value = string.Empty;
            UpdateUserNameLocation();
            _currentNavigationCommandIndex = -1;
            Input.isSelected = true;
            RepositionText();
        }

        public void UpdateUserNameLocation()
        {
            LabelUserNameLocation.text = CurrentLocationAndUserName;
        }

        public void UpdateCommandLineToCommandIndex()
        {
            if (AllCommandsTyped.Count != 0)
                Input.value = AllCommandsTyped[_currentNavigationCommandIndex];
        }

        #endregion

        #region Interface

        #region Show Text

        #region Simple

        public void ShowText(string text, bool asNewLine = true, bool ident = false, bool showLocation = false)
        {
            RepositionText();

            TextEntry entry;
            if (asNewLine)
            {
                var newText = NGUITools.AddChild(TextTable.gameObject, TextEntryPrefab);
                newText.transform.SetAsFirstSibling();
                entry = newText.GetComponent<TextEntry>();

                if (showLocation)
                {
                    entry.Setup(CurrentLocationAndUserName, string.Empty, TextTable.transform);

                    newText = NGUITools.AddChild(TextTable.gameObject, TextEntryPrefab);
                    newText.transform.SetAsFirstSibling();
                    entry = newText.GetComponent<TextEntry>();
                }
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

            if (ident)
                BeginIdentation();

            entry.Setup(showLocation ? Alias.Config.CarrotChar : string.Empty, _identationBuilder + text, TextTable.transform);

            if (ident)
                EndIdentation();

            ValidaMaxEntries();

            RepositionText();
        }

        #endregion

        #region Type writer

        public Coroutine ShowTypeWriterTextWithCancel(string text, float intervalBetweenChars = .02f,
            bool startOnNewLine = false, Action callback = null)
        {
            return ShowTypeWriterText(text, intervalBetweenChars, startOnNewLine, callback, () =>
            {
                return UnityEngine.Input.GetKeyDown(KeyCode.C) && UnityEngine.Input.GetKey(KeyCode.LeftControl);
            });
        }

        public Coroutine ShowTypeWriterText(string text, float intervalBetweenChars = .02f, bool startOnNewLine = false, Action callback = null, Func<bool> cancellationToken = null)
        {
            StringBuilder textBuilder = new StringBuilder();

            // if we will NOT start this text on previous line
            // get the text from the previous line and add to the text to show
            // if we don't do this, the previous text will be cleaned 
            if (!startOnNewLine && TextTable.transform.childCount > 0)
            {
                var entry = TextTable.transform.GetChild(0).GetComponent<TextEntry>();
                textBuilder.Append(entry.Content.text);
            }

            return CoroutineHelper.Instance.WaitAndCallTimesControlled((index) =>
            {
                var currentChar = text[index];

                if (currentChar == '[')
                {
                    textBuilder.Append(currentChar);
                    var stepsToSkip = 0;
                    while (currentChar != ']' && index < text.Length)
                    {
                        index++;
                        stepsToSkip++;
                        currentChar = text[index];
                        textBuilder.Append(currentChar);
                    }
                    return stepsToSkip;
                }

                if (cancellationToken != null)
                {
                    if (cancellationToken())
                    {
                        var lastingText = text.Substring(index);
                        textBuilder.Append(lastingText);
                        ShowText(textBuilder.ToString(), false);
                        return text.Length - index;
                    }
                }


                textBuilder.Append(currentChar);
                ShowText(textBuilder.ToString(), startOnNewLine);
                startOnNewLine = false;

                // number of steps to skip
                return 0;
            }, text.Length, intervalBetweenChars, callback);
        }

        #endregion

        #region Timed text

        public Coroutine ShowTimedText(List<Tuple<string, float>> text, float intervalBetweenChars = .02f,
            bool startOnNewLine = false, Action callback = null)
        {
            return StartCoroutine(InnerShowTimedText(text, intervalBetweenChars, startOnNewLine, callback));
        }

        public IEnumerator InnerShowTimedText(List<Tuple<string, float>> text, float intervalBetweenChars = .02f,
            bool startOnNewLine = false, Action callback = null)
        {
            for (int i = 0; i < text.Count; i++)
            {
                yield return ShowTypeWriterTextWithCancel(text[i].Key);
                yield return new WaitForSeconds(text[i].Value);
            }

            if (callback != null)
                callback();
        }

        #endregion

        #endregion

        #region Clear

        public void ClearAll()
        {
            Clear(TextTable.transform.childCount);
        }

        public void Clear(int quantity)
        {
            if (quantity == 0)
                return;

            quantity = Mathf.Min(TextTable.transform.childCount, quantity);

            for (int i = TextTable.GetChildList().Count - 1; i >= 0 && quantity > 0; i--)
            {
                quantity--;

                if (TextTable.GetChildList()[i])
                {
                    Destroy(TextTable.transform.GetChild(i).gameObject);
                }
            }

            TextTable.Reposition();
        }

        #endregion

        #region Identation

        public void BeginIdentation(int quantity = 4)
        {
            for (int i = 0; i < quantity; i++)
                _identationBuilder.Append(" ");

            _lastIdentationQuantity = quantity;
        }

        public void EndIdentation()
        {
            if (_identationBuilder.Length > _lastIdentationQuantity)
                _identationBuilder.Remove(_identationBuilder.Length - (_lastIdentationQuantity + 1), _lastIdentationQuantity);
            else
                _identationBuilder.Remove(0, _identationBuilder.Length - 1);
        }

        #endregion

        #region Table/Scroll

        public void RepositionText()
        {
            TextTable.Reposition();
            TextScrollView.ResetPosition();
        }

        public void ValidaMaxEntries()
        {
            if (TextTable.transform.childCount > Alias.Config.MaxEntriesCount)
            {
                Clear(Alias.Config.EntriesCountToRemoveWhenMaxed);
            }
        }

        #endregion

        #endregion

        #region Callbacks

        private void ProgramFinished(Program program)
        {
            RunningPrograms.Remove(program);

            program.OnFinish -= ProgramFinished;

            if (OnProgramFinished != null)
                OnProgramFinished(program);

            ShowText("");
        }

        private void OnCurrentDirChanged()
        {
            UpdateUserNameLocation();
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

        public void OnSystemVariableChange(SystemVariableType variableType)
        {
            switch (variableType)
            {
                case SystemVariableType.USERNAME:
                    UpdateUserNameLocation();
                    break;
            }
        }

        public void OnCurrentDeviceChange()
        {
            UpdateUserNameLocation();
        }

        #endregion
    }
}