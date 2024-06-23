using General;
using Godot;
using System;

public partial class GameRoot : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetTree().Paused = true;
		GameData.Instance.SetControls(Input.GetConnectedJoypads().Count > 0 ? Input.GetJoyName(Input.GetConnectedJoypads()[0]) : "");
		start = GetNode<Intro>("Intro");
		game = GetNode<Background>("Background");
		opening = GetNode<GameOpening>("GameOpening");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
		
	private void OnBeginGame()
	{
		start.Visible = false;
		opening.Visible = true;
		opening.Play();
	}

	

	private void OnGameOpeningEnded()
	{
		opening.Visible = false;
		game.Play();
		GetTree().Paused = false;
	}
	

	private Intro start;
	private Background game;
	private GameOpening opening;
}
