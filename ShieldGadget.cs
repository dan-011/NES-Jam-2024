using Godot;
using System;

public partial class ShieldGadget : Area2D
{
	[Signal]
	public delegate void SheildFinishedEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animation.Frame = 0;
		animation.Animation = "building";

		timer = GetNode<Timer>("Timer");
		timer.WaitTime = 5;

		collision = GetNode<CollisionShape2D>("CollisionShape2D");
		collision.Disabled = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PlayShield() {
		animation.Animation = "building";
		animation.Play();
	}
	
	private void OnSheildAnimationFinished()
	{
		if(animation.Animation.Equals("collapsing")) {
			animation.Animation = "building";
			animation.Frame = 0;
			timer.WaitTime = 5;
			EmitSignal(SignalName.SheildFinished);
		}
		else {
			collision.Disabled = false;
			timer.Start(5);
		}
	}
	
	private void OnTimerTimeout()
	{
		animation.Animation = "collapsing";
		collision.Disabled = true;
		animation.Play();
	}


	private AnimatedSprite2D animation;
	private Timer timer;
	private CollisionShape2D collision;
}
