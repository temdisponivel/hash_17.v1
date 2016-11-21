using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hash17.Programs.Implementation
{
    public class Dir : Program
    {
        protected override IEnumerator InnerExecute()
        {
            yield return null;

            if (HelpOrUnknownParameters(true))
                yield break;


        }
    }
}
