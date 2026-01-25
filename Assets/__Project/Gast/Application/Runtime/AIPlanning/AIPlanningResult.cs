using System.Collections.Generic;
using Gast.Domain.AI;

namespace Gast.Application.AIPlanning
{
    public readonly struct AIPlanningResult
    {
        public bool IsSuccess { get; }
        public List<IAIObjective> Objectives { get; }
        public AIPlanningError Error { get; }

        AIPlanningResult(bool isSuccess, List<IAIObjective> objectives, AIPlanningError error)
        {
            IsSuccess = isSuccess;
            Objectives = objectives;
            Error = error;
        }

        public static AIPlanningResult Success(List<IAIObjective> objectives)
            => new(true, objectives, AIPlanningError.None);

        public static AIPlanningResult Failure(AIPlanningError error)
            => new(false, new List<IAIObjective>(), error);
    }

    public readonly struct AIPlanningError
    {
        public AIPlanningErrorCode Code { get; }
        public string Message { get; }
        public string Details { get; }

        public AIPlanningError(AIPlanningErrorCode code, string message, string details = null)
        {
            Code = code;
            Message = message;
            Details = details;
        }

        public static AIPlanningError None => new(AIPlanningErrorCode.None, string.Empty);

        public override string ToString()
            => string.IsNullOrEmpty(Details) ? $"[{Code}] {Message}" : $"[{Code}] {Message} - {Details}";
    }

    public enum AIPlanningErrorCode
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