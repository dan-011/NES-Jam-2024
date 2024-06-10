using Godot;
using System;

namespace General {
	public class CharacterMovement {
		public CharacterMovement(Vector2 pos, float maxXSpeed = 200, float maxYSpeed = 200) {
			startPos = pos;
			position = pos;
			maxVel.X = maxXSpeed;
			maxVel.Y = maxYSpeed;
		}

		public Vector2 GetPos() {
			return position;
		}
		
		public void SetPos(Vector2 pos) {
			position = pos;
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
				vel.X = 200;
			}
		}

		public void ReleaseY() {
			goalVel.Y = 0;
			approachVal = 500f;
		}

		public void ReleaseX() {
			goalVel.X = 0;
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
			if(position.X > startPos.X) position.X += vel.X * dt;
			else position.X = startPos.X;
			position.Y += vel.Y * dt;
		}

		private void UpdateVel(float dt) {
			float xVelUpdate = gravity * dt + vel.X;
            if((xVelUpdate < 0 || xVelUpdate == 0) && vel.X > 0) { // hit peak
                if(boostTime == 0) {
                    boostTime = Time.GetTicksMsec();
                }
            }
            if(Time.GetTicksMsec() - boostTime > 1000) {

            }
			if((xVelUpdate < 0 || xVelUpdate == 0) && vel.X > 0) {
				if(waitTicks == 0) {
					vel.X = xVelUpdate;
				}
				else {
					vel.X = 0;
					waitTicks--;
				}
			}
			else if(xVelUpdate > 0 || (xVelUpdate < 0 && vel.X < 0)){
				vel.X = xVelUpdate;
			}
			if(AtStart() && vel.X < 0) {
                boostTime = 0;
                vel.X = 0;
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
		private float approachVal = 300f;
		private float gravity = -500f;
		private Vector2 startPos;
		private int waitTicks = 10;
        private ulong boostTime = 0;
	}
}
