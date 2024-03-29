using Godot;
using NewConsole;
using CommandHandler;

public class ProgramMain : Node {

	bool isQuitting = false;

	public override void _Ready() {

		GetNode<Validation>("/root/Validation").Validate();
		GetNode<PasswordDatabase>("/root/PasswordDB").Init();

		CommandList commandList = new CommandList();
		commandList.AddCommand(new TestCommand());
		commandList.AddCommand(new SafeModeCommand());
		commandList.AddCommand(new GetPasswordCommand());
		commandList.AddCommand(new CopyUserCommand());

		string runCommand = ProjectSettings.GetSetting("editor/CommandOnStart").ToString();
		commandList.ExecuteCommand(runCommand != "" ? runCommand.Split(" ") : OS.GetCmdlineArgs());

		// Free commands
		commandList.Free();
		

		if(Settings.safeMode) {

			Debugger.Print("Starting in Safe Mode", Debugger.DebuggerState.STATE_WARNING);
		}

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
			//GetTree().Connect("node_removed", this, nameof(NodeRemoved));

			//Godot.Collections.Array NodeList = GetTree().Root.GetChildren();
			//NodeList.Remove(this);

			//foreach(Node i in NodeList) {

			//	i.QueueFree();
			//};
			//QueueFree();
		}
		base._Notification(what);
	}
	public override void _ExitTree() {

		Debugger.Print("Closing Program...", Debugger.DebuggerState.STATE_WARNING);
	}

	//private void NodeRemoved(Node node) {

	//	Debugger.Print(node.Name);
	//	Debugger.Print(GetTree().Root.GetChildCount());

	//	if(GetTree().GetNodeCount() <= 2) {


	//	}
	//}
}