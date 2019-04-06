using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TejiAdesa.MainLibs
{
    class Util
    {
        public static String GetFormattedExceptionInfo<T>(ref T myEx) where T : Exception
        {
            StringBuilder strError = new StringBuilder();
            try
            {
                if (myEx != null)
                {
                    strError.AppendLine("Exception data:");
                    strError.AppendLine(myEx.Message != null ? myEx.Message : "Nessun messaggio di errore da visualizzare");
                    strError.AppendLine(myEx.StackTrace != null ? myEx.StackTrace : "Stack trace assente");
                    GetInnerExInfoRec<T>(myEx, ref strError);
                    strError.AppendLine("Nome del Server: " + Environment.MachineName);
                    return strError.ToString();
                }
                else return "Nessun dato da visualizzare sull'errore";
            }
            catch (Exception ex)
            {
                throw ex;

            }

        }

        public static String GetFormattedExceptionInfo<T>(T myEx) where T : Exception
        {
            StringBuilder strError = new StringBuilder();
            try
            {
                if (myEx != null)
                {
                    strError.AppendLine("Exception data:");
                    strError.AppendLine(myEx.Message != null ? myEx.Message : "Nessun messaggio di errore da visualizzare");
                    strError.AppendLine(myEx.StackTrace != null ? myEx.StackTrace : "Stack trace assente");
                    GetInnerExInfoRec<T>(myEx, ref strError);
                    strError.AppendLine("Nome del Server: " + Environment.MachineName);
                    return strError.ToString();
                }
                else return "Nessun dato da visualizzare sull'errore";
            }
            catch (Exception ex)
            {
                throw ex;

            }

        }

        private static void GetInnerExInfoRec<T>(T ex, ref StringBuilder info) where T : Exception
        {
            if (ex.InnerException == null) return;
            else
            {
                info.AppendLine("Inner Exception data:");
                info.AppendLine(ex.InnerException.Message != null ? ex.InnerException.Message : "nessun inner exception message presente");
                info.AppendLine(ex.InnerException.StackTrace != null ? ex.InnerException.StackTrace : " Nessun inner stack trace presente");
                GetInnerExInfoRec(ex.InnerException, ref info);
            }

        }
    }
}
