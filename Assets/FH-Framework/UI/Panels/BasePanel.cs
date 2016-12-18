using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using FH.UI.Sound;

namespace FH.UI.Panels
{
	public class BasePanel : MonoBehaviour
	{
		public List<UISound> Sounds;

		public event Action<BasePanel> onOpenPanelStart;
		public event Action<BasePanel> onOpenPanelFinish;
		public event Action<BasePanel> onClosePanelStart;
		public event Action<BasePanel> onClosePanelFinish;

		private Action _completeTasksCallback;

		private int _runningTasks;
		public bool IsAnimating { get { return _runningTasks > 0; } }

		#region Open Panel Handling

		public void OpenPanel()
		{
			StartOpeningPanel ();
		}

		protected virtual void StartOpeningPanel()
		{
			_completeTasksCallback = FinishOpeningPanel;
			StartTask();

			if (onOpenPanelStart != null)
				onOpenPanelStart (this);

			if (Sounds != null && Sounds.Count > 0)
			{
				for (byte i = 0; i < Sounds.Count; i++)
				{
					if (Sounds [i].OnStart)
					{
						StartTask ();
						StartCoroutine (PlaySound (Sounds [i]));
					}
				}
			}

			StartCoroutine (FinishInitialTask ());
		}

		protected virtual void FinishOpeningPanel()
		{
			if (onOpenPanelFinish != null)
				onOpenPanelFinish (this);
		}

		#endregion

		#region Close Panel Handling

		public void ClosePanel()
		{
			StartClosingPanel ();
		}

		protected virtual void StartClosingPanel()
		{
			_completeTasksCallback = FinishClosingPanel;
			StartTask();

			if (onClosePanelStart != null)
				onClosePanelStart (this);

			if (Sounds != null && Sounds.Count > 0)
			{
				for (byte i = 0; i < Sounds.Count; i++)
				{
					if (!Sounds [i].OnStart)
					{
						StartTask ();
						StartCoroutine (PlaySound (Sounds [i]));
					}
				}
			}

			StartCoroutine (FinishInitialTask ());
		}

		protected virtual void FinishClosingPanel()
		{
			if (onClosePanelFinish != null)
				onClosePanelFinish (this);
		}

		#endregion

		#region Helper Methods

		public void StartTask()
		{
			_runningTasks++;

			#if Debugging
			Debug.Log("Starting task in base panel " + name + " to a total of +1: " + _runningTasks.ToString());
			#endif
		}

		public void CompleteTask()
		{
            _runningTasks--;

			#if Debugging
			Debug.Log("Ending task in base panel " + name + " to a total of -1: " + _runningTasks.ToString());
			#endif

			if (_runningTasks == 0)
				_completeTasksCallback();
        }

		private IEnumerator FinishInitialTask()
		{
			yield return null;
			CompleteTask ();
		}

		private IEnumerator PlaySound(UISound sound)
		{
			yield return new WaitForSeconds(sound.Delay);
			MasterAudio.PlaySoundAndForget (sound.SoundGroupName);
			CompleteTask ();
		}

		#endregion
	}
}