
namespace Catan.Proxy
{
    public enum CatanWsMessageType { Connect, Ack, GameAdded, GameDeleted, RegisterForGameNotifications }
    public class WsMessage
    {
        public CatanWsMessageType MessageType { get; set; }
        public string DataType { get; set; }
        public int Sequence { get; set; }
        public object Data { get; set; }
        public override string ToString()
        {
            return $"[MessageType={MessageType}][DataType={DataType}][Sequence={Sequence}][Data={Data}]";

        }
    }

    public class WsGameMessage
    {
        public GameInfo GameInfo { get; set; }

        public WsGameMessage() { }

        public override string ToString()
        {
            return $"[GameInfo={GameInfo}]" + base.ToString();

        }
    }
}
