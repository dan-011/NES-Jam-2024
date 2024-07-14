using General;
using Godot;
using System;
using System.Collections.Generic;

public partial class Intro : CanvasLayer
{
	[Signal]
	public delegate void BeginGameEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cycle = GetNode<AnimatedSprite2D>("Cycle");
		person = GetNode<AnimatedSprite2D>("Person");
		lightning = GetNode<AnimatedSprite2D>("Lightning");
		shoes = GetNode<AnimatedSprite2D>("Shoes");
		timer = GetNode<Timer>("Timer");
		blinkTimer = GetNode<Timer>("BlinkTimer");
		titleLabel = GetNode<Label>("TitleLabel");
		titleLabel.AddThemeColorOverride("font_color", new Color("008888"));
		startLabel = GetNode<Label>("StartLabel");
		startLabel.Text = "PRESS " + GameData.Instance.GetControllerMapping("Start").ToUpper();
		//startLabel.GlobalPosition = new Vector2(224 / 2, startLabel.GlobalPosition.Y);

		mainMenu = GetNode<MainMenu>("MainMenu");
		controlsMenu = GetNode<ControlsMenu>("ControlsMenu");
		controlsMenu.ShiftText(new Vector2(0, 25));
		controlsMenu.SetTextColor(new Color("f8d878"));

		leaderboardMenu = GetNode<LeaderboardMenu>("LeaderboardMenu");
		creditsMenu = GetNode<CreditsMenu>("CreditsMenu");
		settingsMenu = GetNode<SettingsMenu>("SettingsMenu");

		settingsMenu.SetSettingsLabelVisible(false);

		selectOptions = new List<VoidMethod>();
		selectOptions.Add(StartGame);
		selectOptions.Add(Leaderboard);
		selectOptions.Add(Controls);
		selectOptions.Add(Credits);
		selectOptions.Add(Settings);
		selectOptions.Add(ExitGame);

		openMenu = new List<VoidMethod>();
		openMenu.Add(controlsMenu.Open);

		titleCenterPos = new Vector2(71, 4);
		titleOrigin = new Vector2(131, 75);

