using RabbitMQ.Client;

public static class RabbitMqPropertiesExtensions
{
/// <summary>
/// Easy conversion to BasicProperties from IReadOnlyBasicProperties
/// Used for when an Invalid Message should be published to the Manual Inspection Exchange
/// To keep original headers
/// </summary>
/// <param name="readOnlyProps"> the headers from the original message</param>
/// <returns> BasicProperties type from the original message's headers </returns>
  public static BasicProperties ConvertToBasicProperties(this IReadOnlyBasicProperties readOnlyProps)
  {
    var newProps = new BasicProperties();

    newProps.AppId = readOnlyProps.AppId;
    newProps.ClusterId = readOnlyProps.ClusterId;
    newProps.ContentEncoding = readOnlyProps.ContentEncoding;
    newProps.ContentType = readOnlyProps.ContentType;
    newProps.CorrelationId = readOnlyProps.CorrelationId;
    newProps.DeliveryMode = readOnlyProps.DeliveryMode;
    newProps.Expiration = readOnlyProps.Expiration;
    newProps.Headers = readOnlyProps.Headers;
    newProps.MessageId = readOnlyProps.MessageId;
    newProps.Priority = readOnlyProps.Priority;
    newProps.ReplyTo = readOnlyProps.ReplyTo;
    newProps.Timestamp = readOnlyProps.Timestamp;
    newProps.Type = readOnlyProps.Type;
    newProps.UserId = readOnlyProps.UserId;

    return newProps;
  }
}
