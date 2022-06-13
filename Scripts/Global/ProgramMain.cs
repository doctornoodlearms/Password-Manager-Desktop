using System;
using Godot;
using NewConsole;

public class ProgramMain : Node {

	Boolean isQuitting = false;

	public override void _Ready() {

		GetNode<Validation>("/root/Validation").Validate();
		GetNode<PasswordDatabase>("/root/PasswordDB").Init();

		base._Ready();
	}

	public override void _Notification(int what) {

		if(what == NotificationWmQuitRequest && !isQuitting) {

			Debugger.Print("Preparing To Close Program");
			Debugger.Print("Saving User Data");

			GetNode<PasswordDatabase>("/root/PasswordDB").SaveToFile();

			Settings.master = Validation.EncryptString(Settings.master.PadRight(32));
			Settings.token = Validation.EncryptString(Settings.token);
			Settings.SaveToFile();

			Debugger.Print("Removing Nodes");
			GetTree().Connect("node_removed", this, nameof(NodeRemoved));

			Godot.Collections.Array NodeList = GetTree().Root.GetChildren();
			NodeList.Remove(this);

			foreach(Node i in NodeList) {

				i.QueueFree();
			};
		}
		base._Notification(what);
	}
	public override void _ExitTree() {

		Debugger.Print("Closing Program...", Debugger.DebuggerState.STATE_WARNING);
	}

	private void NodeRemoved(Node node) {

		if(GetTree().GetNodeCount() <= 2) {

			GetTree().Root.QueueFree();
			GetTree().Quit();
		}
	}
}