		GameData.Instance.SetControls(Input.GetConnectedJoypads().Count > 0 ? Input.GetJoyName(Input.GetConnectedJoypads()[0]) : "");
		titleLabelStartPos = new Vector2(131, 20);
		titleLabel.GlobalPosition = titleLabelStartPos;
		titleMusic = GetNode<AudioStreamPlayer>("TitleMusic");
		titleMovement = new CharacterMovement(titleLabel.GlobalPosition, _isNPC: true);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!Visible) return;
		deltaSum += delta;
		if(playBackgroundMusic && GameData.Instance.GetCanPlayMusic() != titleMusic.Playing) { // TODO: this might not be necessary
			if(GameData.Instance.GetCanPlayMusic()) titleMusic.Play();
			else titleMusic.Stop();
		}
		if(lightning.Frame == 4 && !titleMusic.Playing) {
			if(GameData.Instance.GetCanPlayMusic()) titleMusic.Play();
			playBackgroundMusic = true;
		}
		if(!titleLabel.Visible && lightning.Frame == 10 && person.Animation.Equals("enter")) {
			titleLabel.Visible = true;
			startLabel.Visible = true;
			blinkTimer.Start(blinkThreshold);
		}
		if(titleLabel.Visible && !mainMenu.Visible) {
			InputHandling();
		}
		if(deltaSum >= 0.0167f) {
			delta = deltaSum; // TODO adjust velocity of title
			deltaSum = 0;
			if(!finishedIntro && person.Animation.Equals("exit")) {
				if(titleMovement.GetVel().Y == 0) titleMovement.AnimateToPoint(titleLabel.GlobalPosition, titleOrigin, 65);
				titleMovement.Update((float)delta);
				titleLabel.GlobalPosition = titleMovement.GetPos();
				titleMovement.SetPos(titleLabel.GlobalPosition);
				if(titleMovement.IsAnimationDone()) finishedIntro = true;
			}
			if(shiftTitleToCenter) {
				Vector2 globalPos = titleLabel.GlobalPosition;
				if(titleMovement.GetVel().X == 0) titleMovement.AnimateToPoint(globalPos, titleCenterPos, 150);
				titleMovement.Update((float)delta);
				titleLabel.GlobalPosition = titleMovement.GetPos();
				titleMovement.SetPos(titleLabel.GlobalPosition);
				if(titleMovement.IsAnimationDone()) {
					menuAction();
					shiftTitleToCenter = false;
				}
			}
			if(shiftTitleToOrigin) {
				Vector2 globalPos = titleLabel.GlobalPosition;
				if(titleMovement.GetVel().X == 0) titleMovement.AnimateToPoint(globalPos, titleOrigin, 150);
				titleMovement.Update((float)delta);
				titleLabel.GlobalPosition = titleMovement.GetPos();
				titleMovement.SetPos(titleLabel.GlobalPosition);
				if(titleMovement.IsAnimationDone()) {
					shiftTitleToOrigin = false;
					mainMenu.Open();
				}
			}
			if(moveTitleToRight) {
				if(titleMovement.GetVel().X == 0) titleMovement.AnimateToPoint(titleLabel.GlobalPosition, new Vector2(-120, titleLabel.GlobalPosition.Y), 250); // old vel: 200
				titleMovement.Update((float)delta);
				titleLabel.GlobalPosition = titleMovement.GetPos();
				titleMovement.SetPos(titleLabel.GlobalPosition);
				if(titleMovement.IsAnimationDone()) {
					moveTitleToRight = false;
				}
			}
		}
	}

	public void Start() {
		// titleMusic.Play();
		startLabel.Visible = false;
		titleLabel.Visible = false;
		titleLabel.GlobalPosition = titleLabelStartPos;
		titleMovement.SetPos(titleLabel.GlobalPosition);
		titleMovement.SetGoalVel(new Vector2(0, 0));
		titleMovement.SetVel(new Vector2(0, 0));
		person.Frame = 0;
		lightning.Animation = "enter";
		lightning.Frame = 0;
		shoes.Frame = 0;
		cycle.Animation = "begin";
		cycle.Frame = 0;
		cycle.Play();
		person.Animation = "enter";
		person.Frame = 0;
		shoes.Animation = "enter";
		shoes.Frame = 0;
		shiftTitleToCenter = false;
		shiftTitleToOrigin = false;
		moveTitleToRight = false;
		finishedIntro = false;
		startingGame = false;
		playBackgroundMusic = false;
		blinkThreshold = 0.5f;
		mainMenu.SetSelection(0);
		Visible = true;
	}

	private void InputHandling() {
		if(Input.IsActionJustPressed("start") && person.Animation.Equals("enter") && person.Frame == person.SpriteFrames.GetFrameCount("enter")-1) {
			blinkTimer.Stop();
			blinkThreshold = 0.1f;
			blinkTimer.Start(blinkThreshold);
			timer.Start(1);
		}
	}
		

	private void OnCycleAnimationFinished()
	{
		if(cycle.Animation.Equals("begin")) {
			cycle.Animation = "cycle";
			cycle.Frame = 0;
			cycle.Play();
			timer.Start(blinkThreshold);
		}
		else if(cycle.Animation.Equals("end")) {
			titleMusic.Stop();
			playBackgroundMusic = false;
			titleLabel.Visible = false;
			EmitSignal(SignalName.BeginGame);
		}
	}


	private void OnPersonAnimationFinished()
	{
		if(person.Animation.Equals("enter")) timer.Start(0.05);
		else {
			mainMenu.Open();
		}
	}

	
	private void OnTimerTimeout()
	{
		if(person.Frame == 0) {
			timer.Stop();
			person.Play();
		}
		else if(lightning.Frame == 0) {
			timer.Stop();
			lightning.Play();
		}
		else {
			timer.Stop();
			blinkTimer.Stop();
			startLabel.Visible = false;
			//titleLabel.Visible = false;
			lightning.Stop();
			lightning.Animation = "exit";
			lightning.Frame = 0;
			lightning.Play();
			shoes.Play();
			person.Animation = "exit";
			person.Stop();
			person.Frame = 0;
			person.Play();
		}
	}

	
	private void OnBlinkTimerTimeout()
	{
		startLabel.Visible = !startLabel.Visible;
		blinkTimer.Start(blinkThreshold);
	}

	private void StartGame() {
		startingGame = true;
		moveTitleToRight = true;
		mainMenu.Visible = false;
		ClearShoes();
		ClearBuildings();
	}

	private void Leaderboard() {
		menuAction = leaderboardMenu.Open;
		GoToMenu();
	}

	private void Controls() {
		menuAction = controlsMenu.Open;
		GoToMenu();
	}

	private void Credits() {
		menuAction = creditsMenu.Open;
		GoToMenu();
	}

	private void Settings() {
		menuAction = settingsMenu.Open;
		GoToMenu();
	}

	private void ExitGame() {
		GetTree().Quit();
	}

	private void OnMainMenuSelect()
	{
		selectOptions[mainMenu.GetSelection()]();
	}
	
	private void OnReturnFromControls()
	{
		controlsMenu.Visible = false;
		ReturnFromMenu();
	}

	
	private void OnReturnFromLeaderboard()
	{
		leaderboardMenu.Visible = false;
		ReturnFromMenu();
	}
	
	
	private void OnReturnFromCredits()
	{
		creditsMenu.Visible = false;
		ReturnFromMenu();
	}

	private void OnReturnFromSettings()
	{
		settingsMenu.Visible = false;
		ReturnFromMenu();
	}

	private void GoToMenu() {
		shiftTitleToCenter = true;
		mainMenu.Visible = false;
		ClearShoes();
		ClearBuildings();
	}
	private void ClearShoes() {
		shoes.Animation = "exit";
		shoes.Frame = 0;
		shoes.Play();
	}
	private void ClearBuildings() {
		lightning.Animation = "leave";
		lightning.Frame = 0;
		lightning.Play();
	}
	private void ReturnFromMenu() {
		shoes.Animation = "exit";
		shoes.Frame = shoes.SpriteFrames.GetFrameCount("exit") - 1;
		shoes.PlayBackwards();
		lightning.Animation = "leave";
		lightning.Frame = lightning.SpriteFrames.GetFrameCount("leave") - 1;
		lightning.PlayBackwards();
		shiftTitleToOrigin = true;
	}
	
	
	private void OnCycleAnimationLooped()
	{
		if(startingGame && cycle.Animation.Equals("cycle")) {
			cycle.Stop();
			cycle.Animation = "end";
			cycle.Frame = 0;
			cycle.Play();
		}
	}
	
	
	private void OnTitleMusicFinished()
	{
		if(GameData.Instance.GetCanPlayMusic()) titleMusic.Play();
	}

	private AnimatedSprite2D cycle;
	private AnimatedSprite2D person;
	private AnimatedSprite2D lightning;
	private AnimatedSprite2D shoes;
	private Timer timer;
	private Timer blinkTimer;
	private Label titleLabel;
	private Label startLabel;
	private CharacterMovement titleMovement;
	private float blinkThreshold;
	private double deltaSum = 0;
	private MainMenu mainMenu;
	private delegate void VoidMethod();
	VoidMethod menuAction;
	private List<VoidMethod> selectOptions;
	private List<VoidMethod> openMenu;
	private bool shiftTitleToCenter = false;
	private Vector2 titleCenterPos;
	private bool shiftTitleToOrigin = false;
	private Vector2 titleOrigin;
	private bool finishedIntro = false;
	private ControlsMenu controlsMenu;
	private LeaderboardMenu leaderboardMenu;
	private CreditsMenu creditsMenu;
	private bool startingGame = false;
	private bool moveTitleToRight;
	private AudioStreamPlayer titleMusic;
	private SettingsMenu settingsMenu;
	private bool playBackgroundMusic = false;
	private Vector2 titleLabelStartPos;
}
