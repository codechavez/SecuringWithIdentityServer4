using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CodeChavez.Cerberus.Models;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static IdentityModel.OidcConstants;

namespace CodeChavez.Cerberus.Controllers
{
    [Route("connect/register")]
    [ApiController, Consumes("application/json"), Produces("application/json")]
    public class ClientController : ControllerBase
    {
        private readonly ConfigurationDbContext _ConfigContext;

        public ClientController(ConfigurationDbContext context)
        {
            _ConfigContext = context;
        }

        [HttpGet("")]
        public IActionResult ClientRegisterAsync()
        {
            return new OkObjectResult(new ClientRegistrationModel());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ClientRegisterAsync([FromBody] ClientRegistrationModel clientRegistration)
        {
            // TODO: Model Validation and Security checks

            // Create response
            var response = (ClientRegistrationResponse)clientRegistration;
            response.GenerateSecret();

            // Initialize Client
            var client = new Client
            {
                ClientId = response.ClientId,
                ClientName = clientRegistration.ClientName,
                AbsoluteRefreshTokenLifetime = 3600, // 1hr 
                RefreshTokenUsage = (int)IdentityServer4.Models.TokenUsage.ReUse,
                RefreshTokenExpiration = (int)IdentityServer4.Models.TokenExpiration.Absolute,
                ClientSecrets = new List<ClientSecret>(),
                AllowedGrantTypes = new List<ClientGrantType>(),
                AllowedScopes = new List<ClientScope>(),
            };

            // Adding Grand Type
            client.AllowedGrantTypes.Add(new ClientGrantType { Client = client, GrantType = GrantTypes.ClientCredentials });
            // Adding Client Secret
            client.ClientSecrets.Add(new ClientSecret { Client = client, Value = response.ClientSecret.ToSha256() });
            // Adding Client Scopes
            clientRegistration.Scopes.ForEach(scope =>
            {
                client.AllowedScopes.Add(new ClientScope { Client = client, Scope = scope });
            });

            // Adding to database
            _ConfigContext.Clients.Add(client);
            await _ConfigContext.SaveChangesAsync();

            return Accepted(response);
        }
    }
}
