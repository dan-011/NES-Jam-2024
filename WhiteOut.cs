using General;
using Godot;
using System;

public partial class WhiteOut : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
		
		color = GetNode<ColorRect>("ColorRect");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!color.Visible) HandleWhiteOut();
	}

	private void HandleWhiteOut() {
		if(GameData.Instance.GetIsWhiteOut()) {
			color.Visible = true;
			timer.Start(3);
		}
	}
	
	private void OnTimerTimeout()
	{
		timer.Stop();
		color.Visible = false;
		GameData.Instance.PlayAction();
		GameData.Instance.SetIsWhiteOut(false);
	}


	private Timer timer;
	private ColorRect color;
}
