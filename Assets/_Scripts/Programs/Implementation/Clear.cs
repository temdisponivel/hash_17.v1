using System.Collections;
using Hash17.Utils;

namespace Hash17.Programs.Implementation
{
    public class Clear : Program
    {
        protected override IEnumerator InnerExecute()
        {
            if (HelpOrUnknownParameters(true))
            {
                yield break;
            }

            Alias.Term.ClearAll();
            yield break;
        }
    }
}