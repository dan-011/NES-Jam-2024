using General;
using Godot;
using System;
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
	// Called when the node enters the scene tree for the first time.
	PlayerChase player;
	AnimatedSprite2D backgroundAnimation;
	Timer npcTimer;
	Timer bubbleTimer;
	PlayerUI playerUI;

	public override void _Ready()
	{
		GD.Seed(Time.GetTicksMsec());
		player = GetNode<PlayerChase>("PlayerChase");
		backgroundAnimation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		npcTimer = GetNode<Timer>("NPCTimer");
		bubbleTimer = GetNode<Timer>("GadgetSpawnTimer");
		ResetTimer(npcTimer, 1);
		bubbleTimer.WaitTime = 1;
		ResetTimer(bubbleTimer, 5);
		backgroundAnimation.Play();
		debugBullet = GetNode<AnimatedSprite2D>("DebugBullet");
		debugBullet.Play();
		playerUI = GetNode<PlayerUI>("PlayerUI");
	}
	private AnimatedSprite2D debugBullet;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(GameData.Instance.GetIsPaused()) {
			backgroundAnimation.Pause();
		}
		else if(!backgroundAnimation.IsPlaying()) backgroundAnimation.Play();
		debugBullet.GlobalPosition = player.GlobalPosition;
		InputHandling();
		if(Input.IsActionPressed("debug_close")) GetTree().Quit();
	}

	private void InputHandling() {
		int selectedGadget = GameData.Instance.GetSelectedGadget();
		if(!player.GetPlayerMovement().GetIsBoosting() && Input.IsActionJustPressed("gadget") && (selectedGadget == 0 || selectedGadget == 3) && GameData.Instance.CanUseItem(selectedGadget)) {
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
			//BombGadget gadget = BombGadgetScene.Instantiate<BombGadget>();
			/*gadget.Connect(SignalName.GadgetFinished, Callable.From(() => {
				GD.Print("Deleting gadget");
				RemoveChild(gadget);
			}));*/

		}
	}

	
	private void OnNPCTimerTimeout()
	{
		NPC npc = NPCScene.Instantiate<NPC>();
		AddChild(npc);
		ResetTimer(npcTimer, 10);
	}

	private void ResetTimer(Timer timer, int minLength) {
		timer.WaitTime = (GD.Randi() % 10) + minLength;
		timer.Start();
	}
	
		
	private void OnGadgetSpawnTimerTimeout()
	{
		Bubble bubble = BubbleScene.Instantiate<Bubble>();
		AddChild(bubble);
		ResetTimer(bubbleTimer, 10);
	}	
}

