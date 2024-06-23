using Godot;
using System;
using System.Collections.Generic;
using System.Xml.Schema;

enum Gadgets {
		Bomb,
		Shield,
		Reactor,
		Oil,
		Hologram
}

namespace General {
	public class CharacterMovement {
		public CharacterMovement(Vector2 pos, float maxXSpeed = 200, float maxYSpeed = 200, bool _isNPC = false) {
			startPos = pos;
			position = pos;
			maxVel.X = maxXSpeed;
			maxVel.Y = maxYSpeed;
			isNPC = _isNPC;
		}

		public Vector2 GetPos() {
			return position;
		}
		
		public void SetPos(Vector2 pos) {
			position = pos;
		}

		public Vector2 GetVel() {
			return vel;
		}

		public void SetVel(Vector2 velocity) {
			vel = velocity;
		}

		public Vector2 GetGoalVel() {
			return goalVel;
		}

		public void SetGoalVel(Vector2 goalVelocity) {
			goalVel = goalVelocity;
		}

		public void Update(float dt) {
			UpdateVel(dt);
			UpdatePos(dt);
		}

		public void MoveUp() {
			goalVel.Y = -1 * maxVel.Y;
			approachVal = 1000f;
		}

		public void MoveDown() {
			goalVel.Y = maxVel.Y;
			approachVal = 1000f;
		}

		public void Boost() {
			if(AtStart()) {
				vel.X = 300;
				isBoosting = true;
			}
		}

		public void ReleaseY() {
			goalVel.Y = 0;
			approachVal = 500f;
		}

		public void ReleaseX() {
			goalVel.X = 0;
		}
		public bool GetIsBoosting() {
			return isBoosting;
		}
		public void SetIsBoosting(bool boosting) {
			isBoosting = boosting;
		}
		// private methods
		private float Interpolate(float goal, float cur, float dt) {
			double diff = goal - cur;
			if(diff > dt) return cur + dt;
			else if(diff < (dt * -1)) return cur - dt;
			else return goal;
		}

		private void UpdatePos(float dt) {
			position.X += vel.X * dt;
			if(!isNPC) {
				if(position.X > startPos.X) position.X += vel.X * dt;
				else position.X = startPos.X;
			}
			position.Y += vel.Y * dt;
		}

		private void UpdateVel(float dt) {
			if(isNPC) {
				vel.X = Interpolate(goalVel.X, vel.X, dt*approachVal);
			}
			else {
				float xVelUpdate = gravity * dt + vel.X;
				bool wait = false;
				if((xVelUpdate < 0 || xVelUpdate == 0) && vel.X > 0) { // hit peak
					if(boostTime < 0) {
						boostTime = (long)Time.GetTicksMsec();
						wait = true;
					}
					wait = (long)Time.GetTicksMsec() - boostTime < 500;
					if(vel.X == 0f) vel.X = 0.333f;
				}
				if(!wait) {
					boostTime = -1;
					vel.X = xVelUpdate;
				}
				if(AtStart() && vel.X < 0) {
					isBoosting = false;
					boostTime = 0;
					vel.X = 0;
				}
			}
			vel.Y = Interpolate(goalVel.Y, vel.Y, dt*approachVal);
		}
		
		public bool AtStart() {
			return Math.Abs(position.X - startPos.X) < 0.00000001;
		}

		// fields
		private Vector2 position;
		private Vector2 vel;
		private Vector2 goalVel;
		private Vector2 maxVel;
		private float approachVal = 700f;
		private float gravity = -1300f;
		private Vector2 startPos;
		private long boostTime = -1;
		private bool isNPC;
		private bool isBoosting = false;
	}
	
	public sealed class GameData {
		private static GameData instance = null;
		private static readonly object padlock = new object();
		private List<uint> inventory;
		private Dictionary<string, string> controlMapping;

		GameData()
		{
			health = 100f;
			score = 0;
			isShielding = false;
			isWhiteOut = false;
			pauseAction = false;
			isReactor = false;
			inventory = new List<uint>();
			inventory.Add(0);
			inventory.Add(0);
			inventory.Add(0);
			inventory.Add(0);
			inventory.Add(0);
			selectedGadget = 0;
			totalItems = 0;
			buttonTracker = new Tuple<string, string>("", "");
			controlMapping = new Dictionary<string, string>();
			controlMapping.Add("A", "A");
			controlMapping.Add("B", "B");
			controlMapping.Add("Start", "Start");
			controlMapping.Add("Select", "Select");
			controlMapping.Add("D-Pad", "D-Pad/Joystick");
			flipAB = false;
		}

