using General;
using Godot;
using System;
using System.Collections.Generic;

public partial class ControlsMenu : CanvasLayer
{
	[Signal]
	public delegate void SelectBackEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		selectTimer = GetNode<Timer>("SelectTimer");
		backSelector = GetNode<AnimatedSprite2D>("BackSelector");
		backLabel = GetNode<Label>("BackLabel");

		aLabel = GetNode<Label>("ALabel");
		bLabel = GetNode<Label>("BLabel");
		startLabel = GetNode<Label>("StartLabel");
		selectLabel = GetNode<Label>("SelectLabel");
		dPadLabel = GetNode<Label>("DPadLabel");

		labels = new List<Label>();
		labels.Add(aLabel);
		labels.Add(bLabel);
		labels.Add(startLabel);
		labels.Add(selectLabel);
		labels.Add(dPadLabel);

		GameData.Instance.SetControls(Input.GetConnectedJoypads().Count > 0 ? Input.GetJoyName(Input.GetConnectedJoypads()[0]) : "");
		if(Input.GetConnectedJoypads().Count > 0) GD.Print(Input.GetJoyName(Input.GetConnectedJoypads()[0]));
		aLabel.Text = GameData.Instance.GetControllerMapping("A") + ":  Use  gadget";
		bLabel.Text = GameData.Instance.GetControllerMapping("B") + ":  Boost  forward";
		startLabel.Text = GameData.Instance.GetControllerMapping("Start") + ":  Open  menu";
		selectLabel.Text = GameData.Instance.GetControllerMapping("Select") + ":  Change gadget";
		dPadLabel.Text = GameData.Instance.GetControllerMapping("D-Pad") + ":  Move  player";

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Visible) { // && GameData.Instance.GetIsPaused()) {
			InputHandling();
		}
	}

	public void Open() {
		Visible = true;
		backSelector.Visible = true;
		backLabel.AddThemeColorOverride("font_color", new Color("fcfcfc"));
	}

	private void InputHandling() {
		if(Input.IsActionPressed(GameData.Instance.GetA())) {
			GoBack(GameData.Instance.GetA());
		}
		if(Input.IsActionJustPressed(GameData.Instance.GetB())) {
			GoBack(GameData.Instance.GetB());
		}
	}

	private void GoBack(string action) {
		Input.ActionRelease(action);
		backSelector.Visible = false;
		backLabel.AddThemeColorOverride("font_color", new Color("bcbcbc"));
		selectTimer.Start(0.1);
	}
	
	private void OnSelectTimerTimeout()
	{
		selectTimer.Stop();
		EmitSignal(SignalName.SelectBack);
	}

	public void ShiftText(Vector2 offset) {
		for(int i = 0; i < labels.Count; i++) {
			Vector2 pos = labels[i].GlobalPosition;
			pos.X += offset.X;
			pos.Y += offset.Y;
			labels[i].Position = pos;
		}
	}

	public void ShiftBackButton(Vector2 offset) {
		Vector2 pos = backLabel.GlobalPosition;
		pos.X += offset.X;
		pos.Y += offset.Y;
		backLabel.GlobalPosition = pos;

		Vector2 selPos = backSelector.GlobalPosition;
		selPos.X += offset.X;
		selPos.Y += offset.Y;
		backSelector.GlobalPosition = selPos;
	}

	public void SetTextColor(Color color) {
		for(int i = 0; i < labels.Count; i++) {
			labels[i].AddThemeColorOverride("font_color", color);
		}
	}

	private Timer selectTimer;
	private AnimatedSprite2D backSelector;
	private Label backLabel;
	private Label aLabel;
	private Label bLabel;
	private Label startLabel;
	private Label selectLabel;
	private Label dPadLabel;
	private List<Label> labels;
}
