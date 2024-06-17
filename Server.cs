
using NetMQ.Sockets;
using NetMQ;

namespace NetMQClassLibrary
{
	public class Server
	{
		Dictionary<string, NetMQFrame> serverList = new();

		public void Run()
		{
			using (var server = new RouterSocket())
			{
				server.Bind("tcp://*:1234");
				while (true)
				{
					var receive = server.ReceiveMultipartMessage();
					var message = Message.DeserializeJsonToMessage(receive.Last.ConvertToString());
					if (!serverList.ContainsValue(receive.First))
					{
						if (serverList.ContainsKey(message.NickNameFrom)) serverList[message.NickNameFrom] = receive.First;
						else serverList.Add(message.NickNameFrom, receive.First);
					}
					message.Print();
					Task.Run(() =>
					{
						foreach (var client in serverList)
						{
							var responsemsg = new NetMQMessage();
							responsemsg.Append(client.Value);
							responsemsg.Append(receive.Last.ConvertToString());
							server.SendMultipartMessage(responsemsg);
						}
					});
                }
			}
		}
	}
}
