using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Gast.Lib.Gaia
{
    public class GaiaPlanningClient : IGaiaPlanningClient
    {
        readonly GaiaServerSettings settings;
        readonly HttpClient httpClient;
        readonly JsonSerializerSettings serializerSettings;

        public GaiaPlanningClient(GaiaServerSettings settings)
        {
            this.settings = settings;
            httpClient = new HttpClient { Timeout = System.TimeSpan.FromSeconds(60) };
            serializerSettings = GaiaJsonSettings.Create();
        }

        public async Task<PlanningSessionDto> CreateSessionAsync(
            CreateSessionRequest request,
            CancellationToken cancellationToken)
        {
            return await PostAsync<CreateSessionRequest, PlanningSessionDto>("/planning/request", request, cancellationToken);
        }

        public async Task<PlanningSessionDto> SubmitToolOutputsAsync(
            string sessionId,
            SubmitToolOutputsRequest request,
            CancellationToken cancellationToken)
        {
            return await PostAsync<SubmitToolOutputsRequest, PlanningSessionDto>($"/planning/respond/{sessionId}", request, cancellationToken);
        }

        public async Task<StoryResponseDto> CreateStoryAsync(
            CreateStoryRequest request,
            CancellationToken cancellationToken)
        {
            return await PostAsync<CreateStoryRequest, StoryResponseDto>("/story/generate", request, cancellationToken);
        }

        async Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var requestJson = JsonConvert.SerializeObject(request, serializerSettings);
                var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{settings.BaseUrl}{path}", content, cancellationToken);
                return await ProcessResponseAsync<TResponse>(response);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                throw new GaiaTimeoutException("The request to the Gaia server timed out.", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new GaiaConnectionException("A connection error occurred while communicating with the Gaia server.", ex);
            }
        }

        async Task<T> ProcessResponseAsync<T>(HttpResponseMessage response)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new GaiaServerException(
                    response.StatusCode,
                    responseJson,
                    $"Gaia server returned an error: {(int)response.StatusCode} {response.ReasonPhrase}");
            }

            return JsonConvert.DeserializeObject<T>(responseJson, serializerSettings);
        }
    }
}