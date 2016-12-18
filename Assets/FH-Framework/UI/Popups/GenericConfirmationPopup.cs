using UnityEngine;
using System.Collections;
using System;
using FH.UI.Panels;

namespace FH.UI.Popups
{
	public class GenericConfirmationPopup : BasePanel
	{
		public UIPanel PopupPanel;
		public UIPanel[] InnerPanels;
		public Action<bool, GenericConfirmationPopup> PopupResultCallback;

		#region Panel Events

		protected override void StartClosingPanel ()
		{
			base.StartClosingPanel ();

			PopupResultCallback = null;
		}

		#endregion

		#region Buttons

		public virtual void Cancel()
		{
			if (IsAnimating)
				return;

			if (PopupResultCallback != null)
				PopupResultCallback (false, this);
		}

		public virtual void Confirm()
		{
			if (IsAnimating)
				return;

			if (PopupResultCallback != null)
				PopupResultCallback (true, this);
		}
		
		#endregion
	
		#region Helper Methods

		public void UpdatePanelDepths(int startingRenderQueue)
		{
			for (var i = 0; i < InnerPanels.Length; i++)
			{
				InnerPanels [i].startingRenderQueue = startingRenderQueue + (InnerPanels [i].startingRenderQueue - PopupPanel.startingRenderQueue);
				InnerPanels [i].depth = startingRenderQueue + (InnerPanels [i].depth - PopupPanel.depth);
			}

			PopupPanel.startingRenderQueue = startingRenderQueue;
			PopupPanel.depth = startingRenderQueue;
		}

		#endregion
	}
}