using IdentityModel;
using Newtonsoft.Json;
using System;

namespace CodeChavez.Cerberus.Models
{
    public class ClientRegistrationResponse
    {
        [JsonProperty(OidcConstants.RegistrationResponse.ClientId)]
        public string ClientId { get; set; }

        [JsonProperty(OidcConstants.RegistrationResponse.ClientSecret)]
        public string ClientSecret { get; set; }

        [JsonProperty(OidcConstants.ClientMetadata.ClientName)]
        public string ClientName { get; set; }

        public static explicit operator ClientRegistrationResponse(ClientRegistrationModel model)
        {
            return new ClientRegistrationResponse
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = model.ClientName,
            };
        }

        internal void GenerateSecret()
        {
            // TODO: Define secret generation

            var temp = Guid.NewGuid().ToString().Split('-');
            var secret = temp[temp.Length - 1];


            ClientSecret = secret;
        }
    }
}
