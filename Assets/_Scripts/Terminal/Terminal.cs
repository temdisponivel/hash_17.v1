using System;
using UnityEngine;
using System.Text;
using Hash17.Programs;
using Hash17.Utils;

namespace Hash17.Terminal_
{
    public class Terminal : PersistentSingleton<Terminal>
    {
        #region Properties

        public UIPanel RootPanel;
        public GameObject TextEntryPrefab;
        public UITable TextTable;
        public UIInput Input;
        public UILabel LabelUserNameLocation;

        private readonly System.Collections.Generic.List<Program> _runningPrograms = new System.Collections.Generic.List<Program>();
        public System.Collections.Generic.List<Program> RunningPrograms
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
                Input.enabled = !_blockInput;
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

        #endregion

        #region User name

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
            get { return string.Format("{0}:{1}>", Alias.Board.CurrentDevice.UniqueId, Alias.Board.FileSystem.CurrentDirectory.Path); }
        }

        #endregion

        #region Commands

        public readonly System.Collections.Generic.List<string> AllCommandsTyped = new System.Collections.Generic.List<string>();
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
            ClearInput();
            Alias.Board.FileSystem.OnChangeCurrentDirectory += OnCurrentDirChanged;
            RunProgram(Alias.Board.SpecialPrograms[ProgramId.Init], string.Empty);
            CurrentUserName = "temdisponivel";
            Input.label.SetupWithHash17Settings();
            LabelUserNameLocation.SetupWithHash17Settings();
        }

        #endregion

        #region ProgramsByCommand

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
            else if (ShowTextWhenNotTreatingInput)
            {
                DontTreatInputText(Input.value);
                ClearInput();
            }

            if (OnInputSubmited != null)
                OnInputSubmited(value);
        }

        private void DontTreatInputText(string text)
        {
            text = text.Replace("\n", string.Empty);
            ShowText(text);
        }

        private void TreatInputText(string text)
        {
            text = text.Replace("\n", string.Empty);
            ShowText(text, showLocation: true);

            string programName, programParams;
            Interpreter.GetProgram(text, out programName, out programParams);
            Program program;
            if (!Alias.Board.ProgramsByCommand.TryGetValue(programName, out program))
            {
                ShowText(TextBuilder.WarningText(string.Format("Unknow command \"{0}\"", text)));
                ShowText(TextBuilder.WarningText("Type \"help\" to get some help"));
                ShowText(TextBuilder.WarningText(string.Format("Use 'search -all {0}' to search for {0} in all files.", text)));
                return;
            }

            var device = Alias.Board.CurrentDevice;
            var deviceProgramId = 0;
            if (device.SpecialPrograms.TryGetValue(program.Id, out deviceProgramId))
            {
                var progBkp = program;
                if (!Alias.Board.ProgramDefinitionByUniqueId.TryGetValue(deviceProgramId, out program))
                {
                    program = progBkp;

                    // if the program is not global and current device don't have it, error
                    if (!program.Global)
                    {
                        ShowText(TextBuilder.WarningText(string.Format("Unknow command \"{0}\"", text)));
                        ShowText(TextBuilder.WarningText("Type \"help\" to get some help"));
                        return;
                    }
                }
            }
            
            var programInstance = RunProgram(program, programParams);

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

        public void ShowText(string text, bool asNewLine = true, bool ident = false, bool showLocation = false)
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

            if (ident)
                BeginIdentation();

            entry.Setup(showLocation ? CurrentLocationAndUserName : "", _identationBuilder + text, TextTable.transform);

            if (ident)
                EndIdentation();

            CoroutineHelper.Instance.WaitAndCall(() =>
            {
                TextTable.Reposition();
            }, 0.1f);

            //TextTable.Reposition();
        }

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

        #endregion
    }
}