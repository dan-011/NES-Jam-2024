using General;
using Godot;
using System;

public partial class GadgetsGuideMenu : CanvasLayer
{
	[Signal]
	public delegate void SelectBackEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		selectTimer = GetNode<Timer>("SelectorTimer");
		backLabel = GetNode<Label>("BackLabel");
		selector = GetNode<AnimatedSprite2D>("Selector");
		menuTick = GetNode<AudioStreamPlayer>("MenuTick");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Visible && GameData.Instance.GetIsPaused() && canSelect) {
			InputHandling();
		}
	}

	public void Open() {
		canSelect = true;
		Visible = true;
		selector.Visible = true;
		backLabel.AddThemeColorOverride("font_color", new Color("fcfcfc"));
	}
	
	private void InputHandling() {
		if(Input.IsActionJustPressed("A")) {
			canSelect = false;
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			GoBack("A");
		}
		if(Input.IsActionJustPressed("B")) {
			canSelect = false;
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			GoBack("B");
		}
	}

	private void GoBack(string action) {
		Input.ActionRelease(action);
		selector.Visible = false;
		backLabel.AddThemeColorOverride("font_color", new Color("bcbcbc"));
		selectTimer.Start(0.1);
	}
	
	private void OnSelectorTimerTimeout()
	{
		canSelect = true;
		selectTimer.Stop();
		EmitSignal(SignalName.SelectBack);
	}

	private Timer selectTimer;
	private Label backLabel;
	private AnimatedSprite2D selector;
	private AudioStreamPlayer menuTick;
	private bool canSelect;
}
