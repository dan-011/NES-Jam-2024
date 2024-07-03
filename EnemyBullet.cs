using General;
using Godot;
using System;

public partial class EnemyBullet : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		movement = new CharacterMovement(Position, _isNPC: true);
		animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		animation.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(GameData.Instance.GetIsWhiteOut()) QueueFree();
		if(GameData.Instance.GetIsPaused()) return;
		deltaSum += delta;
		if(deltaSum >= 0.0167f) {
			delta = deltaSum;
			deltaSum = 0;
			//delta = 0.0167f;
			movement.Update((float)delta);
			GlobalPosition = movement.GetPos();
			movement.SetPos(GlobalPosition);
		}
		// GD.Print(movement.GetVel());
	}

	public void SetPoints(Vector2 start, Vector2 end, bool isOiled) {
		Vector2 vel;
		float scale = 100f;
		if(isOiled) {
			vel = new Vector2(scale, 0);
		}
		else {
			vel = new Vector2(end.X - start.X, end.Y - start.Y);
			float norm = (float)Math.Sqrt((vel.X * vel.X) + (vel.Y * vel.Y));
			vel.X *= scale/norm;
			vel.Y *= scale/norm;
		}

		GlobalPosition = start;
		movement.SetPos(GlobalPosition);
		movement.SetGoalVel(vel);
		movement.SetVel(vel);
	}

	private void CheckBounds() {
		if(Position.X < 0 || Position.X > 256 || Position.Y < 0 || Position.Y > 224) QueueFree();
	}	

	private void OnAreaEntered(Area2D area)
	{
		if(area is ShieldGadget || area is HologramGadget || area is ReactorGadget) {
			QueueFree();
		}
		else if(area is PlayerChase) {
			if(!GameData.Instance.GetIsShielding() && !GameData.Instance.GetIsWhiteOut()) GameData.Instance.DecrementHealth(7);
			QueueFree();
		}
	}

	private CharacterMovement movement;
	private AnimatedSprite2D animation;
	private double deltaSum = 0;
}
