using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using System;
using FH.UI.Panels;

namespace FH.UI.Popups
{
	public abstract class PopupHandlerBase : MonoBehaviour
	{
		protected Stack<GenericConfirmationPopup> _openPopups = new Stack<GenericConfirmationPopup> ();
		protected List<GenericConfirmationPopup> _popupsToClose = new List<GenericConfirmationPopup>();

		public event Action<GenericConfirmationPopup> OnPopupOpened;
		public event Action<GenericConfirmationPopup, GenericConfirmationPopup> OnPopupClosed;

		public GenericConfirmationPopup CurrentPopup
		{
			get
			{
				if (_openPopups.Count > 0)
					return _openPopups.Peek ();
				return null;
			}
		}

		#region Popup Pool

		protected void AddPopup(GameObject prefab, List<GenericConfirmationPopup> popupList)
		{
			var go = NGUITools.AddChild(gameObject, prefab);
			popupList.Add(go.GetComponent<GenericConfirmationPopup>());
			go.SetActive(false);
		}

		#endregion

		#region Popup Handling

		protected GenericConfirmationPopup InnerOpenPopup (Action<bool, GenericConfirmationPopup> callback, List<GenericConfirmationPopup> popups, Action addPopupCallback, string soundGroup = null)
		{
			if (_openPopups.Count > 0 && _openPopups.Peek().IsAnimating)
				return null;

			var hasAvailablePopup = false;
			for (var i = 0; i < popups.Count; i++)
			{
				if (popups[i].gameObject.activeSelf)
					continue;

				_openPopups.Push(popups[i]);
				hasAvailablePopup = true;
			}

			if (!hasAvailablePopup)
			{
				addPopupCallback ();
				_openPopups.Push(popups[popups.Count - 1]);
			}

			var currentPopup = _openPopups.Peek ();
			currentPopup.PopupResultCallback = callback;
            
			currentPopup.UpdatePanelDepths (4000 + (_openPopups.Count * 100));

			currentPopup.gameObject.SetActive (true);
			currentPopup.OpenPanel ();

			if (!string.IsNullOrEmpty (soundGroup))
				MasterAudio.PlaySoundAndForget (soundGroup);

			if (OnPopupOpened != null)
				OnPopupOpened (currentPopup);

			return currentPopup;
		}

		public void ClosePopup(GenericConfirmationPopup popupToClose)
		{
			if (_openPopups.Count <= 0)
				return;

			if (popupToClose != null && _openPopups.Peek () != popupToClose)
			{
				_popupsToClose.Add (popupToClose);
				return;
			}

			var currentPopup = _openPopups.Pop ();
			if (currentPopup.IsAnimating)
				return;

			currentPopup.ClosePanel ();
			currentPopup.onClosePanelFinish += FinishClosingPopup;

			if (OnPopupClosed != null)
				OnPopupClosed (currentPopup, _openPopups.Count > 0 ? _openPopups.Peek () : null);

			if (_openPopups.Count <= 0)
				return;

			if (_popupsToClose.Count <= 0)
				return;

			var popupIndex = _popupsToClose.IndexOf (_openPopups.Peek ());
			if (popupIndex == -1)
				return;

			ClosePopup (_popupsToClose [popupIndex]);
		}

		private void FinishClosingPopup(BasePanel basePanel)
		{
			basePanel.onClosePanelFinish -= FinishClosingPopup;
            basePanel.gameObject.SetActive (false);
		}

        #endregion
    }
}