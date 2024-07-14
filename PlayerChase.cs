using General;
using Godot;
using System;
using System.Collections.Generic;


public partial class PlayerChase : Area2D
{
	// we'll create a parallax animation that will be constantly running in the background
	// the player can boost forward but then it pushes them back (similar to jumping)
	// otherwise changes in the x direction are locked
	// we should have an idle bobbing of the character while moving in a straight line
	
	// Called when the node enters the scene tree for the first time.
	[Signal]
	public delegate void PlayerDeathEventHandler();
	[Signal]
	public delegate void ContinueTutorialEventHandler();
	public override void _Ready()
	{
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		screenSize = GetViewportRect().Size;

		shieldGadget = GetNode<ShieldGadget>("ShieldGadget");
		reactorGadget = GetNode<ReactorGadget>("ReactorGadget");
		hologramGadget = GetNode<HologramGadget>("HologramGadget");

		itemActions = new List<VoidMethod>();
		itemActions.Add(Throw);
		itemActions.Add(Shield);
		itemActions.Add(Reactor);
		itemActions.Add(Throw);
		itemActions.Add(Hologram);

		DEBUG = GetNode<AnimatedSprite2D>("DEBUG");
		crashSound = GetNode<AudioStreamPlayer>("CrashSound");
		boostSound = GetNode<AudioStreamPlayer>("BoostSound");
	}

	public void Start() {
		Vector2 startPos = new Vector2(158, 100);
		Position = startPos;
		movement = new CharacterMovement(startPos, 400, 800);
		animation.Frame = 0;
		animation.Animation = "idle";
		animation.Frame = 0;
		canGoDown = true;
		canGoUp = true;
		isThrowing = false;
		deltaSum = 0;
		isDying = false;
		tutorialInstruction = 0;
		tutorialBubbleCount = 0;
		animation.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		DEBUG.GlobalPosition = GlobalPosition;
		if(GameData.Instance.GetIsPaused()) {
			ResetYVel();
			return;
		}
		InputHandling();
		if(GameData.Instance.GetHealth() <= 0 && !isDying) {
			Die();
		}
		deltaSum += delta;
		if(deltaSum >= 0.0167f) {
			delta = deltaSum;
			deltaSum = 0;
			//delta = 0.0167f;
			//
			movement.Update((float)delta);
			Position = movement.GetPos();
			movement.SetPos(Position);
			// Vector2 globalCenter = new Vector2(GlobalPosition.X + collisionShape.Position.X + collisionShape)
			GameData.Instance.SetPlayerPos(GlobalPosition);
			
			if(hologramGadget.Visible) GameData.Instance.SetPlayerPos(hologramGadget.GlobalPosition);
			else if(GameData.Instance.GetIsReactor()) GameData.Instance.SetPlayerPos(reactorGadget.GlobalPosition);
			if(!isDying) {	
				HandleBounds();
				AnimationHandling();
			}
		}
		if(GlobalPosition.Y > 300) {
			//crashSound.Stop();
			EmitSignal(SignalName.PlayerDeath);
			GameData.Instance.PauseAction();
		}
	}
	public CharacterMovement GetPlayerMovement() {
		return movement;
	}
	public void ResetYVel() {
		movement.ReleaseY();
	}
	public void IncrementTutorialInstruction() {
		tutorialInstruction++;
	}
	public void SetTutorialInstruction(int instruction) {
		tutorialInstruction = instruction;
	}
	private void Die() {
		//crashSound.Play();
		boostSound.Stop();
		shieldGadget.Visible = false;
		reactorGadget.Visible = false;
		hologramGadget.Visible = false;
		reactorGadget.Stop();
		shieldGadget.Stop();
		movement.SetGoalVel(new Vector2(10, 10));
		movement.SetVel(new Vector2(10, 10));
		animation.Stop();
		animation.Animation = "dying";
		animation.Play();
		isDying = true;
	}

	private void InputHandling() {
		if(animation.Animation.Equals("dying") || animation.Animation.Equals("die")) return;
		if(Input.IsActionJustReleased("move_up")) {
			
			movement.ReleaseY();
		}
		if(Input.IsActionJustReleased("move_down")) {
			movement.ReleaseY();
		}
		if(Input.IsActionPressed("move_up")) {
			if(canGoUp) movement.MoveUp();
			if(tutorialInstruction == 1) EmitSignal(SignalName.ContinueTutorial);
		}
		if(Input.IsActionPressed("move_down")) {
			if(canGoDown) movement.MoveDown();
			if(tutorialInstruction == 1) EmitSignal(SignalName.ContinueTutorial);
		}
		if(Input.IsActionJustPressed(GameData.Instance.GetB())) {
			if(movement.AtStart() && Input.GetConnectedJoypads().Count > 0 && GameData.Instance.GetCanUseRumble()) Input.StartJoyVibration(Input.GetConnectedJoypads()[0], 1, 0, 1);
			movement.Boost();
			if(GameData.Instance.GetCanPlaySFX()) boostSound.Play();
			if(tutorialInstruction == 2) EmitSignal(SignalName.ContinueTutorial);
		}
		else if(!GameData.Instance.GetIsShielding() && !GameData.Instance.GetIsReactor() && !movement.GetIsBoosting() && Input.IsActionJustPressed(GameData.Instance.GetA())) {
			int selectedGadget = GameData.Instance.GetSelectedGadget();
			if(tutorialInstruction == 5) EmitSignal(SignalName.ContinueTutorial);
			if(GameData.Instance.CanUseItem(selectedGadget) && PreventItemReuse(selectedGadget)) {
				animation.Stop();
				animation.Frame = 0;
				itemActions[selectedGadget]();
				
				GameData.Instance.UseItem(selectedGadget);
				
				//animation.Stop();
				//animation.Play("throw");
				//animation.Play("shield"); // for reactor and shield
				//Shield();
				//Reactor();
				//Hologram();
			}
		}
		if(tutorialInstruction == 4 && Input.IsActionJustPressed("select")) {
			EmitSignal(SignalName.ContinueTutorial);
		}
	}
	
