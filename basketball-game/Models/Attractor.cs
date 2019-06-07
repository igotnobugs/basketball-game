using basketball_game.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basketball_game.Models
{
    public class Attractor : ObjectMesh
    {
        public float G = 0.05f;

        public Vector3 CalculateAttraction(Movable movable)
        {
            var force = this.Position - movable.Position;
            var distance = force.GetLength();

            distance = GameUtils.Constrain(distance, 5, 25);

            force.Normalize();

            var strength = (this.G * this.Mass * movable.Mass) / (distance * distance);
            force *= strength;
            return force;
        }
    }
}
