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
		if(delta > 0.0065) {
			if(delta > 0.007) delta = 0.007;
			movement.Update((float)delta);
			Position = movement.GetPos();
			movement.SetPos(Position);
		}
	}

	public void SetPoints(Vector2 start, Vector2 end) {
		GD.Print("Fire");
		Vector2 vel = new Vector2(end.X - start.X, end.Y - start.Y);
		float norm = (float)Math.Sqrt((vel.X * vel.X) + (vel.Y * vel.Y));
		float scale = 100f;
		vel.X *= scale/norm;
		vel.Y *= scale/norm;

		Position = start;
		movement.SetGoalVel(vel);
		movement.SetVel(vel);
	}

	private void CheckBounds() {
		if(Position.X < 0 || Position.X > 256 || Position.Y < 0 || Position.Y > 224) QueueFree();
	}	

	private void OnAreaEntered(Area2D area)
	{
		if(area is PlayerChase) {
			GameData.Instance.DecrementHealth(10);
			QueueFree();
		}
	}

	private CharacterMovement movement;
	private AnimatedSprite2D animation;
}
