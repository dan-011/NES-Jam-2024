using Godot;
using System;

public partial class Background : Node
{
	[Export]
	public PackedScene NPCScene {get; set;}
	
	[Export]
	public PackedScene BombGadgetScene {get; set;}

	[Signal]
	public delegate void GadgetFinishedEventHandler();
	// Called when the node enters the scene tree for the first time.
	PlayerChase player;
	AnimatedSprite2D backgroundAnimation;
	Timer npcTimer;

	public override void _Ready()
	{
		player = GetNode<PlayerChase>("PlayerChase");
		backgroundAnimation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		npcTimer = GetNode<Timer>("NPCTimer");
		ResetTimer();
		backgroundAnimation.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		InputHandling();
		if(Input.IsActionPressed("debug_close")) GetTree().Quit();
	}

	private void InputHandling() {
		if(Input.IsActionJustPressed("gadget")) {
			BombGadget gadget = BombGadgetScene.Instantiate<BombGadget>();
			AddChild(gadget);
			gadget.SetStartPos(new Vector2(player.Position.X-10, player.Position.Y));
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
		ResetTimer();
	}

	private void ResetTimer() {
		npcTimer.WaitTime = (GD.Randi() % 3) + 3;
		npcTimer.Start();
	}
}
