namespace TimestampLogger.Processors
{
    public interface IMessageProcessor<T>
    {
        void ProcessMessage(T message);
    }
}
