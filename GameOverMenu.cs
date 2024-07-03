using General;
using Godot;
using System;
using System.Collections.Generic;

public partial class GameOverMenu : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GlobalPosition = new Vector2(0, -200);
		movement = new CharacterMovement(GlobalPosition, _isNPC: true);

		menuOptions = new List<VoidMethod>();
		menuOptions.Add(EnterLeaderboard);
		menuOptions.Add(Restart);
		menuOptions.Add(MainMenu);
		menuOptions.Add(ExitGame);

		generalGameOverMenu = GetNode<GeneralGameOverMenu>("GeneralGameOverMenu");
		enterLeaderboardMenu = GetNode<EnterLeaderboardMenu>("EnterLeaderboardMenu");
	}

	public void Open() {
		GlobalPosition = new Vector2(0, -200);
		movement.SetPos(GlobalPosition);
		Visible = true;
		movement.SetGoalVel(new Vector2(0, 200));
		movement.SetVel(new Vector2(0, 200));

		generalGameOverMenu.Open();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!Visible) return;
		deltaSum += delta;
		if(deltaSum >= 0.0167f) {
			delta = deltaSum;
			deltaSum = 0;
			if(GlobalPosition.Y < 0) {
				movement.Update((float)delta);
				if(movement.GetPos().Y > 0) {
					movement.SetGoalVel(new Vector2(0, 0));
					movement.SetVel(new Vector2(0, 0));
					movement.SetPos(new Vector2(0, 0));
				}
				GlobalPosition = movement.GetPos();
			}
		}
	}

	private void OnMenuOptionSelect(int selectedIndex)
	{
		menuOptions[selectedIndex]();
	}

	private void EnterLeaderboard() {
		generalGameOverMenu.Visible = false;
		enterLeaderboardMenu.Open();
	}
	private void Restart() {

	}
	private void MainMenu() {

	}
	private void ExitGame() {
		GetTree().Quit();
	}

	
	private void OnEnterLeaderboardReturn()
	{
		enterLeaderboardMenu.Close();
		enterLeaderboardMenu.Visible = false;
		generalGameOverMenu.Open();
	}
	
	private CharacterMovement movement;
	private double deltaSum = 0;
	private delegate void VoidMethod();
	private List<VoidMethod> menuOptions;
	private GeneralGameOverMenu generalGameOverMenu;
	private EnterLeaderboardMenu enterLeaderboardMenu;
}
