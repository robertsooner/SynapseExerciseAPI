using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using SynapseExerciseAPI.Models;
using System.ClientModel;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SynapseExerciseAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OpenAI : ControllerBase
    {
        IConfiguration _configuration;
        ChatClient _openAIClient;
        public OpenAI(IConfiguration configuration) { 
            _configuration = configuration;
            _openAIClient = new ChatClient("gpt-4.1", _configuration.GetValue<string>("AppSettings:OpenAPI_Key"));
        }


        // POST api/<OpenAI>
        [HttpPost("Tuner/ClinicalNotes")]
        [EnableCors("AllowAll")]
        public async Task<ActionResult> Post([FromBody] AIRequest request)
        {
            const int maxRetries = 3;
            int retryCount = 0;
            int delayMs = 1000;

            if (request == null)
            {
                return BadRequest("AI Request cannot be null.");
            } else {
                if (string.IsNullOrWhiteSpace(request.SystemMessage) || string.IsNullOrWhiteSpace(request.UserMessage) || string.IsNullOrWhiteSpace(request.SamplePrompt))
                {
                    return BadRequest("System Message, User Message, and Sample Prompt cannot be null or empty.");
                }
                if (request.Temperature < 0 || request.Temperature > 1)
                {
                    return BadRequest("Temperature must be between 0 and 1.");
                }
                if (request.Topp < 0 || request.Topp > 1)
                {
                    return BadRequest("Topp must be between 0 and 1.");
                }
                if (request.MaxTokens <= 0)
                {
                    return BadRequest("Max Tokens must be greater than zero.");
                }
            }

            while (true)
            {
                try
                {
                    List<ChatMessage> messages = new List<ChatMessage>();
                    messages.Add(new SystemChatMessage(request.SystemMessage));
                    messages.Add(new UserChatMessage(request.UserMessage));
                    messages.Add(new UserChatMessage(request.SamplePrompt));

                    ChatCompletionOptions options = new ChatCompletionOptions();
                    options.Temperature = request.Temperature;
                    options.TopP = request.Topp;
                    options.MaxOutputTokenCount = request.MaxTokens;
                    options.ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat();

                    var stopwatch = Stopwatch.StartNew();
                    ClientResult<ChatCompletion> chatCompletionResult = await _openAIClient.CompleteChatAsync(messages, options);
                    stopwatch.Stop();

                    if (chatCompletionResult.Value != null)
                    {
                        AIResponse response = new AIResponse();
                        response.Response = chatCompletionResult.Value.Content[0].Text;
                        response.ElapsedTime = $"{stopwatch.Elapsed.TotalSeconds:F3}";
                        response.PromptTokens = chatCompletionResult.Value.Usage.InputTokenCount;
                        response.CompletionTokens = chatCompletionResult.Value.Usage.OutputTokenCount;
                        response.TotalTokens = chatCompletionResult.Value.Usage.TotalTokenCount;

                        return Ok(response);
                    }
                    else
                    {
                        return StatusCode(500, "OpenAI API call failed.");
                    }
                }

                catch (Exception ex) when (retryCount < maxRetries)
                {
                    retryCount++;
                    await Task.Delay(delayMs * retryCount); // Exponential backoff
                }
                catch (Exception)
                {
                    return StatusCode(500);
                    throw;
                }
            }
        }

    }
}
