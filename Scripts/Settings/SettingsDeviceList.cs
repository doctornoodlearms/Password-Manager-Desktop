using Godot;
using NewConsole;
public class SettingsDeviceList : VBoxContainer {

	public override void _Ready() {

		GetNode<Button>("StartServer").Connect("pressed", this, nameof(OnStartServerPressed));
		GetNode<Button>("Client/Send").Connect("pressed",this,nameof(OnSendPressed));

		GetTree().Connect("network_peer_connected", this, nameof(OnClientConnected));

		GetNode<Network>("/root/Network").Connect(nameof(Network.ClientInfoRecieved), this, nameof(OnClientInfoRecieved));
	}

	public override void _ExitTree() {

		base._ExitTree();
	}

	private void OnStartServerPressed() {

		GetNode<Network>("/root/Network").StartNetwork();
		GetNode<Button>("StartServer").Visible = false;
		GetNode<Label>("NoClients").Visible = true;
		GetNode<Label>("../Hbox/Address").Text = "IP Address: "+GetNode<Network>("/root/Network").GetIP();
	}

	private void OnClientConnected(int id) {

		GetNode<Network>("/root/Network").clientID = id;
		GetNode<HBoxContainer>("Client").Visible = true;
		GetNode<Label>("NoClients").Visible = false;
	}

	private void OnSendPressed() {

		string key = GetNode<LineEdit>("Client/PasswordKey").Text;
		string label = GetNode<LineEdit>("Client/PasswordLabel").Text;
		GetNode<LineEdit>("Client/PasswordKey").Text = "";
		GetNode<LineEdit>("Client/PasswordLabel").Text = "";
		GetNode<Network>("/root/Network").SendPassword(key, label);
	}

	private void OnClientInfoRecieved(string platform, string model) {

		GetNode<Label>("Client/ClientName").Text = platform+": "+model;
	}
}
