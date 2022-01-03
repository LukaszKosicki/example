using api.Models.Entities.Auth;
using SparkPost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Services
{
    public static class SparkPostService
    {
        public static void SendEmail(User userIdentity, string token, string template)
        {
            var transmission = new Transmission();
            transmission.Content.TemplateId = template;
            transmission.SubstitutionData.Add("username", userIdentity.UserName);
            transmission.SubstitutionData.Add("token", token);
            transmission.SubstitutionData.Add("email", userIdentity.Email);

            var recipient = new Recipient
            {
                Address = new Address { Email = userIdentity.Email }
            };
            transmission.Recipients.Add(recipient);
          
            var client = new Client("");
           
            client.CustomSettings.SendingMode = SendingModes.Sync;
            var response = client.Transmissions.Send(transmission);

        }
    }
}
