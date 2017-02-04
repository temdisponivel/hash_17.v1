using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;
using System;

namespace FH.Util.Extensions
{
	public static class ExtensionMethods
	{
		public static Transform Reset(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			return transform;
		}

	    public static string InLineFormat(this string format, params object[] args)
	    {
	        return string.Format(format, args);
	    }
	}
}