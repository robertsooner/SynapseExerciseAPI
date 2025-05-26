namespace SynapseExerciseAPI.Models
{
    public class AIResponse
    {
        public string Response { get; set; }
        public string ElapsedTime { get; set; }
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
    }
}
