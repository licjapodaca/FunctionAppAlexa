using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Alexa.NET;

namespace FunctionAppAlexa
{
    public static class Function1
    {
        [FunctionName("Alexa")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
			SkillResponse response = null;

			try
			{
				log.LogInformation("Iniciando Azure Function de Alexa...");

				string json = await req.ReadAsStringAsync();

				log.LogInformation("JSON: {0}", json);

				var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(json);
				var requestType = skillRequest.GetRequestType();


				if (requestType == typeof(LaunchRequest))
				{
					response = ResponseBuilder.Tell("Buen dia licenciado Apodaca, bienvenido a la gestion de expedientes Temis!");
					response.Response.ShouldEndSession = false;
				}
				else if(requestType == typeof(IntentRequest))
				{
					var intentRequest = skillRequest.Request as IntentRequest;

					if(intentRequest.Intent.Name == "Solicitudes")
					{
						var random = new Random();
						int num = random.Next(23);
						if(num == 1)
							response = ResponseBuilder.Tell($"Usted tiene {num} solicitud asignada.");
						else
							response = ResponseBuilder.Tell($"Usted tiene {num} solicitudes asignadas.");
					}
				}
				else if(requestType == typeof(SessionEndedRequest))
				{
					response = ResponseBuilder.Tell("Gracias por utilizar los servicios de gestion de expedientes Temis licenciado Apodaca.");
				}

				log.LogInformation("Fin de la logica de negocio de Alexa...");
			}
			catch(Exception ex)
			{
				log.LogError(ex, "Error en la Azure Function");
				throw ex;
			}

			return new OkObjectResult(response);
        }
    }
}