		public void DecrementHealth(float val) {
			health -= val;
		}
		public float GetHealth() {
			return health;
		}
		public void AddScore(uint val) {
			score += val;
		}
		public uint GetScore() {
			return score;
		}
		public Vector2 GetPlayerPos() {
			return playerPos;
		}
		public void SetPlayerPos(Vector2 pos) {
			playerPos = pos;
		}
		public void SetIsShielding(bool sheilding) {
			isShielding = sheilding;
		}
		public bool GetIsShielding() {
			return isShielding;
		}
		public void SetIsWhiteOut(bool whiteout) {
			isWhiteOut = whiteout;
		}
		public bool GetIsWhiteOut() {
			return isWhiteOut;
		}
		public void PauseAction() {
			pauseAction = true;
		}
		public void PlayAction() {
			pauseAction = false;
		}
		public bool GetIsPaused() {
			return pauseAction;
		}
		public bool GetIsReactor() {
			return isReactor;
		}
		public void SetIsReactor(bool reactor) {
			isReactor = reactor;
		}
		public uint GetInvetoryAmount(int gadget) {
			return inventory[gadget];
		}
		public void AddToInventory(int gadget, uint amount) {
			if(inventory[gadget] + amount > 999) totalItems += 999 - inventory[gadget];
			else totalItems += amount;
			inventory[gadget] = Math.Min(999, inventory[gadget] + amount);

		}
		public void UseItem(int gadget) {
			if(inventory[gadget] == 0) return;
			inventory[gadget]--;
			totalItems--;
			
		}
		public uint GetTotalItems() {
			return totalItems;
		}
		public bool CanUseItem(int gadget) {
			return inventory[gadget] > 0;
		}

		public void SetSelectedGadget(int select) {
			selectedGadget = select;
		}
		public int GetSelectedGadget() {
			return selectedGadget;
		}

		public bool CanPress(string menuType, string button) {
			return buttonTracker.Item1.Equals(menuType) && buttonTracker.Item2.Equals(button);
		}

		public void SetButtonTracker(string menuType, string button) {
			buttonTracker = new Tuple<string, string>(menuType, button);
		}
		public void SetControls(string gamepad = "") {
			if(gamepad.Length == 0) {
				controlMapping["A"] = "X";
				controlMapping["B"] = "Z";
				controlMapping["Start"] = "S";
				controlMapping["Select"] = "A";
				controlMapping["D-Pad"] = "Arrow Keys";
			}
			else {
				switch (gamepad) {
				case "XInput Gamepad":
				case "Xbox Series Controller":
				case "Xbox 360 Controller":
				case "Xbox One Controller":
					flipAB = true;
					controlMapping["Select"] = "RB/RT";
					break;
				case "Switch":
				case "PowerA Nintendo Switch Controller":
					controlMapping["Select"] = "R/ZR";
					break;
				case "Joy-Con (L)":
				case "Joy-Con (R)":
					controlMapping["Select"] = "R";
					controlMapping["D-Pad"] = "Joystick";
					break;
				case "Sony DualSense":
				case "PS5 Controller":
				case "PS4 Controller":
				case "Nacon Revolution Unlimited Pro Controller":
					controlMapping["A"] = "Cross";
					controlMapping["B"] = "circle";
					controlMapping["Select"] = "R1/R2";
					flipAB = true;
					break;
				default:
					if(gamepad.Contains("X")) {
						flipAB = true;
						controlMapping["Select"] = "RB/RT";
					}
					else if(gamepad.Contains("Nintendo")) {
						controlMapping["Select"] = "R/ZR";
					}
					break;
				}
			}
		}
		public string GetA() {
			return flipAB ? "B" : "A";
		}

		public string GetB() {
			return flipAB ? "A" : "B";
		}
		public string GetControllerMapping(string control) {
			return controlMapping[control];
		}

		public static GameData Instance
		{
			get
			{
				lock (padlock)
				{
					if (instance == null)
					{
						instance = new GameData();
					}
					return instance;
				}
			}
		}

		private float health;
		private uint score;
		private Vector2 playerPos;
		private bool isShielding;
		private bool isWhiteOut;
		private bool pauseAction;
		private bool isReactor;
		private int selectedGadget;
		private uint totalItems;

		private Tuple<string, string> buttonTracker;
		bool flipAB;
}
}
