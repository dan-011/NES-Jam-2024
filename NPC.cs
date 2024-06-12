using General;
using Godot;
using System;
using System.Collections.Generic;
using static General.CharacterMovement;

public partial class NPC : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		List<PathFollow2D> paths = new List<PathFollow2D>();
		paths.Add(GetNode<PathFollow2D>("ZigZagPath/MobPosition"));
		paths.Add(GetNode<PathFollow2D>("IrradicPath/MobPosition"));
		paths.Add(GetNode<PathFollow2D>("StraightPath/MobPosition"));

		int idx = (int)GD.Randi() % paths.Count;

		pathPosition = paths[idx];
		if(idx == paths.Count - 1) {
			offset += (int)GD.Randi() % 200;
		}
		GD.Print(pathPosition);
		pathPosition.ProgressRatio = GD.Randf();
		Position = new Vector2(-19, pathPosition.Position.Y);
		movement = new CharacterMovement(Position, _isNPC: true);
		movement.SetGoalVel(new Vector2(2, 0));
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

	private CharacterMovement movement;
	private PathFollow2D pathPosition;
	private int offset = 0;
}
