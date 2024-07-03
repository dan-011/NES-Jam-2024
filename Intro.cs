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
		blinkThreshold = 0.5f;

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
		titleLabel.GlobalPosition = new Vector2(131, 20);
		titleMusic = GetNode<AudioStreamPlayer>("TitleMusic");

		Start();
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
		if(deltaSum >= 1/25) {
			delta = deltaSum; // TODO adjust velocity of title
			deltaSum = 0;
			if(!finishedIntro && person.Animation.Equals("exit") && titleLabel.GlobalPosition.Y < 75) {
				Vector2 globalPos = titleLabel.GlobalPosition;
				globalPos.Y += 1;
				titleLabel.GlobalPosition = globalPos;
			}
			else if(!finishedIntro && titleLabel.GlobalPosition.Y >= 75) finishedIntro = true;
			if(shiftTitleToCenter) {
				Vector2 globalPos = titleLabel.GlobalPosition;
				Vector2 slope = new Vector2(titleCenterPos.X - globalPos.X, titleCenterPos.Y - globalPos.Y);
				float magnitude = (float)Math.Sqrt((slope.X * slope.X) + (slope.Y * slope.Y));
				globalPos.X += slope.X / magnitude;
				globalPos.Y += slope.Y / magnitude;
				titleLabel.GlobalPosition = globalPos;
				if(Math.Ceiling(globalPos.X) == titleCenterPos.X && Math.Ceiling(globalPos.Y) == titleCenterPos.Y) {
					titleLabel.GlobalPosition = titleCenterPos;
					menuAction();
					shiftTitleToCenter = false;
				}
			}
			if(shiftTitleToOrigin) {
				Vector2 globalPos = titleLabel.GlobalPosition;
				Vector2 slope = new Vector2(titleOrigin.X - globalPos.X, titleOrigin.Y - globalPos.Y);
				float magnitude = (float)Math.Sqrt((slope.X * slope.X) + (slope.Y * slope.Y));
				globalPos.X += slope.X / magnitude;
				globalPos.Y += slope.Y / magnitude;
				titleLabel.GlobalPosition = globalPos;
				if(Math.Abs(titleOrigin.X - globalPos.X) < 0.0001f && Math.Abs(titleOrigin.Y - globalPos.Y) < 0.0001f) shiftTitleToOrigin = false;
				if(Math.Floor(globalPos.X) == titleOrigin.X && Math.Floor(globalPos.Y) == titleOrigin.Y) {
					titleLabel.GlobalPosition = titleOrigin;
					mainMenu.Open();
					shiftTitleToOrigin = false;
				}
			}
			if(startingGame && titleLabel.GlobalPosition.X > -125) {
				Vector2 globalPos = titleLabel.GlobalPosition;
				globalPos.X -= 1.5f;
				titleLabel.GlobalPosition = globalPos;
			}
		}
	}

	public void Start() {
		// titleMusic.Play();
		startLabel.Visible = false;
		titleLabel.Visible = false;
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
			GD.Print("Start game");
			titleMusic.Stop();
			playBackgroundMusic = false;
			EmitSignal(SignalName.BeginGame);
		}
	}


	private void OnPersonAnimationFinished()
	{
		if(person.Animation.Equals("enter")) timer.Start(0.05);
		else {
			mainMenu.Visible = true;
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
		GD.Print("start game");
		startingGame = true;
		mainMenu.Visible = false;
		ClearShoes();
		ClearBuildings();
	}

	private void Leaderboard() {
		GD.Print("leaderboard");
		menuAction = leaderboardMenu.Open;
		GoToMenu();
	}

	private void Controls() {
		GD.Print("controls");
		menuAction = controlsMenu.Open;
		GoToMenu();
	}

	private void Credits() {
		GD.Print("credits");
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
	private AudioStreamPlayer titleMusic;
	private SettingsMenu settingsMenu;
	private bool playBackgroundMusic = false;
}
