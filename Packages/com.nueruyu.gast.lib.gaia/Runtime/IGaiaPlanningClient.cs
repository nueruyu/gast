using System.Threading;
using System.Threading.Tasks;

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

        Task<StoryResponse> CreateStoryAsync(
            CreateStoryRequest request,
            CancellationToken cancellationToken);
    }
}