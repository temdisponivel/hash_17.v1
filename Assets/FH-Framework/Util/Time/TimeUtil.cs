using UnityEngine;
using System.Collections;

namespace FH.Util.Time
{
	public class TimeUtil
	{
		public static IEnumerator WaitForRealSeconds(float waitTime)
		{
			var start = UnityEngine.Time.realtimeSinceStartup;
			while (UnityEngine.Time.realtimeSinceStartup < start + waitTime)
				yield return null;
		}
	}
}