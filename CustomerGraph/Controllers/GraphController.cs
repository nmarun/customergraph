using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using GraphQL.Server.Transports.AspNetCore.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;

namespace CustomerGraph.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GraphController : Controller
    {
        private readonly IDocumentExecuter _documentExecuter;
        private readonly ISchema _schema;
        private readonly ILogger<GraphController> _logger;

        public GraphController(ISchema schema, IDocumentExecuter documentExecuter, ILogger<GraphController> logger)
        {
            _schema = schema;
            _documentExecuter = documentExecuter;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLRequest query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var inputs = query.Variables.ToInputs();
            var executionOptions = new ExecutionOptions
            {
                Schema = _schema,
                Query = query.Query,
                Inputs = inputs,
                ThrowOnUnhandledException = true
            };

            ExecutionResult result = await _documentExecuter.ExecuteAsync(executionOptions);
            _logger.LogInformation(JsonConvert.SerializeObject(result.Perf));
            if(result.Errors != null && result.Errors.Any())
            {
                _logger.LogInformation(JsonConvert.SerializeObject(result.Errors));
            }
            _logger.LogInformation(JsonConvert.SerializeObject(result));

            if (result.Errors?.Count > 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

    }
}
