using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisisCheckinApp
{
    /// <summary>
    /// Project wide consts
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Background agent name
        /// </summary>
        public const string BackgroungAgentName = "BackgroundAgent";

        /// <summary>
        /// Background agent activation period
        /// </summary>
        public const int BackgroungActivationPeriod = 14;

        /// <summary>
        /// Timeout mutex length (millisecs)
        /// </summary>
        public const int MutexWaitTimeout = 5000;

        /// <summary>
        /// Mutext name for isolated storage settings
        /// </summary>
        public const string IsoSettingsMutexName = "CrisisCheckinIsolatedStorageSettingsMutex";

        /// <summary>
        /// Max wait-time till location signal is received from GPS
        /// </summary>
        public const int LocationSignalReceivedWaitTimeout = 5000;

        /// <summary>
        /// About us - web site
        /// </summary>
        public static readonly string AboutUsURL = "http://crisischeckin.azurewebsites.net/";

        public const string UserProfileKey = "UserProfile";
        public const string UserCredentialsKey = "UserCredentials";
        public const string DisastersKey = "Disasters";
        public const string IsSignedInKey = "IsSignedIn";
    }
}
