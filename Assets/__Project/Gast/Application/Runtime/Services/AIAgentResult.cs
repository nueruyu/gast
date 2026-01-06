using System.Collections.Generic;
using Gast.Api.AI;

namespace Gast.Application.Services
{
    public readonly struct AIAgentResult
    {
        public bool IsSuccess { get; }
        public List<IGoal> Goals { get; }
        public AIAgentError Error { get; }

        AIAgentResult(bool isSuccess, List<IGoal> goals, AIAgentError error)
        {
            IsSuccess = isSuccess;
            Goals = goals;
            Error = error;
        }

        public static AIAgentResult Success(List<IGoal> goals)
            => new(true, goals, AIAgentError.None);

        public static AIAgentResult Failure(AIAgentError error)
            => new(false, new List<IGoal>(), error);
    }

    public readonly struct AIAgentError
    {
        public AIAgentErrorCode Code { get; }
        public string Message { get; }
        public string Details { get; }

        public AIAgentError(AIAgentErrorCode code, string message, string details = null)
        {
            Code = code;
            Message = message;
            Details = details;
        }

        public static AIAgentError None => new(AIAgentErrorCode.None, string.Empty);

        public override string ToString()
            => string.IsNullOrEmpty(Details) ? $"[{Code}] {Message}" : $"[{Code}] {Message} - {Details}";
    }

    public enum AIAgentErrorCode
    {
        None = 0,
        NetworkError,
        Timeout,
        ServerError,
        InvalidResponse,
        ServiceUnavailable,
        Cancelled
    }
}
