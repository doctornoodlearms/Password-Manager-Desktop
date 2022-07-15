using Godot;
using NewConsole;

public class GroupSelectPopup : PanelContainer {

	[Signal] public delegate void Accept(string groupName);
	[Signal] public delegate void Cancel();

	bool moving;
	Vector2 offset = new Vector2();

	public override void _Ready() {

		GetNode<OptionButton>("Vbox/OptionButton").Connect("item_selected", this, nameof(OnOptionButtonItemSelected));
		GetNode<Button>("Vbox/Buttons/Accept").Connect("pressed", this, nameof(OnAcceptPressed));
		GetNode<Button>("Vbox/Buttons/Cancel").Connect("pressed", this, nameof(OnCancelPressed));

		Godot.Collections.Array groupList = GetTree().GetNodesInGroup("Groups");

		System.Collections.IEnumerator enumerator = groupList.GetEnumerator();

		while(enumerator.MoveNext()) {

			Node itemName = (Node) enumerator.Current;
			AddItem(itemName.Name);
		}
		GetNode<OptionButton>("Vbox/OptionButton").AddSeparator();
		AddItem("New Group");

		GetNode<OptionButton>("Vbox/OptionButton").GrabFocus();
	}

	public override void _GuiInput(InputEvent @event) {

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
		}
	}

	private void AddItem(string value) {

		GetNode<OptionButton>("Vbox/OptionButton").AddItem(value);
	}
	private void OnOptionButtonItemSelected(int index) {

		GetNode<LineEdit>("Vbox/Custom").Visible = index == GetNode<OptionButton>("Vbox/OptionButton").GetItemCount() - 1;
	}
	private void OnAcceptPressed() {

		string groupName;
		if(GetNode<LineEdit>("Vbox/Custom").Visible) {

			groupName = GetNode<LineEdit>("Vbox/Custom").Text;
		}
		else {

			int index = GetNode<OptionButton>("Vbox/OptionButton").Selected;
			groupName = GetNode<OptionButton>("Vbox/OptionButton").GetItemText(index);
		}

		Debugger.Print("Group Selected: '"+groupName+"'");
		EmitSignal(nameof(Accept), groupName);
		QueueFree();
	}
	private void OnCancelPressed() {

		EmitSignal(nameof(Cancel));
		QueueFree();
	}
}
