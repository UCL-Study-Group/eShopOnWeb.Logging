using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopOnWeb.LoggingService.Models
{
  public record InvalidMessageReport
  {
    public required string ReportingService { get; init; } //e.g. catalogservice
    public required string ErrorMessage { get; init; }
    public string? OriginalMessageBody { get; init; } //the whole body from the received message
  }

}
