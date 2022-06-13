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
		System.Collections.IEnumerator enumerator = GetNode<PasswordDatabase>("/root/PasswordDB").passwordList.Values.GetEnumerator();

		//Create pre existing passwords
		while(enumerator.MoveNext()) {

			Debugger.Print((enumerator.Current as PasswordData).Id);
			CreateNewPassword((PasswordData) enumerator.Current);
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

			newPassword.Name = savedPassword.Id;
			newPassword.savedPassword = savedPassword;
			newPassword.GetNode<Label>("Label").Text = savedPassword.Label;

			Debugger.Print("Checking If Group '"+savedPassword.Group+"' Exists");
			if(HasNode(savedPassword.Group)) {

				Debugger.Print("Group Found, Forwarding Node To Group");
				GetNode<CreatedGroup>(savedPassword.Group).AddPassword(newPassword, savedPassword.Index);
			}
			else {

				Debugger.Print("Failed To Find Group", Debugger.DebuggerState.STATE_WARNING);
				Debugger.Print("Creating New Group: "+ savedPassword.Group);
				CreatedGroup createdGroup = CreateNewGroup(savedPassword.Group);
				createdGroup.AddPassword(newPassword, savedPassword.Index);
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
			GetNode<CreatedGroup>(savedPassword.Group).RemovePassword(savedPassword.Id);

		}
		else if(HasNode(savedPassword.Id)) {

			Debugger.Print("Failed To Find Group: " + savedPassword.Group, Debugger.DebuggerState.STATE_WARNING);
			GetNode<CreatedPassword>(savedPassword.Id).Remove();
		}
	}
}
