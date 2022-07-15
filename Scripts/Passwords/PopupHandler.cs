using Godot;
using SavedPassword;
using NewConsole;
public class PopupHandler : Control
{

	[Signal] public delegate void GroupSelectionAccept(string groupName);
	[Signal] public delegate void GroupSelectionUpdate(string passwordID, string groupName);
	[Signal] public delegate void GroupSelectionCancel();


	public void PopupNotification(string message, string title) {

		GetNode<AcceptDialog>("Notification").DialogText = message;
		GetNode<AcceptDialog>("Notification").WindowTitle = title;
		GetNode<AcceptDialog>("Notification").PopupCentered();
	}

	public void PopupDeleteConfirm(PasswordData savedPassword) {

		if(GetNode<ConfirmationDialog>("DeleteConfirm").IsConnected("confirmed", this, nameof(OnDeleteConfirmPressed))) {

			GetNode<ConfirmationDialog>("DeleteConfirm").Disconnect("confirmed",this,nameof(OnDeleteConfirmPressed));
		}
		GetNode<ConfirmationDialog>("DeleteConfirm").Connect("confirmed", this, nameof(OnDeleteConfirmPressed), new Godot.Collections.Array { savedPassword });

		GetNode<ConfirmationDialog>("DeleteConfirm").DialogText = "Are you sure you want to delete: "+savedPassword.Label;
		GetNode<ConfirmationDialog>("DeleteConfirm").PopupCentered();
	}

	public void PopupGroupSelection() {

		GroupSelectPopup groupSelect = GD.Load<PackedScene>("res://Scenes/Instances/Popup_GroupSelection.tscn").Instance<GroupSelectPopup>();
		groupSelect.Connect(nameof(GroupSelectPopup.Accept), this, nameof(OnGroupSelectAccept));
		groupSelect.Connect(nameof(GroupSelectPopup.Cancel), this, nameof(OnGroupSelectCancel));
		groupSelect.Connect("tree_exiting", this, nameof(OnGroupSelectExitTree), new Godot.Collections.Array { groupSelect });
		AddChild(groupSelect);
	}
	private void OnGroupSelectAccept(string groupName) {

		EmitSignal(nameof(GroupSelectionAccept), groupName);
	}
	private void OnGroupSelectCancel() {

		EmitSignal(nameof(GroupSelectionCancel));
	}
	private void OnGroupSelectExitTree(GroupSelectPopup groupSelect) {

		groupSelect.Disconnect(nameof(GroupSelectPopup.Accept), this, nameof(OnGroupSelectAccept));
		groupSelect.Disconnect(nameof(GroupSelectPopup.Cancel), this, nameof(OnGroupSelectCancel));
		groupSelect.Disconnect("tree_exiting", this, nameof(OnGroupSelectExitTree));
	}

	/// <summary>
	/// Open password config for a new password
	/// </summary>
	public void PopupPasswordOptions() {

		PasswordOptionsPopup options = GD.Load<PackedScene>("res://Scenes/Instances/Popup_PasswordOptions.tscn").Instance<PasswordOptionsPopup>();
		options.Connect(nameof(PasswordOptionsPopup.Accept), this, nameof(OnPasswordOptionsAccept));
		options.Connect(nameof(PasswordOptionsPopup.PasswordUpdate), this, nameof(OnPasswordOptionsUpdated));
		options.Connect("tree_exiting", this, nameof(OnPasswordOptionsExitTree), new Godot.Collections.Array { options });
		options.Popup();
		AddChild(options);
	}
	/// <summary>
	/// Open config for a pre-existing password
	/// </summary>
	/// <param name="savedPassword"></param>
	public void PopupPasswordOptions(PasswordData savedPassword) {

		PasswordOptionsPopup options = GD.Load<PackedScene>("res://Scenes/Instances/Popup_PasswordOptions.tscn").Instance<PasswordOptionsPopup>();
		options.Connect(nameof(PasswordOptionsPopup.Accept), this, nameof(OnPasswordOptionsAccept));
		options.Connect(nameof(PasswordOptionsPopup.PasswordUpdate), this, nameof(OnPasswordOptionsUpdated));
		options.Connect("tree_exiting", this, nameof(OnPasswordOptionsExitTree), new Godot.Collections.Array { options });
		options.Popup(savedPassword);
		AddChild(options);
	}
	private void OnPasswordOptionsAccept(PasswordData savedPassword) {
		
		Debugger.Print("Requesting Create Password: "+ savedPassword.Id);
		GetNode<PasswordDatabase>("/root/PasswordDB").CreatePassword(savedPassword);
	}
	private void OnPasswordOptionsUpdated(int oldPasswordID, PasswordData savedPassword) {

		Debugger.Print("Updating Password: "+oldPasswordID);
		GetNode<PasswordDatabase>("/root/PasswordDB").UpdatePassword(oldPasswordID, savedPassword);
	}
	private void OnPasswordOptionsExitTree(PasswordOptionsPopup options) {

		options.Disconnect(nameof(PasswordOptionsPopup.Accept), this, nameof(OnPasswordOptionsAccept));
		options.Disconnect(nameof(PasswordOptionsPopup.PasswordUpdate), this, nameof(OnPasswordOptionsUpdated));
		options.Disconnect("tree_exiting", this, nameof(OnPasswordOptionsExitTree));
	}

	private void OnDeleteConfirmPressed(PasswordData savedPassword) {

		GetNode<PasswordDatabase>("/root/PasswordDB").RemovePassword(savedPassword);
	}
}
