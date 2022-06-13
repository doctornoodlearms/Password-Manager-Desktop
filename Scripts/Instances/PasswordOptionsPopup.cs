using Godot;
using System;
using SavedPassword;
using NewConsole;

public class PasswordOptionsPopup : PanelContainer {

	[Signal] public delegate void Accept(PasswordData savedPassword);
	[Signal] public delegate void PasswordUpdate(PasswordData savedPassword);
	[Signal] public delegate void Cancel();

	Boolean moving;
	Vector2 offset=new Vector2();

	private PasswordData savedPassword;

	public override void _Ready() {

		// Buttons
		GetNode<Button>("Margin/Vbox/PasswordGroup/Selection/Button").Connect("pressed", this, nameof(OnGroupSelectPressed));
		GetNode<Button>("Margin/Vbox/Buttons/Accept").Connect("pressed", this, nameof(OnAcceptPressed));
		GetNode<Button>("Margin/Vbox/Buttons/Cancel").Connect("pressed", this, nameof(OnCancelPressed));
		GetNode<Button>("Margin/Vbox/ToggleAdvanced").Connect("pressed", this, nameof(OnAdvancedPressed));

		// Main Settings
		GetNode<LineEdit>("Margin/Vbox/PasswordLabel/LineEdit").Connect("text_changed", this, nameof(OnPasswordLabelTextChanged));
		GetNode<LineEdit>("Margin/Vbox/PasswordKey/LineEdit").Connect("text_changed", this, nameof(OnPasswordKeyTextChanged));

		GetParent<PopupHandler>().Connect(nameof(PopupHandler.GroupSelectionAccept), this, nameof(OnGroupSelectAccept));

		GetNode<Control>("Margin/Vbox/PasswordLabel/LineEdit").GrabFocus();
	}

	public override void _Input(InputEvent @event) {


		switch(@event.GetClass()) {

			case nameof(InputEventMouseButton):
				moving = ((InputEventMouseButton) @event).IsPressed();
				offset = GetGlobalMousePosition() - RectPosition;
				break;

			case nameof(InputEventMouseMotion):
				if(moving) {

					RectPosition = GetGlobalMousePosition() - offset;
				}
				break;

			case nameof(InputEventKey):

				if(@event.IsActionPressed("ui_cancel")) {

					GetTree().SetInputAsHandled();
					QueueFree();
				}
				break;
		}
	}

	public void Popup() {

		GetNode<LineEdit>("Margin/Vbox/Advanced/PasswordCount/LineEdit").Text = Settings.characterCount.ToString();
		GetNode<LineEdit>("Margin/Vbox/Advanced/PasswordUnusable/LineEdit").Text = Settings.unusableCharacters;
		GetNode<OptionButton>("Margin/Vbox/Advanced/PasswordCharacters/Button").Selected = Settings.characterSet;
		GetNode<CheckBox>("Margin/Vbox/Advanced/PasswordLegacy/Button").Pressed = Settings.legacy;

		GetNode<Button>("Margin/Vbox/Buttons/Accept").Disabled = true;
	}
	public void Popup(PasswordData savedPassword) {

		this.savedPassword = savedPassword;
		GetNode<LineEdit>("Margin/Vbox/PasswordLabel/LineEdit").Text = savedPassword.Label;
		GetNode<LineEdit>("Margin/Vbox/PasswordKey/LineEdit").Text = savedPassword.Key;
		GetNode<Label>("Margin/Vbox/PasswordGroup/Selection/Label").Text = savedPassword.Group;

		GetNode<CheckBox>("Margin/Vbox/Advanced/PasswordLegacy/Button").Pressed = savedPassword.Legacy;
		GetNode<LineEdit>("Margin/Vbox/Advanced/PasswordCount/LineEdit").Text = savedPassword.CharCount.ToString();
		GetNode<LineEdit>("Margin/Vbox/Advanced/PasswordUnusable/LineEdit").Text = savedPassword.CharInvalid;
		GetNode<OptionButton>("Margin/Vbox/Advanced/PasswordCharacters/Button").Selected = (int) savedPassword.CharSet;
		GetNode<LineEdit>("Margin/Vbox/Advanced/PasswordPriority/LineEdit").Text = savedPassword.Index.ToString();

		GetNode<Button>("Margin/Vbox/Buttons/Accept").Disabled = savedPassword.Key == "" || savedPassword.Label == "" || savedPassword.Group == "";
	}

