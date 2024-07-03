using General;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class EnterLeaderboardMenu : CanvasLayer
{
	[Signal]
	public delegate void ReturnEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		keyboard = GetNode<KeyboardComponent>("KeyboardComponent");
		leaderboardMenu = GetNode<LeaderboardMenu>("LeaderboardMenu");
		leaderboardMenu.CompressedView();
		rankLabel = GetNode<Label>("LeaderboardMenu/RankLabel");
		rankLabel.Text = "";
	}

	public void Open() {
		if(enteredLeaderboard) leaderboardMenu.Open();
		else keyboard.Open();
	}

	public void Close() {
		leaderboardMenu.Visible = false;
		keyboard.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	
	private void OnKeyboardSubmit(string text)
	{
		StreamReader readStream = new StreamReader("Leaderboard.txt");
		string line;
		List<Tuple<string, int>> scores = new List<Tuple<string, int>>();
		while((line = readStream.ReadLine()) != null) {
			string[] words = line.Split('}');
			if(words.Length == 0) break;
			scores.Add(new Tuple<string, int>(words[0], words[1].ToInt()));
		}
		readStream.Close();
		scores.Add(new Tuple<string, int>(text, (int)GameData.Instance.GetScore())); // TODO: potential for negatives
		scores = scores.OrderByDescending(tuple => tuple.Item2).ToList();

		int rank = 0;

		StreamWriter writeStream = new StreamWriter("Leaderboard.txt", false);
		foreach(var score in scores) {
			rank++;
			if(rank > 0 && score.Item1.Equals(text) && score.Item2 == GameData.Instance.GetScore()) {
				rankLabel.Text = "Rank:  " + rank.ToString();
				rank = -1;
			}
			writeStream.Write(score.Item1 + '}' + score.Item2 + '\n');
		}
		writeStream.Close();

		enteredLeaderboard = true;

		keyboard.Visible = false;
		leaderboardMenu.Open();
	}

	private void OnReturnFromLeaderboard()
	{
		EmitSignal(SignalName.Return);
	}

	private KeyboardComponent keyboard;
	private LeaderboardMenu leaderboardMenu;
	private bool enteredLeaderboard = false;
	private Label rankLabel;
}
