using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace Play.Identity.Service.HealthChecks
{
  public class MongoDnHealthCheck : IHealthCheck
  {
    private readonly MongoClient client;

    public MongoDnHealthCheck(MongoClient client)
    {
      this.client = client;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
      try
      {
        await client.ListDatabaseNamesAsync(cancellationToken);
        return HealthCheckResult.Healthy();
      }
      catch (Exception ex)
      {
        return HealthCheckResult.Unhealthy(exception: ex);
      }
    }
  }
}