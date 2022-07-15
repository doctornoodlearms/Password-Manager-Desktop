using Godot;
using NewConsole;
using CommandHandler;

public class TestCommand : BaseCommand {

	public override string Name => "TestCommand";
	public override string[] AliasList => new string[] { "test", "-test" };
	public override void Execute(string[] args) {

		Debugger.Print("Command Test", Debugger.DebuggerState.STATE_WARNING);
	}
}