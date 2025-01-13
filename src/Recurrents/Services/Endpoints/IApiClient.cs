using Refit;

namespace Recurrents.Services.Endpoints;
[Headers("Content-Type: application/json; charset=utf-8")]
public interface IApiClient
{
    [Get("/latest")]
    Task<ApiResponse<Currency>> GetCurrency(CancellationToken cancellationToken = default);
}
