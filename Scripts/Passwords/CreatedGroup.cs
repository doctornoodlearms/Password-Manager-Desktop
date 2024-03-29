using Godot;
using NewConsole;
public class CreatedGroup : VBoxContainer
{
	public override void _Ready() {

		Debugger.Print("Ready");

		GetNode<CheckBox>("GroupHead/CheckBox").Connect("toggled",this,nameof(OnCheckBoxToggled));
		if(Name != "GroupDuplicate") {

			AddToGroup("Groups");
		}
	}

	public void AddPassword(Node newPassword, int index) {

		Debugger.Print("Adding New Password: "+newPassword.Name);
		if(GetNode("GroupItems").GetChildCount() != 0) {

			GetNode("GroupItems").AddChild(newPassword);
			GetNode("GroupItems").MoveChild(newPassword, index);
		}
		else {

			GetNode("GroupItems").AddChild(newPassword);
		}
	}

	public void RemovePassword(string passwordID) {

		Debugger.Print("Checking If '"+passwordID+"' Exists");
		if(HasNode("GroupItems/"+passwordID)) {

			Debugger.Print("Removing Child: " + passwordID);
			GetNode<CreatedPassword>("GroupItems/"+passwordID).Remove();


			Debugger.Print("Checking If Self Can Be Removed");
			if(GetNode("GroupItems").GetChildCount() <= 0) {

				Debugger.Print("Removing Self: " + Name, Debugger.DebuggerState.STATE_WARNING);
				Free();
			}
		}
		else {

			Debugger.Print("Failed To Remove '"+passwordID+"' Doesnt Exist", Debugger.DebuggerState.STATE_ERROR);
		}
	}

	private void OnCheckBoxToggled(bool value) {

		GetNode<Control>("GroupItems").Visible = value;
	}
}
