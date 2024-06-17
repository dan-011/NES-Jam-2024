using Godot;
using System;

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
				vel.X = 350;
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
		
		private bool AtStart() {
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

		GameData()
		{
			health = 100f;
			score = 0;
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
}
}
