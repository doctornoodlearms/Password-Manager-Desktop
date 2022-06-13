using Godot;
using System;
using NewConsole;

public class Network : Node
{

	[Signal]
	public delegate void ClientInfoRecieved(String name);

	private const Int32 serverPort = 25565;
    private const Int32 maxClients = 4;

	public Int32 clientID;

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

	public void SendPassword(String key, String label) {

		Debugger.Print("Sending Password Data");
		RpcId(clientID, "RecievePassword",Settings.master, key, label);
	}
	public String GetIP() {

		return IP.ResolveHostname(OS.GetEnvironment("COMPUTERNAME"), IP.Type.Ipv4);
	}

	[Remote]
	public void ClientInfo(String platform, String model) {

		Debugger.Print("Client Info Recieved");
		EmitSignal(nameof(ClientInfoRecieved),platform,model);
	}

	public Boolean IsServerRunning() {

		return GetTree().IsNetworkServer();
	}

}
