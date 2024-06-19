using General;
using Godot;
using System;

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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Visible && GameData.Instance.GetIsPaused()) {
			InputHandling();
		}
	}

	public void Open() {
		Visible = true;
		backSelector.Visible = true;
		backLabel.AddThemeColorOverride("font_color", new Color("fcfcfc"));
	}

	private void InputHandling() {
		if(Input.IsActionPressed("A")) {
			GD.Print("A in Controls");
			Input.ActionRelease("A");
			if(!Input.IsActionJustPressed("A")) GD.Print("released A");

			backSelector.Visible = false;
			backLabel.AddThemeColorOverride("font_color", new Color("bcbcbc"));

			selectTimer.Start(0.1);
		}
		if(Input.IsActionJustPressed("B")) {
			Input.ActionRelease("B");
			EmitSignal(SignalName.SelectBack);
		}
	}
	
	private void OnSelectTimerTimeout()
	{
		selectTimer.Stop();
		EmitSignal(SignalName.SelectBack);
	}

	private Timer selectTimer;
	private AnimatedSprite2D backSelector;
	private Label backLabel;
}
