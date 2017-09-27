namespace Payment.Service.Common
{
    public static class LogConstansts
    {
        public static class Protocols
        {
            public const string Http = "http";
            public const string Ampq = "ampq";
        }

        public static class Levels
        {
            public const string Debug = "debug";
            public const string Error = "error";
            public const string Breaking = "breaking";
        }

        public static class Common
        {
            public const string OperationId = "operationId";
        }

        public static class QueueMessageHeaderNames
        {
            public const string Publisher = "publisher";
        }
    }
  
}