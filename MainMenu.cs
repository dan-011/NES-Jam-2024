using General;
using Godot;
using System;
using System.Collections.Generic;

public partial class MainMenu : CanvasLayer
{
	[Signal]
	public delegate void SelectMenuEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		labels = new List<Label>();
		labels.Add(GetNode<Label>("StartGame"));
		labels.Add(GetNode<Label>("Leaderboard"));
		labels.Add(GetNode<Label>("Controls"));
		labels.Add(GetNode<Label>("Credits"));
		labels.Add(GetNode<Label>("Settings"));
		labels.Add(GetNode<Label>("ExitGame"));

		selectors = new List<AnimatedSprite2D>();
		selectors.Add(GetNode<AnimatedSprite2D>("StartGameSelector"));
		selectors.Add(GetNode<AnimatedSprite2D>("LeaderboardSelector"));
		selectors.Add(GetNode<AnimatedSprite2D>("ControlsSelector"));
		selectors.Add(GetNode<AnimatedSprite2D>("CreditsSelector"));
		selectors.Add(GetNode<AnimatedSprite2D>("SettingsSelector"));
		selectors.Add(GetNode<AnimatedSprite2D>("ExitGameSelector"));

		selectTimer = GetNode<Timer>("SelectTimer");
		cur = 0;

		menuTick = GetNode<AudioStreamPlayer>("MenuTick");
	}

	public void Open() {
		for(int i = 0; i < labels.Count; i++) {
			RemoveSelection(i);
		}
		AddSelection(cur);
		Visible = true;
		canSelect = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Visible && canSelect) {
			InputHandling();
		}
	}

	private void RemoveSelection(int i) {
		labels[i].AddThemeColorOverride("font_color", new Color("bcbcbc"));
		selectors[i].Visible = false;
	}

	private void AddSelection(int i) {
		labels[i].AddThemeColorOverride("font_color", new Color("fcfcfc"));
		selectors[i].Visible = true;
	}

	private void InputHandling() {
		if(Input.IsActionPressed(GameData.Instance.GetA())) {
			Input.ActionRelease(GameData.Instance.GetA());
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			RemoveSelection(cur);
			selectTimer.Start(0.1);
			canSelect = false;
		}
		if(Input.IsActionJustPressed("move_down")) {
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			RemoveSelection(cur);
			cur = (cur + 1) % labels.Count;
			AddSelection(cur);
		}
		if(Input.IsActionJustPressed("move_up")) {
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			RemoveSelection(cur);
			cur--;
			cur = cur < 0 ? labels.Count - 1 : cur;
			AddSelection(cur);
		}
	}

	
	private void OnSelectTimerTimeout()
	{
		selectTimer.Stop();
		canSelect = true;
		EmitSignal(SignalName.SelectMenu);
	}

	public int GetSelection() {
		return cur;
	}

	public void SetSelection(int selection) {
		cur = selection;
	}


	private List<Label> labels;
	private List<AnimatedSprite2D> selectors;
	private Timer selectTimer;
	private int cur;
	private AudioStreamPlayer menuTick;
	private bool canSelect;
}
