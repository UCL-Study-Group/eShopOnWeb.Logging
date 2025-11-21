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
      string ReportingService,    // Where is the report from?
      string ErrorMessage,        //Whats the error message?
      string OriginalMessageBody  //the message body
  );
  
}
