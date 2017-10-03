using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    static class StringHelper
    {
        public static string FormatTime(long timeMs)
        {
            return string.Format("{0:0.000}", timeMs / 1000.0);
        }

        public static string CycleDots(int maxDots = 3)
        {
            return new string('.', (int)(Time.realtimeSinceStartup % maxDots) + 1);
        }

        // Print out as much useful information as we can about a connection error.
        public static string SigninInFailureString(System.Threading.Tasks.Task connectionTask)
        {
            if (connectionTask.IsCanceled)
            {
                return StringConstants.SignInCanceled;
            }
            else if (connectionTask.IsFaulted)
            {
                return StringConstants.SignInFailed;
            }
            else
            {
                return StringConstants.SignInSuccessful;
            }
        }

    }


}
