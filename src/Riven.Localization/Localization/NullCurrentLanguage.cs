using System;
using System.Collections.Generic;
using System.Text;

namespace Riven.Localization
{
    public class NullCurrentLanguage : ICurrentLanguage
    {
        public string Culture { get; private set; }

        public NullCurrentLanguage()
        {
            Culture = string.Empty;
        }
    }
}
