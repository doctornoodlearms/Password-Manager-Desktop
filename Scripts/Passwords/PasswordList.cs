using Godot;
using System;
using SavedPassword;
using NewConsole;

public class PasswordList : Control
{
	public override void _Ready() {

		//Connect to database for passwords
		GetNode<PasswordDatabase>("/root/PasswordDB").Connect(nameof(PasswordDatabase.PasswordCreated),this,nameof(CreateNewPassword));
		GetNode<PasswordDatabase>("/root/PasswordDB").Connect(nameof(PasswordDatabase.PasswordRemoved), this, nameof(RemovePassword));
		
		//Get list of groups
		Debugger.Print("Creating Existing Groups");
		foreach(Node i in GetNode<PasswordDatabase>("/root/PasswordDB").GetGroupNames()) {

			CreateNewGroup(i.Name);
		}

		//Get list of passwords
		Debugger.Print("Creating Existing Passwords");
		foreach(PasswordData i in GetNode<PasswordDatabase>("/root/PasswordDB").passwordList) {

			Debugger.Print(i.Id);
			CreateNewPassword(i);	
		}

		Debugger.Print("Ready");
	}

	public override void _Input(InputEvent @event) {

		if(@event.IsActionPressed("ui_cancel")) {

			GetTree().ChangeScene("res://Scenes/Main.tscn");
		}
	}

	public CreatedGroup CreateNewGroup(String groupName) {

		Debugger.Print("Adding New Group '"+groupName+"' To Scene Tree");
		VBoxContainer newGroup = GD.Load<PackedScene>("res://Scenes/Instances/Saved_Group.tscn").Instance<CreatedGroup>();
		newGroup.Name = groupName;
		newGroup.GetNode<CheckBox>("GroupHead/CheckBox").Text = groupName;
		AddChild(newGroup);

		return (CreatedGroup) newGroup;
	}
	public void CreateNewPassword(PasswordData savedPassword) {

		Debugger.Print("Creating New Password: " + savedPassword.GetHashCode());
		try {

			CreatedPassword newPassword = GD.Load<PackedScene>("res://Scenes/Instances/Saved_Password.tscn").Instance<CreatedPassword>();

			newPassword.Name = savedPassword.Id.ToString();
			newPassword.savedPassword = savedPassword;
			newPassword.GetNode<Label>("Label").Text = savedPassword.Label;

			Debugger.Print("Checking If Group '"+savedPassword.Group+"' Exists");
			if(HasNode(savedPassword.Group)) {

				Debugger.Print("Group Found, Forwarding Node To Group");
				int index = GetNode<PasswordDatabase>("/root/PasswordDB").GetPasswordIndexInGroup(savedPassword.Id);
				GetNode<CreatedGroup>(savedPassword.Group).AddPassword(newPassword, index);
			}
			else {

				Debugger.Print("Failed To Find Group", Debugger.DebuggerState.STATE_WARNING);
				Debugger.Print("Creating New Group: "+ savedPassword.Group);
				CreatedGroup createdGroup = CreateNewGroup(savedPassword.Group);
				createdGroup.AddPassword(newPassword, 0);
			}
		}
		catch(InvalidPasswordIDException e) {

			Debugger.Print(e.Message, Debugger.DebuggerState.STATE_ERROR);
		}
		
	}
	public void RemovePassword(PasswordData savedPassword) {

		Debugger.Print("Preparing To Remove Password: " + savedPassword.Id);
		Debugger.Print("Checking If Group '" + savedPassword.Group + "' Exists");
		if(HasNode(savedPassword.Group)) {

			Debugger.Print("Group '"+savedPassword.Group+"' Found");
			GetNode<CreatedGroup>(savedPassword.Group).RemovePassword(savedPassword.Id.ToString());

		}
		else if(HasNode(savedPassword.Id.ToString())) {

			Debugger.Print("Failed To Find Group: " + savedPassword.Group, Debugger.DebuggerState.STATE_WARNING);
			GetNode<CreatedPassword>(savedPassword.Id.ToString()).Remove();
		}
	}
}
