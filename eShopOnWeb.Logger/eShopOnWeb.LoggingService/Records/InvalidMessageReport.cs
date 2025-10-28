using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopOnWeb.LoggingService.Models
{
  public record InvalidMessageReport
  {
    public string? ReportingService { get; init; } //e.g. catalogservice
    public string? ErrorMessage { get; init; }
    public string? OriginalMessageBody { get; init; } //the whole body from the received message
  }

}
