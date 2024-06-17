
using NetMQ;
using NetMQ.Sockets;

namespace NetMQClassLibrary
{
	public class Client
	{
		string name;

		public Client(string name)
		{
			this.name = name;
		}

		public void Connect()
		{
			using (var client = new DealerSocket())
			{
				client.Connect("tcp://127.0.0.1:1234");
				var json = new Message($"{name} connected", DateTime.Now, name, "Server").SerializeMessageToJson();
				client.SendFrame(json);
				Task.Run(() => Recieve(client));
				Send(client);
			}
		}

		public void Recieve(DealerSocket client)
		{
			while (true)
			{
				var msg = client.ReceiveFrameString();
				Message.DeserializeJsonToMessage(msg).Print();
			}
		}

		public void Send(DealerSocket client)
		{
			while(true)
			{
				var input = Console.ReadLine();
				if (input == "exit") return;
				var json = new Message(input, DateTime.Now, name, "Server").SerializeMessageToJson();
				client.SendFrame(json);
			}
		}
	}
}
