using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eShopOnWeb.LoggingService.Models
{
  public record InvalidMessageLogEntry(
      DateTime Timestamp,
      string LogLevel,
      string? CorrelationId,      // fromm header
      string ReportingService,    // from report (sent from other services) [...]
      string ErrorMessage,        
      string OriginalMessageBody  
  )
  {
    
    public string ToJson() => JsonSerializer.Serialize(this);
  }
}
