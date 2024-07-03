using General;
using Godot;
using System;
using System.Collections.Generic;

public partial class GeneralGameOverMenu : Node2D
{
	[Signal]
	public delegate void SelectOptionEventHandler(int selectedIndex);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		labels = new List<Label>();
		labels.Add(GetNode<Label>("EnterLeaderboardLabel"));
		labels.Add(GetNode<Label>("RestartLabel"));
		labels.Add(GetNode<Label>("MainMenuLabel"));
		labels.Add(GetNode<Label>("ExitGameLabel"));

		selectors = new List<AnimatedSprite2D>();
		selectors.Add(GetNode<AnimatedSprite2D>("EnterLeaderboardSelect"));
		selectors.Add(GetNode<AnimatedSprite2D>("RestartSelect"));
		selectors.Add(GetNode<AnimatedSprite2D>("MainMenuSelect"));
		selectors.Add(GetNode<AnimatedSprite2D>("ExitGameSelect"));

		scoreLabel = GetNode<Label>("ScoreLabel");
		selectTimer = GetNode<Timer>("SelectTimer");

		cur = 0;
	}

	public void Open() {
		for(int i = 0; i < labels.Count; i++) {
			labels[i].AddThemeColorOverride("font_color", new Color("bcbcbc"));
			selectors[i].Visible = false;
		}
		scoreLabel.Text = "Final  Score:  " + GameData.Instance.GetScore().ToString();

		labels[cur].AddThemeColorOverride("font_color", new Color("fcfcfc"));
		selectors[cur].Visible = true;
		canMove = true;
		Visible = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Visible && GlobalPosition.Y == 0 && canMove) {
			InputHandling();
		}
	}

	private void InputHandling() {
		if(Input.IsActionPressed("move_up")) {
			Input.ActionRelease("move_up");
			ToggleSelect(cur);
			cur--;
			if(cur < 0) cur = labels.Count - 1;
			ToggleSelect(cur);
		}
		if(Input.IsActionPressed("move_down")) {
			Input.ActionRelease("move_down");
			ToggleSelect(cur);
			cur = (cur + 1) % labels.Count;
			ToggleSelect(cur);
		}
		if(Input.IsActionPressed(GameData.Instance.GetA())) {
			Input.ActionRelease(GameData.Instance.GetA());
			ToggleSelect(cur);
			selectTimer.Start(0.1);
			canMove = false;
		}
	}

	private void ToggleSelect(int pos) {
		selectors[pos].Visible = !selectors[pos].Visible;
		if(selectors[pos].Visible) labels[cur].AddThemeColorOverride("font_color", new Color("fcfcfc"));
		else labels[cur].AddThemeColorOverride("font_color", new Color("bcbcbc"));
	}

	private void OnSelectTimerTimout()
	{
		selectTimer.Stop();
		canMove = true;
		GD.Print("Emit signal");
		EmitSignal(SignalName.SelectOption, cur);
	}

	private List<Label> labels;
	private List<AnimatedSprite2D> selectors;
	private Label scoreLabel;
	private int cur;
	private Timer selectTimer;
	bool canMove;
}
