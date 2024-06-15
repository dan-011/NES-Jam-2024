using General;
using Godot;
using System;
using System.Collections.Generic;
using static General.CharacterMovement;

public partial class NPC : Area2D
{
	// Called when the node enters the scene tree for the first time.
	// We're going to want a way to control difficulty through things like speed
	public override void _Ready()
	{
		animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		List<PathFollow2D> paths = new List<PathFollow2D>();
		paths.Add(GetNode<PathFollow2D>("ZigZagPath/MobPosition"));
		paths.Add(GetNode<PathFollow2D>("IrradicPath/MobPosition"));
		paths.Add(GetNode<PathFollow2D>("StraightPath/MobPosition"));

		int idx = Math.Abs((int)GD.Randi()) % paths.Count;

		pathPosition = paths[idx];
		if(idx == paths.Count - 1) {
			offset += (int)GD.Randi() % 200;
		}
		
		pathPosition.ProgressRatio = GD.Randf();
		//Position = new Vector2(-19, pathPosition.Position.Y);
		Position = new Vector2(10, 100); // DEBUG
		movement = new CharacterMovement(Position, _isNPC: true);
		movement.SetGoalVel(new Vector2(2, 0));
		
		animation.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(delta > 0.0065) {
			if(delta > 0.007) delta = 0.007;
			movement.Update((float)delta);
			Position = movement.GetPos();
			UpdatePath();
			movement.SetPos(Position);
		}
	}
	private void UpdatePath() {
		pathPosition.ProgressRatio += 0.0005f;
		Position = new Vector2(Position.X, pathPosition.Position.Y + 100 + offset);
	}
		
	private void OnAreaEntered(Area2D area)
	{
		if(area is BombGadget) {
			GD.Print("Enemy Hit");
		}
	}

	private CharacterMovement movement;
	private PathFollow2D pathPosition;
	private AnimatedSprite2D animation;
	private int offset = 0;
}
