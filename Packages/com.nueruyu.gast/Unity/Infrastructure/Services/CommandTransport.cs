namespace Gast.Unity.Infrastructure.Services
{
    /// <summary>
    /// Internal transport DTO used by NewtonsoftCommandSerializer for command serialization.
    /// </summary>
    class CommandTransport
    {
        public string CommandTypeName { get; set; }
        public string CommandPayload { get; set; }

        public CommandTransport()
        {
        }

        public CommandTransport(string commandTypeName, string commandPayload)
        {
            CommandTypeName = commandTypeName;
            CommandPayload = commandPayload;
        }
    }
}