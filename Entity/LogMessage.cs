namespace Pipeline.App.Entity
{
    public class LogMessage
    {
        public string PipelineId { get; set; }

        public string Id { get; set; }

        public int Encoding { get; set; }

        public string Body { get; set; }

        public string NextId { get; set; }
    }
}
