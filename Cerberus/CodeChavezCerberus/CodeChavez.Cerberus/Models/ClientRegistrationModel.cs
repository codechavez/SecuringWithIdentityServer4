using IdentityModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeChavez.Cerberus.Models
{
    public class ClientRegistrationModel
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public List<string> Scopes { get; set; } = new List<string>();
    }
}
