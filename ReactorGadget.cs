using General;
using Godot;
using System;

public partial class ReactorGadget : Area2D
{
	[Signal]
	public delegate void ReactorFinishedEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		bulletCount = 0;
		animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animation.Frame = 0;
		animation.Animation = "building";

		collision = GetNode<CollisionShape2D>("CollisionShape2D");
		collision.Disabled = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Visible) CheckBullets();
	}

	public void PlayReactor() {
		collision.Disabled = false;
		animation.Play();
	}
	
	
	private void CheckBullets() {
		if(bulletCount == maxBullets && animation.Frame == animation.SpriteFrames.GetFrameCount("building") - 1) {
			bulletCount = 0;
			animation.Stop();
			animation.Play("releasing");
		}
	}

	private void OnAnimationFinished()
	{
		if(animation.Animation.Equals("releasing")) {
			collision.Disabled = true;
			GameData.Instance.PauseAction();
			GameData.Instance.SetIsWhiteOut(true);

			animation.Animation = "building";

			EmitSignal(SignalName.ReactorFinished);
		}
	}
	
	private void OnAreaEntered(Area2D area)
	{
		if(Visible && area is EnemyBullet) {
			bulletCount++;
		}
	}

	private uint bulletCount;
	private uint maxBullets = 10;
	private AnimatedSprite2D animation;
	private CollisionShape2D collision;
}
