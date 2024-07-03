using General;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class LeaderboardMenu : CanvasLayer
{
	[Signal]
	public delegate void SelectBackEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		backLabel = GetNode<Label>("BackLabel");
		backSelector = GetNode<AnimatedSprite2D>("BackSelector");
		selectTimer = GetNode<Timer>("SelectTimer");

		rankings = new List<Label>();
		rankings.Add(GetNode<Label>("FirstLabel"));
		rankings.Add(GetNode<Label>("SecondLabel"));
		rankings.Add(GetNode<Label>("ThirdLabel"));
		rankings.Add(GetNode<Label>("FourthLabel"));
		rankings.Add(GetNode<Label>("FifthLabel"));

		scores = new List<Label>();
		scores.Add(GetNode<Label>("FirstScore"));
		scores.Add(GetNode<Label>("SecondScore"));
		scores.Add(GetNode<Label>("ThirdScore"));
		scores.Add(GetNode<Label>("FourthScore"));
		scores.Add(GetNode<Label>("FifthScore"));

		SetScores();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Visible) {
			InputHandling();
		}
	}

	public void Open() {
		Visible = true;
		backSelector.Visible = true;
		backLabel.AddThemeColorOverride("font_color", new Color("fcfcfc"));
		SetScores();
	}

	public void CompressedView() {
		rankings[0].GlobalPosition = new Vector2(50, 70);
		scores[0].GlobalPosition = new Vector2(166, 70);
		rankings[1].GlobalPosition = new Vector2(50, 85);
		scores[1].GlobalPosition = new Vector2(166, 84);
		rankings[2].GlobalPosition = new Vector2(50, 100);
		scores[2].GlobalPosition = new Vector2(166, 98);
		rankings[3].GlobalPosition = new Vector2(50, 115);
		scores[3].GlobalPosition = new Vector2(166, 112);
		rankings[4].GlobalPosition = new Vector2(50, 130);
		scores[4].GlobalPosition = new Vector2(166, 126);

		backLabel.GlobalPosition = new Vector2(180, 173);
		backSelector.GlobalPosition = new Vector2(175, 177);
	}

	private void InputHandling() {
		if(Input.IsActionPressed(GameData.Instance.GetA())) {
			GoBack(GameData.Instance.GetA());
		}
		if(Input.IsActionJustPressed(GameData.Instance.GetB())) {
			GoBack(GameData.Instance.GetB());
		}
	}

	private void GoBack(string action) {
		Input.ActionRelease(action);
		backSelector.Visible = false;
		backLabel.AddThemeColorOverride("font_color", new Color("bcbcbc"));
		selectTimer.Start(0.1);
	}
	
	private void OnSelectTimerTimeout()
	{
		selectTimer.Stop();
		EmitSignal(SignalName.SelectBack);
	}

	public void SetScores() {
		for(int i = 0; i < scores.Count; i++) {
			rankings[i].Visible = false;
			scores[i].Visible = false;
		}
		StreamReader fileStream = new StreamReader("Leaderboard.txt");
		string line;
		int rank = 0;
		while(rank < scores.Count && (line = fileStream.ReadLine()) != null) {
			string[] words = line.Split('}');
			if(words.Length == 0) break;
			rankings[rank].Text = (rank+1).ToString() + ".";
			rankings[rank].Text += "  " + words[0];
			scores[rank].Text = words[words.Length - 1];
			rankings[rank].Visible = true;
			scores[rank].Visible = true;
			rank++;
		}
		fileStream.Close();
	}

	private Label backLabel;
	private AnimatedSprite2D backSelector;
	private Timer selectTimer;
	private List<Label> rankings;
	private List<Label> scores;
}
