using UnityEngine;
using System.Collections;

namespace FH.UI.Camera
{
	public class UICameraEnhanced : UICamera
	{
		public override void ProcessTouch (bool pressed, bool released)
		{
			base.ProcessTouch (pressed, released);

			if (currentTouch.current != null && currentTouch.pressStarted)
			{
				if (currentTouch.longpressed == null)
				{
					currentTouch.longpressed = currentTouch.current;
					Notify (currentTouch.longpressed, "OnCustomLongPress", new bool[]{true, false});
				}
				else
				{
					if (currentTouch.longpressed != currentTouch.current)
					{
						Notify (currentTouch.longpressed, "OnCustomLongPress", new bool[]{false, false});
						currentTouch.longpressed = currentTouch.current;
						Notify (currentTouch.longpressed, "OnCustomLongPress", new bool[]{true, false});
					}
				}
			}
			if (released)
			{
				if (currentTouch.longpressed != null)
				{
					Notify (currentTouch.longpressed, "OnCustomLongPress", new bool[]{false, true});
					currentTouch.longpressed = null;
				}
			}
		}
	}
}