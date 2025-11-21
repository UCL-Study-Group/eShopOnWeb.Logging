using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eShopOnWeb.LoggingService.Models
{
  public record DeadLetterLogEntry(
       DateTime Timestamp,
       string LogLevel,
       string? CorrelationId,      // from header [...]
       string FailedQueue,         
       string ErrorReason,         
       string OriginalRoutingKey,  
       string OriginalExchange,    
       List<object>? FailureHistory // call stack
   )
  {
   
    public string ToJson() => JsonSerializer.Serialize(this);
  }
}
