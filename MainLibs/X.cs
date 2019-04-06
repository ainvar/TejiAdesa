using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TejiAdesa.MainLibs
{
    public static class OnObj
    {
        public static String ToSafeString(this Object obj)
        {
            try
            {
                if (obj == null) return ""; else return obj.ToString();
            }
            catch { return ""; }
        }
    }
}
