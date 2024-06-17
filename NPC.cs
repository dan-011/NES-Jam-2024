using General;
using Godot;
using System;
using System.Collections.Generic;
using System.Transactions;
using static General.CharacterMovement;

public partial class NPC : Area2D
{
	[Export]
	public PackedScene NPCBulletScene {get; set;}
	// Called when the node enters the scene tree for the first time.
	// We're going to want a way to control difficulty through things like speed
	public override void _Ready()
	{
		animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		fireAnimation = GetNode<AnimatedSprite2D>("FireAnimation");
		fireAnimation.Visible = false;
		fireTimer = GetNode<Timer>("FireTimer");
		List<PathFollow2D> paths = new List<PathFollow2D>();
		paths.Add(GetNode<PathFollow2D>("ZigZagPath/MobPosition"));
		paths.Add(GetNode<PathFollow2D>("IrradicPath/MobPosition"));
		paths.Add(GetNode<PathFollow2D>("StraightPath/MobPosition"));
		ResetTimer(fireTimer, 5, 3);

		int idx = Math.Abs((int)GD.Randi()) % paths.Count;

		pathPosition = paths[idx];
		if(idx == paths.Count - 1) {
			offset += (int)GD.Randi() % 200;
		}
		
		pathPosition.ProgressRatio = GD.Randf();
		//Position = new Vector2(-19, pathPosition.Position.Y);
		Position = new Vector2(-31, 100); // DEBUG
		movement = new CharacterMovement(Position, _isNPC: true);
		movement.SetGoalVel(new Vector2(100, 0));
		
		animation.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Position.X > 32) {
			Vector2 vel = movement.GetVel();
			vel.X = 2;
			movement.SetGoalVel(vel);
			movement.SetVel(vel);
		}
		deltaSum += delta;
		if(deltaSum >= 0.0167f) {
			deltaSum = 0;
			delta = 0.0167f;
			movement.Update((float)delta);
			GlobalPosition = movement.GetPos();
			UpdatePath();
			movement.SetPos(GlobalPosition);
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

	private void ResetTimer(Timer timer, int modVal, int minVal) {
		timer.WaitTime = (Math.Abs((int)GD.Randi()) % modVal) + minVal;
		timer.Start();
	}
	
		
	private void OnFireTimerTimeout()
	{
		fireAnimation.Visible = true;
		fireAnimation.Play();
		EnemyBullet bullet = NPCBulletScene.Instantiate<EnemyBullet>();
		AddChild(bullet);
		Vector2 startPos = new Vector2(GlobalPosition.X + 30, GlobalPosition.Y + 12);
		bullet.GlobalPosition = startPos;
		bullet.SetPoints(startPos, GameData.Instance.GetPlayerPos());
	}
	
		
	private void OnFireAnimationFinished()
	{
		fireAnimation.Stop();
		fireAnimation.Visible = false;
		ResetTimer(fireTimer, 1, 1);
	}



	private CharacterMovement movement;
	private PathFollow2D pathPosition;
	private AnimatedSprite2D animation;
	private AnimatedSprite2D fireAnimation;
	private int offset = 0;
	private Timer fireTimer;
	private double deltaSum = 0;
}
