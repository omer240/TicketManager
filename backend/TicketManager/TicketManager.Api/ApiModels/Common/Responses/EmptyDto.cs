namespace TicketManager.Api.ApiModels.Common.Responses
{
    public sealed class EmptyDto
    {
        public static readonly EmptyDto Instance = new();
        private EmptyDto() { }
    }
}
