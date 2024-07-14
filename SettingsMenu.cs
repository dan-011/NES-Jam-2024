using General;
using Godot;
using System;
using System.Collections.Generic;

public partial class SettingsMenu : CanvasLayer
{
	[Signal]
	public delegate void BackSelectEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		labels = new List<Label>();
		labels.Add(GetNode<Label>("MusicLabel"));
		labels.Add(GetNode<Label>("SFXLabel"));
		labels.Add(GetNode<Label>("RumbleLabel"));
		labels.Add(GetNode<Label>("BackLabel"));

		selectors = new List<AnimatedSprite2D>();
		selectors.Add(GetNode<AnimatedSprite2D>("MusicSelector"));
		selectors.Add(GetNode<AnimatedSprite2D>("SFXSelector"));
		selectors.Add(GetNode<AnimatedSprite2D>("RumbleSelector"));
		selectors.Add(GetNode<AnimatedSprite2D>("BackSelector"));

		checkboxes = new List<AnimatedSprite2D>();
		checkboxes.Add(GetNode<AnimatedSprite2D>("MusicCheck"));
		checkboxes.Add(GetNode<AnimatedSprite2D>("SFXCheck"));
		checkboxes.Add(GetNode<AnimatedSprite2D>("RumbleCheck"));

		selectOptions = new List<VoidMethod>();
		selectOptions.Add(CheckMusic);
		selectOptions.Add(CheckSFX);
		selectOptions.Add(CheckRumble);
		selectOptions.Add(Back);

		selectTimer = GetNode<Timer>("SelectTimer");
		settingsLabel = GetNode<Label>("SettingsLabel");
		menuTick = GetNode<AudioStreamPlayer>("MenuTick");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Visible && canSelect) {
			InputHandling();
		}
	}

	public void Open() {
		for(int i = 0; i < labels.Count; i++) {
			selectors[i].Visible = false;
			labels[i].AddThemeColorOverride("font_color", new Color("bcbcbc"));
		}

		checkboxes[0].Frame = GameData.Instance.GetCanPlayMusic() ? 1 : 0;
		checkboxes[1].Frame = GameData.Instance.GetCanPlaySFX() ? 1 : 0;
		checkboxes[2].Frame = GameData.Instance.GetCanUseRumble() ? 1 : 0;

		selection = 0;
		canSelect = true;
		ToggleSelection(selection);

		Visible = true;
	}
	public void SetSettingsLabelVisible(bool visible) {
		settingsLabel.Visible = visible;
	}

	private void InputHandling() {
		if(Input.IsActionPressed("move_up")) {
			Input.ActionRelease("move_up");
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			ToggleSelection(selection);
			selection--;
			if(selection < 0) selection = labels.Count - 1;
			ToggleSelection(selection);
		}
		if(Input.IsActionPressed("move_down")) {
			Input.ActionRelease("move_down");
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			ToggleSelection(selection);
			selection = (selection + 1) % labels.Count;
			ToggleSelection(selection);
		}
		if(Input.IsActionPressed(GameData.Instance.GetA())) {
			Input.ActionRelease(GameData.Instance.GetA());
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			ToggleSelection(selection);
			selectTimer.Start(0.1);
			canSelect = false;
		}
		if(Input.IsActionPressed(GameData.Instance.GetB())) {
			Input.ActionRelease(GameData.Instance.GetB());
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			Back();
		}
	}

	private void ToggleSelection(int idx) {
		selectors[idx].Visible = !selectors[idx].Visible;
		if(selectors[idx].Visible) {
			labels[idx].AddThemeColorOverride("font_color", new Color("fcfcfc"));
		}
		else {
			labels[idx].AddThemeColorOverride("font_color", new Color("bcbcbc"));
		}
	}

	private void SelectOption() {
		selectOptions[selection]();
	}

	private void CheckMusic() {
		checkboxes[selection].Frame = (checkboxes[selection].Frame + 1) % 2;
		GameData.Instance.SetCanPlayMusic(!GameData.Instance.GetCanPlayMusic());
		ToggleSelection(selection);
	}

	private void CheckSFX() {
		checkboxes[selection].Frame = (checkboxes[selection].Frame + 1) % 2;
		GameData.Instance.SetCanPlaySFX(!GameData.Instance.GetCanPlaySFX());
		ToggleSelection(selection);
	}

	private void CheckRumble() {
		checkboxes[selection].Frame = (checkboxes[selection].Frame + 1) % 2;
		GameData.Instance.SetCanUseRumble(!GameData.Instance.GetCanUseRumble());
		ToggleSelection(selection);
	}

	private void Back() {
		EmitSignal(SignalName.BackSelect);
	}

	
	private void OnSelectTimerTimeout()
	{
		// make sure they can't navigate while the select is happening
		selectTimer.Stop();
		SelectOption();
		canSelect = true;
	}

	private List<Label> labels;
	private List<AnimatedSprite2D> selectors;
	private List<AnimatedSprite2D> checkboxes;
	private int selection;
	private delegate void VoidMethod();
	private List<VoidMethod> selectOptions;
	private Timer selectTimer;
	private bool canSelect;
	private Label settingsLabel;
	private AudioStreamPlayer menuTick;
}
