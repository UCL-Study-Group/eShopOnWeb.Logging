using System.Text;
using System.Text.Json;

namespace eShopOnWeb.LoggingService.Helpers
{
  /// <summary>
  /// Helper to unpack messages received from other services
  /// </summary>
  public static class MessageHandler
  {
    /// <summary>
    /// Tries to deserialize a message from bytes 
    /// </summary>
    /// <typeparam name="T"> The type </typeparam>
    /// <param name="body"> Message in bytes from RabbitMQ </param>
    /// <param name="logger"> Logger </param>
    /// <returns> Deserialized object T or default (null) if it fails </returns>
    public static T? TryUnpack<T>(ReadOnlyMemory<byte> body, ILogger logger)
    {
      try
      {
        string jsonString = Encoding.UTF8.GetString(body.Span);

        return JsonSerializer.Deserialize<T>(jsonString);
      }
      catch (JsonException ex)
      {

        logger.LogError(ex, "Couldn't deserialize RabbitMQ message to type: {Type}", typeof(T).Name);

        return default;
      }
      catch (Exception ex)
      {

        logger.LogError(ex, "Unexpected exception was thrown. Couldn't deserialize message to type: {Type}", typeof(T).Name);
        return default;
      }


    }
  }
}
