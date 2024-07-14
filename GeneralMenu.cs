using General;
using Godot;
using System;
using System.Collections.Generic;

public partial class GeneralMenu : CanvasLayer
{
	[Signal]
	public delegate void SelectMenuOptionEventHandler(int selectedIndex);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		selectTimer = GetNode<Timer>("SelectTimer");

		selectors = new List<AnimatedSprite2D>();
		selectors.Add(GetNode<AnimatedSprite2D>("ResumeSelector"));
		selectors.Add(GetNode<AnimatedSprite2D>("ControlsSelector"));
		selectors.Add(GetNode<AnimatedSprite2D>("GadgetsGuideSelector"));
		selectors.Add(GetNode<AnimatedSprite2D>("SettingsSelector"));
		selectors.Add(GetNode<AnimatedSprite2D>("MainMenuSelector"));
		selectors.Add(GetNode<AnimatedSprite2D>("ExitGameSelector"));

		labels = new List<Label>();
		labels.Add(GetNode<Label>("ResumeLabel"));
		labels.Add(GetNode<Label>("ControlsLabel"));
		labels.Add(GetNode<Label>("GadgetsGuideLabel"));
		labels.Add(GetNode<Label>("SettingsLabel"));
		labels.Add(GetNode<Label>("MainMenuLabel"));
		labels.Add(GetNode<Label>("ExitGameLabel"));

		for(int i = 0; i < labels.Count; i++) {
			selectors[i].Visible = false;
			labels[i].AddThemeColorOverride("font_color", new Color("bcbcbc"));
		}
		cur = 0;
		menuTick = GetNode<AudioStreamPlayer>("MenuTick");
		Select();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Visible && GameData.Instance.GetIsPaused() && canMove) {
			InputHandling();
		}
	}

	private void InputHandling() {
		if(Input.IsActionPressed("move_up")) {
			Input.ActionRelease("move_up");
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			Deselect();
			cur--;
			if(cur < 0) cur = labels.Count - 1;
			Select();
		}
		if(Input.IsActionPressed("move_down")) {
			Input.ActionRelease("move_down");
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			Deselect();
			cur = (cur + 1) % labels.Count;
			Select();
		}
		if(Input.IsActionPressed(GameData.Instance.GetA())) {
			Input.ActionRelease(GameData.Instance.GetA());
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			Deselect();
			selectTimer.Start(0.1);
		}
	}
	private void Select() {
		selectors[cur].Visible = true;
		labels[cur].AddThemeColorOverride("font_color", new Color("fcfcfc"));
	}
	
	private void Deselect() {
		selectors[cur].Visible = false;
		labels[cur].AddThemeColorOverride("font_color", new Color("bcbcbc"));
	}
	
	public void Open() {
		Select();
		canMove = true;
		Visible = true;
	}

	private void OnSelectTimerTimeout()
	{
		selectTimer.Stop();
		EmitSignal(SignalName.SelectMenuOption, cur);
	}

	public void SetSelectPosition(int pos) {
		cur = pos;
	}

	private List<AnimatedSprite2D> selectors;
	private List<Label> labels;
	private int cur;
	private Timer selectTimer;
	private bool canMove;
	private AudioStreamPlayer menuTick;
}
