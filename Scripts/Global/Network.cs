using Godot;
using NewConsole;

public class Network : Node
{

	[Signal]
	public delegate void ClientInfoRecieved(string name);

	private const int serverPort = 25565;
    private const int maxClients = 4;

	public int clientID;

	public void StartNetwork() {

		if(GetTree().IsNetworkServer() == false) {
			NetworkedMultiplayerENet peer = new NetworkedMultiplayerENet();
			peer.CreateServer(serverPort, maxClients);
			GetTree().NetworkPeer = peer;

			Debugger.Print("Server Started");
		}
		else {
			Debugger.Print("Server Currently Running");
		}
	}

	public void SendPassword(string key, string label) {

		Debugger.Print("Sending Password Data");
		RpcId(clientID, "RecievePassword",Settings.master, key, label);
	}
	public string GetIP() {

		return IP.ResolveHostname(OS.GetEnvironment("COMPUTERNAME"), IP.Type.Ipv4);
	}

	[Remote]
	public void ClientInfo(string platform, string model) {

		Debugger.Print("Client Info Recieved");
		EmitSignal(nameof(ClientInfoRecieved),platform,model);
	}

	public bool IsServerRunning() {

		return GetTree().IsNetworkServer();
	}

}
