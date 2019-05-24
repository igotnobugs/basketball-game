using aplimat_final_exam.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aplimat_final_exam.Models
{
    public class Attractor : CubeMesh
    {
        public float G = 0.05f;

        public Vector3 CalculateAttraction(Movable movable)
        {
            var force = this.Position - movable.Position;
            var distance = force.GetLength();

            distance = AplimatUtils.Constrain(distance, 5, 25);

            force.Normalize();

            var strength = (this.G * this.Mass * movable.Mass) / (distance * distance);
            force *= strength;
            return force;
        }
    }
}
