using Azure.AI.OpenAI;
using Infoscreens.Common.Helpers;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Linq;

namespace Infoscreens.Common.Repositories
{
    public class AiChatBotRepository
    {
        private const string GPT_4_O_MINI_STRING = "gpt-4o-mini";

        private static ChatClient _chatClient;

        private static ChatClient GetChatBotClient()
        {
            if(_chatClient == null)
            {
                var azureOpenAiClient = new AzureOpenAIClient(
                new Uri(CommonConfigHelper.AzureOpenAiEndpoint),
                new ApiKeyCredential(CommonConfigHelper.AzureOpenAiKey));

                // Get an Azure Open AI chat client
                _chatClient = azureOpenAiClient.GetChatClient(GPT_4_O_MINI_STRING);
            }
            
            return _chatClient;
        }

        public static string GetAiChatBotResponse(string prompt)
        {
            var chatClient = GetChatBotClient();

            // Send prompt to Azure Open AI chat bot
            var chatCompletion = chatClient.CompleteChat(new UserChatMessage(prompt));

            // Extract text from bot response
            var chatMessageContent = chatCompletion.Value.Content.FirstOrDefault();
            var content = chatMessageContent?.Text ?? string.Empty;

            return content;
        }
    }
}
