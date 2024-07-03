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
		animation.Animation = "bike";
		fireAnimation = GetNode<AnimatedSprite2D>("FireAnimation");
		fireAnimation.Visible = false;
		fireTimer = GetNode<Timer>("FireTimer");
		oiledAnimation = GetNode<AnimatedSprite2D>("OiledAnimation");
		List<PathFollow2D> paths = new List<PathFollow2D>();
		string dir = "Path2/PathFollow2D";
		for(int i = 0; i < 16; i++) {
			//dir = "Path" + i.ToString() + "/PathFollow2D";
			paths.Add(GetNode<PathFollow2D>(dir));
		}
		ResetTimer(fireTimer, 5, 3);

		int idx = Math.Abs((int)GD.Randi()) % paths.Count;

		pathPosition = paths[idx];
		//GD.Print(idx);
		
		pathPosition.ProgressRatio = GD.Randf();
		//Position = new Vector2(-19, pathPosition.Position.Y);
		GlobalPosition = new Vector2(-31, pathPosition.Position.Y); // DEBUG
		movement = new CharacterMovement(Position, _isNPC: true);
		movement.SetGoalVel(new Vector2(100, 0));

		whiteOutDeathTimer = GetNode<Timer>("WhiteOutDeath");
		whiteOutDeathTimer.WaitTime = 4;
		
		animation.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(GameData.Instance.GetIsWhiteOut()) {
			HandleWhiteOut();
		}
		if(GameData.Instance.GetIsPaused()) return;
		if(Position.X > 20) {
			Vector2 vel = movement.GetVel();
			vel.X = 1;
			movement.SetGoalVel(vel);
			movement.SetVel(vel);
		}
		deltaSum += delta;
		if(deltaSum >= 0.0167f) {
			//GD.Print(movement.GetVel());
			delta = deltaSum;
			deltaSum = 0;
			//delta = 0.0167f;
			movement.Update((float)delta);
			GlobalPosition = movement.GetPos();
			if(!dying) UpdatePath();
			movement.SetPos(GlobalPosition);
		}
	}

	public bool IsOiled() {
		return oiledAnimation.Visible;
	}
	private void UpdatePath() {
		pathPosition.ProgressRatio += 0.0005f;
		GlobalPosition = new Vector2(Position.X, pathPosition.Position.Y + 100 + offset);
	}
		
	private void OnAreaEntered(Area2D area)
	{
		if(area is BombGadget) {
			Die();
		}
		else if(area is OilGadget) {
			oiledAnimation.Visible = true;
		}
	}

	private void HandleWhiteOut() {
		if(whiteOutDeathTimer.WaitTime > 3) {
			whiteOutDeathTimer.Start(2);
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
		bullet.SetPoints(startPos, GameData.Instance.GetPlayerPos(), oiledAnimation.Visible);
	}
	
		
	private void OnFireAnimationFinished()
	{
		fireAnimation.Stop();
		fireAnimation.Visible = false;
		ResetTimer(fireTimer, 1, 1);
	}

	private void Die() {
		fireTimer.Stop();
		movement.SetVel(new Vector2(100, 100));
		movement.SetGoalVel(new Vector2(100, 100));
		oiledAnimation.Visible = false;
		animation.Animation = "death";
		animation.Play();
		dying = true;
	}

	private void CheckBounds() {
		if(GlobalPosition.X > 256 || GlobalPosition.Y < 0 || GlobalPosition.Y > 224) {
			QueueFree();
		}
	}

	
	private void OnWhiteOutDeathTimerTimeout()
	{
		Die();
		whiteOutDeathTimer.WaitTime = 4;
	}

	private CharacterMovement movement;
	private PathFollow2D pathPosition;
	private AnimatedSprite2D animation;
	private AnimatedSprite2D fireAnimation;
	private AnimatedSprite2D oiledAnimation;
	private int offset = 0;
	private Timer fireTimer;
	private Timer whiteOutDeathTimer;
	private double deltaSum = 0;
	private bool dying = false;
}
