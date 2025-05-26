namespace SynapseExerciseAPI.Models
{
    public class AIRequest
    {
        public string SystemMessage {  get; set; }
        public string UserMessage { get; set; }
        public string SamplePrompt { get; set; }
        public float Temperature { get; set; }
        public float Topp { get; set; }
        public short MaxTokens { get; set; }
    }
}
