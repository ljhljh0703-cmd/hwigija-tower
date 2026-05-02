namespace HwigiTower.Run
{
    public readonly struct PrototypeNodeResolution
    {
        public PrototypeNodeResolution(string nodeId, string payloadId, string message, bool runCompleted)
        {
            NodeId = nodeId ?? string.Empty;
            PayloadId = payloadId ?? string.Empty;
            Message = message ?? string.Empty;
            RunCompleted = runCompleted;
        }

        public string NodeId { get; }
        public string PayloadId { get; }
        public string Message { get; }
        public bool RunCompleted { get; }
    }
}