	private bool PreventItemReuse(int idx) {
		return !hologramGadget.Visible || idx != 4;
	}

	private void Throw() {
		isThrowing = true;
		//animation.Frame = 0;
		if(hologramGadget.Visible) animation.Play("hologram-throw");
		else animation.Play("throw");
	}

	private void Shield() {
		if(hologramGadget.Visible) animation.Play("hologram-shield");
		else animation.Play("shield");
		GameData.Instance.SetIsShielding(true);
		shieldGadget.Visible = true;
		shieldGadget.PlayShield();
	}

	private void Reactor() {
		if(hologramGadget.Visible) animation.Play("hologram-shield");
		else animation.Play("shield");
		GameData.Instance.SetIsReactor(true);
		reactorGadget.Visible = true;
		reactorGadget.PlayReactor();
	}

	private void Hologram() {
		hologramGadget.SetPosition(GlobalPosition);
		hologramGadget.Visible = true;
	}

	private void AnimationHandling() {
		if(GameData.Instance.GetIsShielding() || GameData.Instance.GetIsReactor()) {
			if(hologramGadget.Visible) {
				if(!animation.Animation.Equals("hologram-shield")) {
					animation.Stop();
					animation.Play("holgram-shield");
				}
			}
			else {
				if(!animation.Animation.Equals("shield")) {
					animation.Stop();
					animation.Play("shield");
				}
			}
		}
		else if(movement.GetIsBoosting()) {
			if(hologramGadget.Visible) {
				if(!animation.Animation.Equals("hologram-boost")) {
					animation.Stop();
					animation.Play("hologram-boost");
				}
			}
			else {
				if(!animation.Animation.Equals("boost")) {
					animation.Stop();
					animation.Play("boost");
				}
			}
		}
		else if(!isThrowing) {
			if(hologramGadget.Visible) {
				if(movement.GetVel().Y == 0 && !animation.Animation.Equals("hologram-idle")) {
					animation.Stop();
					animation.Play("hologram-idle");
				}
				else if(movement.GetVel().Y < 0 && !animation.Animation.Equals("hologram-up")) {
					animation.Stop();
					animation.Play("hologram-up");
				}
				else if(movement.GetVel().Y > 0 && !animation.Animation.Equals("hologram-down")) {
					animation.Stop();
					animation.Play("hologram-down");
				}
			}
			else {
				if(movement.GetVel().Y == 0 && !animation.Animation.Equals("idle")) {
					animation.Stop();
					animation.Play("idle");
				}
				else if(movement.GetVel().Y < 0 && !animation.Animation.Equals("up")) {
					animation.Stop();
					animation.Play("up");
				}
				else if(movement.GetVel().Y > 0 && !animation.Animation.Equals("down")) {
					animation.Stop();
					animation.Play("down");
				}
			}
		}
	}

	private void HandleBounds() {
		Vector2 dims = collisionShape.Shape.GetRect().Size;
		if(Position.Y < (dims.Y / 2) + 5) {
			canGoUp = false;
			movement.SetVel(new Vector2(movement.GetVel().X, Math.Max(0, movement.GetVel().Y)));
		}
		else canGoUp = true;
		if(Position.Y > screenSize.Y - (dims.Y / 2) - 5) {
			canGoDown = false;
			movement.SetVel(new Vector2(movement.GetVel().X, Math.Min(movement.GetVel().Y, 0)));
		}
		else canGoDown = true;
	}
	

		
	private void OnAnimationFinished()
	{
		if(animation.Animation.Equals("throw") || animation.Animation.Equals("hologram-throw")) {
			isThrowing = false;
		}
		else if(animation.Animation.Equals("dying")) {
			movement.SetGoalVel(new Vector2(100, 100));
			animation.Stop();
			animation.Animation = "die";
			animation.Play();
		}
	}
	
		
	private void OnShieldGadgetFinished()
	{
		GameData.Instance.SetIsShielding(false);
		shieldGadget.Visible = false;
		isThrowing = false;
	}

		
	private void OnReactorGadgetFinished()
	{
		GameData.Instance.SetIsReactor(false);
		reactorGadget.Visible = false;
		isThrowing = false;
	}
	
		
	private void OnHologramGadgetFinished()
	{
		hologramGadget.Visible = false;
	}


	private void OnAreaEntered(Area2D area)
	{
		if(tutorialInstruction == 3 && area is Bubble && GameData.Instance.ShowTutorial()) {
			if(tutorialBubbleCount < 1) tutorialBubbleCount++;
			else EmitSignal(SignalName.ContinueTutorial);
		}
	}

	private CharacterMovement movement;
	private Vector2 screenSize;
	private CollisionShape2D collisionShape;
	private AnimatedSprite2D animation;
	private bool canGoDown = true;
	private bool canGoUp = true;
	private bool isThrowing = false;
	private double deltaSum = 0;
	private ShieldGadget shieldGadget;
	private ReactorGadget reactorGadget;
	private HologramGadget hologramGadget;
	private bool isDying = false;
	private delegate void VoidMethod();
	private List<VoidMethod> itemActions;
	private AnimatedSprite2D DEBUG;
	private AudioStreamPlayer crashSound;
	private AudioStreamPlayer boostSound;
	private int tutorialInstruction;
	private int tutorialBubbleCount;
}
