using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CrisisCheckinApp.ServiceClient
{
    public interface ICrisisCheckinService
    {
        void SignupAsync(UserProfile profile, Action<ServiceResponseStatus> callback = null);

        void SigninAsync(UserCredentials credentials, Action<ServiceResponseStatus, UserProfile> callback = null);

        void SignoutAsync(UserCredentials credentials, Action<ServiceResponseStatus> callback = null);

        void GetDisastersAsync(Location location, Action<ServiceResponseStatus, IEnumerable<Disaster>> callback = null);

        void GetClustersAsync(Action<ServiceResponseStatus, IEnumerable<string>> callback = null);

        void Volunteer(UserCredentials credentials, Action<ServiceResponseStatus> callback = null);

        void Checkin(UserCredentials credentials, Location location, Action<ServiceResponseStatus> callback = null);

        void CheckinHistory(UserCredentials credentials, DateTime beforeDate, Action<ServiceResponseStatus, IEnumerable<Checkin>> callback = null);
    }

    #region API Data Model

    /// <summary>
    /// Represents user credentials.
    /// </summary>
    [DataContract]
    public class UserCredentials
    {
        /// <summary>
        /// User Name
        /// </summary>
        [DataMember(IsRequired = true)]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user authentication type.
        /// Should be 'liveid' or 'facebook';
        /// </summary>
        [DataMember(IsRequired = true)]
        public string AuthType { get; set; }

        /// <summary>
        /// Gets or sets the token to validate user authentication. 
        /// </summary>
        [DataMember(IsRequired = true)]
        public string AuthToken { get; set; }
    }

    [DataContract]
    public class UserProfile
    {
        [DataMember(IsRequired = true)]
        public string FirstName { get; set; }

        [DataMember(IsRequired = true)]
        public string LastName { get; set; }

        [DataMember(IsRequired = true)]
        public string UserName { get; set; }

        [DataMember(IsRequired = true)]
        public string Email { get; set; }

        [DataMember(IsRequired = true)]
        public string MobilePhone { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string OtherPhone { get; set; }

        [DataMember(IsRequired = true)]
        public string Cluster { get; set; }
    }

    [DataContract]
    public class ServiceResponseStatus
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ServiceResponseError Error { get; private set; }

        [DataMember(IsRequired = true)]
        public bool IsSuccess { get; private set; }

        public ServiceResponseStatus()
            : this(null)
        {
        }

        public ServiceResponseStatus(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                IsSuccess = true;
            }
            else
            {
                IsSuccess = false;
                Error = new ServiceResponseError(error);
            }
        }
    }

    [DataContract]
    public class ServiceResponseError
    {
        public ServiceResponseError(string error)
        {
            Error = error;
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Error { get; private set; }
    }

    [DataContract]
    public class Location
    {
        [DataMember(IsRequired = true)]
        public double Latitude { get; set; }

        [DataMember(IsRequired = true)]
        public double Longitude { get; set; }
    }

    [DataContract]
    public class Disaster
    {
        [DataMember(IsRequired = true)]
        public string Id { get; set; }

        [DataMember(IsRequired = true)]
        public string Name { get; set; }

        [DataMember(IsRequired = true)]
        public bool IsActive { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Location Location { get; set; }
    }

    [DataContract]
    public class Checkin
    {
        [DataMember(IsRequired = true)]
        public string Id { get; set; }

        [DataMember(IsRequired = true)]
        public string DisasterId { get; set; }

        [DataMember(IsRequired = true)]
        public string DisasterName { get; set; }

        [DataMember(IsRequired = true)]
        public Location Location { get; set; }

        [DataMember(IsRequired = true)]
        public DateTime Date { get; set; }
    }

    #endregion 
}
