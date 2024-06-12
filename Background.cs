using Godot;
using System;

public partial class Background : Node
{
	[Export]
	public PackedScene NPCScene {get; set;}
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
		if(Input.IsActionPressed("debug_close")) GetTree().Quit();
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
