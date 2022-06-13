using Godot;
using System;
using NewConsole;
public class SettingsDeviceList : VBoxContainer {

	public override void _Ready() {

		GetNode<Button>("StartServer").Connect("pressed", this, nameof(OnStartServerPressed));
		GetNode<Button>("Client/Send").Connect("pressed",this,nameof(OnSendPressed));

		GetTree().Connect("network_peer_connected", this, nameof(OnClientConnected));

		GetNode<Network>("/root/Network").Connect("ClientInfoRecieved", this, nameof(OnClientInfoRecieved));

	}

	private void OnStartServerPressed() {

		GetNode<Network>("/root/Network").StartNetwork();
		GetNode<Button>("StartServer").Visible = false;
		GetNode<Label>("NoClients").Visible = true;
		GetNode<Label>("../Hbox/Address").Text = "IP Address: "+GetNode<Network>("/root/Network").GetIP();
	}

	private void OnClientConnected(Int32 id) {

		GetNode<Network>("/root/Network").clientID = id;
		GetNode<HBoxContainer>("Client").Visible = true;
		GetNode<Label>("NoClients").Visible = false;
	}

	private void OnSendPressed() {

		String key = GetNode<LineEdit>("Client/PasswordKey").Text;
		String label = GetNode<LineEdit>("Client/PasswordLabel").Text;
		GetNode<LineEdit>("Client/PasswordKey").Text = "";
		GetNode<LineEdit>("Client/PasswordLabel").Text = "";
		GetNode<Network>("/root/Network").SendPassword(key, label);
	}

	private void OnClientInfoRecieved(String platform, String model) {

		GetNode<Label>("Client/ClientName").Text = platform+": "+model;
	}
}