	// Button Connections
	private void OnGroupSelectPressed() {

		GetParent<PopupHandler>().PopupGroupSelection();
	}
	private void OnAcceptPressed() {

		Debugger.Print("Accept Pressed");

		String label= GetNode<LineEdit>("Margin/Vbox/PasswordLabel/LineEdit").Text;
		String key = GetNode<LineEdit>("Margin/Vbox/PasswordKey/LineEdit").Text;
		String group = GetNode<Label>("Margin/Vbox/PasswordGroup/Selection/Label").Text;


		// Advanced
		String temp = GetNode<LineEdit>("Margin/Vbox/Advanced/PasswordCount/LineEdit").Text;
		String indexTemp = GetNode<LineEdit>("Margin/Vbox/Advanced/PasswordPriority/LineEdit").Text;
		
		Int32 count = temp.IsValidInteger() ? int.Parse(temp) : Settings.characterCount;
		String invalid = GetNode<LineEdit>("Margin/Vbox/Advanced/PasswordUnusable/LineEdit").Text;
		Int32 set = GetNode<OptionButton>("Margin/Vbox/Advanced/PasswordCharacters/Button").Selected;
		Boolean legacy = GetNode<CheckBox>("Margin/Vbox/Advanced/PasswordLegacy/Button").Pressed;
		Int32 index = indexTemp.IsValidInteger() ? int.Parse(indexTemp) : 0;

		PasswordData newPassword = new PasswordData(
			key,
			newLabel: label,
			newGroup: group,
			newCount: count,
			newInvalid: invalid,
			newSet: (CharacterSets) set,
			newLegacy: legacy,
			newIndex: index
		);

		if(savedPassword != null) {

			EmitSignal(nameof(PasswordUpdate), savedPassword.Id, newPassword);
		}
		else {

			EmitSignal(nameof(Accept), newPassword);
		}
		ClearOptions();
		Visible = false;
	}
	private void OnCancelPressed() {

		Debugger.Print("Cancel Pressed");
		EmitSignal(nameof(Cancel));
		QueueFree();
	}
	private void OnAdvancedPressed() {

		GetNode<VBoxContainer>("Margin/Vbox/Advanced").Visible ^= true;
		GetNode<Button>("Margin/Vbox/ToggleAdvanced").Text = GetNode<VBoxContainer>("Margin/Vbox/Advanced").Visible ? "Hide Advanced" : "Show Advanced";
	}


	// Change Setting Connections
	private void OnGroupSelectAccept(String groupName) {

		GetNode<Label>("Margin/Vbox/PasswordGroup/Selection/Label").Text = groupName;
		CheckAcceptDisable();

		GetNode<Control>("Margin/Vbox/ToggleAdvanced").GrabFocus();
	}
	private void OnPasswordLabelTextChanged(String newPasswordLabel) {

		CheckAcceptDisable();
	}
	private void OnPasswordKeyTextChanged(String newPasswordKey) {

		CheckAcceptDisable();
	}

	private void ClearOptions() {

		Debugger.Print("Clearing Options");

		// Clear base options
		GetNode<LineEdit>("Margin/Vbox/PasswordLabel/LineEdit").Text = "";
		GetNode<LineEdit>("Margin/Vbox/PasswordKey/LineEdit").Text = "";
		GetNode<Label>("Margin/Vbox/PasswordGroup/Selection/Label").Text = "";

		// Clear advanced options
		GetNode<LineEdit>("Margin/Vbox/Advanced/PasswordCount/LineEdit").Text = "";
		GetNode<LineEdit>("Margin/Vbox/Advanced/PasswordUnusable/LineEdit").Text = "";
		GetNode<VBoxContainer>("Margin/Vbox/Advanced").Visible = false;
		savedPassword = null;
	}
	private void CheckAcceptDisable() {

		LineEdit label = GetNode<LineEdit>("Margin/Vbox/PasswordLabel/LineEdit");
		LineEdit key = GetNode<LineEdit>("Margin/Vbox/PasswordKey/LineEdit");
		Label group = GetNode<Label>("Margin/Vbox/PasswordGroup/Selection/Label");

		GetNode<Button>("Margin/Vbox/Buttons/Accept").Disabled = label.Text == "" || key.Text == "" || group.Text == "";
	}
}
