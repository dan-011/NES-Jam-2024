using General;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Background : Node
{
	[Export]
	public PackedScene NPCScene {get; set;}
	
	[Export]
	public PackedScene BombGadgetScene {get; set;}
	[Export]
	public PackedScene BubbleScene {get; set;}
	[Export]
	public PackedScene OilGadgetScene {get; set;}

	[Signal]
	public delegate void GadgetFinishedEventHandler();
	[Signal]
	public delegate void GameOverEventHandler();
	[Signal]
	public delegate void MainMenuFromStartEventHandler();

	// Called when the node enters the scene tree for the first time.
	PlayerChase player;
	AnimatedSprite2D backgroundAnimation;
	Timer npcTimer;
	Timer bubbleTimer;
	PlayerUI playerUI;
	StartMenu startMenu;
	Timer scoreTimer;
	Label beginLabel;
	AudioStreamPlayer mainGameMusic;
	TutorialDialog tutorialDialog;
	Timer tutorialTimer;

	public override void _Ready()
	{
		GD.Seed(Time.GetTicksMsec());
		player = GetNode<PlayerChase>("PlayerChase");
		backgroundAnimation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		npcTimer = GetNode<Timer>("NPCTimer");
		bubbleTimer = GetNode<Timer>("GadgetSpawnTimer");
		debugBullet = GetNode<AnimatedSprite2D>("DebugBullet");
		debugBullet.Play();
		playerUI = GetNode<PlayerUI>("PlayerUI");
		startMenu = GetNode<StartMenu>("StartMenu");
		playerUI.Visible = false;
		scoreTimer = GetNode<Timer>("ScoreTimer");
		beginLabel = GetNode<Label>("BeginLabel");
		mainGameMusic = GetNode<AudioStreamPlayer>("MainGameMusic");
		tutorialDialog = GetNode<TutorialDialog>("TutorialDialog");
		tutorialTimer = GetNode<Timer>("TutorialTimer");
	}
	private AnimatedSprite2D debugBullet;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(GameData.Instance.GetCanPlayMusic() != mainGameMusic.Playing) {
			if(mainGameMusic.Playing) mainGameMusic.Stop();
			else mainGameMusic.Play();
		}
		if(GameData.Instance.GetIsPaused()) {
			backgroundAnimation.Pause();
		}
		else if(!backgroundAnimation.IsPlaying()) backgroundAnimation.Play();
		debugBullet.GlobalPosition = player.GlobalPosition;
		InputHandling();
		if(Input.IsActionPressed("debug_close")) GetTree().Quit();
	}

	public void Play() {
		
		npcTimer.Stop();
		bubbleTimer.Stop();
		scoreTimer.Stop();
		mainGameMusic.Stop();
		tutorialTimer.Stop();
		backgroundAnimation.Stop();
		backgroundAnimation.Frame = 0;
		if(GameData.Instance.ShowTutorial()) {
			tutorialTimer.Start(3);
			npcTimer.Start(9);
		}
		else {
			ResetTimer(npcTimer, 1, 10);
			bubbleTimer.WaitTime = 1;
			ResetTimer(bubbleTimer, 1, 5);
		}
		backgroundAnimation.Play();

		beginLabel.Visible = true;

		GameData.Instance.ResetGameData();
		GameData.Instance.PlayAction();
		player.Start();

		playerUI.Visible = true;
		scoreTimer.Start(2);
		if(GameData.Instance.GetCanPlayMusic()) mainGameMusic.Play();
	}

	private void InputHandling() {
		int selectedGadget = GameData.Instance.GetSelectedGadget();
		scoreTimer.Paused = GameData.Instance.GetIsPaused();
		if(Input.IsActionJustPressed("start")) {
			PauseGame();
		}
		else if(!GameData.Instance.GetIsShielding() && !GameData.Instance.GetIsReactor() && !player.GetPlayerMovement().GetIsBoosting() && Input.IsActionJustPressed(GameData.Instance.GetA()) && (selectedGadget == 0 || selectedGadget == 3) && GameData.Instance.CanUseItem(selectedGadget)) {
			if(selectedGadget == 0) {
				BombGadget gadget = BombGadgetScene.Instantiate<BombGadget>();
				AddChild(gadget);
				gadget.SetStartPos(new Vector2(player.Position.X-10, player.Position.Y));
			}
			else {
				OilGadget gadget = OilGadgetScene.Instantiate<OilGadget>();
				AddChild(gadget);
				gadget.SetStartPos(new Vector2(player.Position.X-10, player.Position.Y));
			}

		}
	}

	private void PauseGame() {
		GetTree().Paused = true;
		startMenu.Open();
		// GetTree().Paused - see what this does
	}
	
	private void OnNPCTimerTimeout()
	{
		if(!GameData.Instance.GetIsWhiteOut()) {
			NPC npc = NPCScene.Instantiate<NPC>();
			AddChild(npc);
		}
		ResetTimer(npcTimer, GameData.Instance.GetLevelData().GetNPCOffset(), GameData.Instance.GetLevelData().GetNPCModVal());
	}

	private void ResetTimer(Timer timer, int minLength, int modVal) {
		timer.WaitTime = (GD.Randi() % modVal) + minLength;
		timer.Start();
	}
	
		
	private void OnGadgetSpawnTimerTimeout()
	{
		Bubble bubble = BubbleScene.Instantiate<Bubble>();
		AddChild(bubble);
		ResetTimer(bubbleTimer, GameData.Instance.GetLevelData().GetBubbleOffset(), GameData.Instance.GetLevelData().GetBubbleModVal());
	}	
	
	private void OnScoreTimerTimeout()
	{
		if(beginLabel.Visible) beginLabel.Visible = false;
		else GameData.Instance.AddScore(1);
		scoreTimer.Start(1);
		if(GameData.Instance.GetScore() % 25 == 0) {
			GameData.Instance.NextLevel();
		}
	}
	
	
	private void OnMainGameMusicFinished()
	{
		if(GameData.Instance.GetCanPlayMusic()) mainGameMusic.Play();
	}

	
	private void OnPlayerDeath()
	{
		ResetGame();
		EmitSignal(SignalName.GameOver);
	}

	private void ResetGame() {
		List<Node> toRemove = new List<Node>();
		for(int i = 0; i < GetChildCount(); i++) {
			if(GetChild(i) is NPC || GetChild(i) is Bubble || GetChild(i) is OilGadget || GetChild(i) is BombGadget) toRemove.Add(GetChild(i));
		}
		for(int i = 0; i < toRemove.Count; i++) {
			toRemove[i].QueueFree();
		}
		playerUI.Visible = false;
	}

	private void OnTutorialTimerTimeout()
	{
		tutorialTimer.Stop();
		if(!GameData.Instance.ShowTutorial()) return;
		GameData.Instance.PauseAction();
		player.ResetYVel();
		if(tutorialDialog.StartInstructions()) {
			tutorialDialog.Open();
		}
		else tutorialDialog.NextInstruction();
	}

	private void OnContinueTutorial()
	{
		tutorialDialog.Visible = false;
		GetTree().Paused = false;
		if(tutorialDialog.HasInstructionsLeft()) {
			player.IncrementTutorialInstruction();
			if(tutorialDialog.GetCurrentInstruction() == 3) {
				Bubble firstBubble = BubbleScene.Instantiate<Bubble>();
				Bubble secondBubble = BubbleScene.Instantiate<Bubble>();
				AddChild(firstBubble);
				AddChild(secondBubble);
				firstBubble.SetCustomItem(1);
				secondBubble.SetCustomItem(0);
				ResetTimer(bubbleTimer, 5, 10);
			}
		}
		else {
			GameData.Instance.HideTutorial();
			player.SetTutorialInstruction(6);
		}
		GameData.Instance.PlayAction();
	}

	private void OnSkipTutorial()
	{
		GameData.Instance.HideTutorial();
		tutorialDialog.Visible = false;
		GetTree().Paused = false;
		GameData.Instance.PlayAction();
		npcTimer.Stop();
		bubbleTimer.Stop();
		npcTimer.Start(2);
		bubbleTimer.Start(1);
	}
	
	private void OnPlayerContinueTutorial()
	{
		if(tutorialTimer.IsStopped()) tutorialTimer.Start(1.5);
		if(tutorialDialog.GetCurrentInstruction() == 5) {
			GameData.Instance.HideTutorial();
			player.IncrementTutorialInstruction();
		}
	}

	private void OnStartMenuMainMenu()
	{
		ResetGame();
		EmitSignal(SignalName.MainMenuFromStart);
	}

}
