using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
POST  /signup?fname=limor&lname=lahiani&phone=&email=limorl@Microsoft.com&uname=limonit
&token=xyz&clusterId=123

POST /signin?uname=limonit

POST /signout?uname=limonit

GET /clusters

GET /disasters?lat=73.15141&lon=45.9393939393

POST /volunteer?uname=limonit&did=4353535&sDate=848484&eDate=949494

POST /checkin?uname=limonit&did=55353&lat=56.777777&lon=75.95959595  

POST /checkout?uname=limonit
 */

namespace CrisisCheckinApp.ServiceClient
{
    /// <summary>
    /// Mock implementation 
    /// </summary>
    public class CrisisCheckinServiceClient : ICrisisCheckinService
    {
        public void SignupAsync(UserProfile profile, Action<ServiceResponseStatus> callback = null)
        {
            if (callback != null)
            {
                var status = new ServiceResponseStatus();

                callback(status);
            }
        }

        public void SigninAsync(UserCredentials credentials, Action<ServiceResponseStatus, UserProfile> callback = null)
        {
            if (callback != null)
            {
                var status = new ServiceResponseStatus();
                var profile = new UserProfile
                {
                    FirstName = "Dan",
                    LastName = "Holmes",
                    UserName = "dano",
                    Email = "dan.homes@outlook.com",
                    MobilePhone = "+1425987778"
                };

                callback(status, profile);
            }
        }

        public void SignoutAsync(UserCredentials credentials, Action<ServiceResponseStatus> callback = null)
        {
            if (callback != null)
            {
                var status = new ServiceResponseStatus();

                callback(status);
            }
        }

        public void GetDisastersAsync(Location location, Action<ServiceResponseStatus, IEnumerable<Disaster>> callback = null)
        {
            if (callback != null)
            {
                var status = new ServiceResponseStatus();
                var disasters = new List<Disaster>{
                    new Disaster{ Name = "Tzunami in Thailand", Id = Guid.NewGuid().ToString(), Location = new Location { Latitude = 73.6262626, Longitude = 64.4449949 }, IsActive = true },
                    new Disaster{ Name = "Hurricane in Miami", Id = Guid.NewGuid().ToString(), Location = new Location { Latitude = 73.6262626, Longitude = 76.696969 }, IsActive = true },
                    new Disaster{ Name = "Tornado in Cuba", Id = Guid.NewGuid().ToString(), Location = new Location { Latitude = 73.6262626, Longitude = 32.4449949 }, IsActive = true }    
                };

                callback(status, disasters);
            }
        }

        public void GetClustersAsync(Action<ServiceResponseStatus, IEnumerable<string>> callback = null)
        {
            if (callback != null)
            {
                var status = new ServiceResponseStatus();
                var clusters = new List<string>{ 
                    "Agriculture", 
                    "Camp Coordination and Management", 
                    "Early Recovery",
                    "Emergency Shelter",
                    "Food",
                    "Health",
                    "Logistics",
                    "Nutrition",
                    "Protection",
                    "Water and Sanitation"
                };
        
                callback(status, clusters);
            }
        }

        public void Volunteer(UserCredentials credentials, Action<ServiceResponseStatus> callback = null)
        {
            if (callback != null)
            {
                var status = new ServiceResponseStatus();

                callback(status);
            }
        }

        public void Checkin(UserCredentials credentials, Location location, Action<ServiceResponseStatus> callback = null)
        {
            if (callback != null)
            {
                var status = new ServiceResponseStatus();
                
                callback(status);
            }
        }

        public void CheckinHistory(UserCredentials credentials, DateTime beforeDate, Action<ServiceResponseStatus, IEnumerable<Checkin>> callback = null)
        {
            if (callback != null)
            {
                var status = new ServiceResponseStatus();
                var checkins = new List<Checkin>{
                    new Checkin{ DisasterName = "Tzunami in Thailan", DisasterId = Guid.NewGuid().ToString(), Id = Guid.NewGuid().ToString(), Location = new Location { Latitude = 73.6262626, Longitude = 64.4449949 }, Date = new DateTime(2010, 10, 25) },
                    new Checkin{ DisasterName = "Hurican in Miami", DisasterId = Guid.NewGuid().ToString(), Id = Guid.NewGuid().ToString(), Location = new Location { Latitude = 73.6262626, Longitude = 64.4449949 }, Date = new DateTime(2013, 9, 3) },
                    new Checkin{ DisasterName = "Tornado in Cuba", DisasterId = Guid.NewGuid().ToString(), Id = Guid.NewGuid().ToString(), Location = new Location { Latitude = 73.6262626, Longitude = 64.4449949 }, Date = new DateTime(2008, 11, 12) },
                };

                callback(status, checkins.OrderByDescending(c => c.Date));
            }
        }

    }
}


