using Godot;
using Godot.Collections;
using System;
using SavedPassword;
using NewConsole;

public class CreatedPassword : HBoxContainer
{

	public PasswordData savedPassword;

	public override void _Ready() {

		AddToGroup("Password", true);

		GetNode<Button>("Copy").Connect("pressed", this, nameof(OnCopyPressed));
		GetNode<Button>("ViewKey").Connect("pressed", this, nameof(OnViewKeyPressed));
		GetNode<Button>("Edit").Connect("pressed", this, nameof(OnEditPressed));
		GetNode<Button>("Delete").Connect("pressed", this, nameof(OnDeletePressed));
		GetNode<Button>("Export").Connect("pressed", this, nameof(OnExportPressed));

		GetNode<Button>("Export").Disabled = !GetNode<Network>("/root/Network").IsServerRunning();
	}
	public override void _ExitTree() {

		Debugger.Print($"{savedPassword.Id}: Removing from tree");
	}

	public void Remove() {

		Debugger.Print("Removing Self: " + Name, Debugger.DebuggerState.STATE_WARNING);
		Free();
	}

	private void OnCopyPressed() {

		Debugger.Print("Copy Pressed");
		PasswordDatabase.GeneratePassword(savedPassword);
	}
	private void OnViewKeyPressed() {

		Debugger.Print("View Key Pressed");
		GetNode<PopupHandler>("/root/Control/Popups").PopupNotification("Password Key: "+savedPassword.Key, "View Key");
	}
	private void OnEditPressed() {

		Debugger.Print("Edit Pressed");
		GetNode<PopupHandler>("/root/Control/Popups").PopupPasswordOptions(savedPassword);
	}
	private void OnDeletePressed() {

		Debugger.Print("Delete Pressed");
		GetNode<PopupHandler>("/root/Control/Popups").PopupDeleteConfirm(savedPassword);
	}
	private void OnExportPressed() {

		Debugger.Print("Export Pressed");
		GetNode<Network>("/root/Network").SendPassword(savedPassword.Key, savedPassword.Label);
	}
}
