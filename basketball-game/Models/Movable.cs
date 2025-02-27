﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using basketball_game.Utilities;

namespace basketball_game.Models
{
    public class Movable
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Acceleration;
        public Vector3 Friction;
        public int Rotation;

        public float Mass = 1;

        public void ApplyForce(Vector3 force)
        {
            // F = MA
            // A = F/M
            this.Acceleration += (force / Mass); //force accumulation
        }

        public void ApplyGravity(float scalar = 0.1f)
        {
            this.Acceleration += (new Vector3(0, -scalar * Mass, 0) / Mass);
        }

        public void ApplyFriction(float frictionCoefficient = 0.1f, float normalForce = 1.0f)
        {
            var frictionMagnitude = frictionCoefficient * normalForce;

            Friction = this.Velocity;
            Friction *= -1;
            Friction.Normalize();
            Friction *= frictionMagnitude;
            this.ApplyForce(Friction);
        }
    }
}
