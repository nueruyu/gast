using System.Threading;
using System.Threading.Tasks;
using Gast.Lib.Gaia.Dto;

namespace Gast.Lib.Gaia
{
    public interface IGaiaPlanningClient
    {
        Task<PlanningSessionDto> CreateSessionAsync(
            CreateSessionRequest request,
            CancellationToken cancellationToken);

        Task<PlanningSessionDto> SubmitToolOutputsAsync(
            string sessionId,
            SubmitToolOutputsRequest request,
            CancellationToken cancellationToken);
    }
}