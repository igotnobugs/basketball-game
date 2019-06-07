using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aplimat_final_exam.Models
{
    public class Liquid
    {
        public float x, y;
        public float width, depth;
        public float drag;

        public Liquid(float x, float y, float width, float height, float drag)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.depth = height;
            this.drag = drag;
        }

        public void Draw(OpenGL gl, byte r = 28, byte g = 120, byte b = 186)
        {
            gl.Color(r, g, b);
            gl.Begin(OpenGL.GL_POLYGON);
            gl.Vertex(x - width, y, 0);
            gl.Vertex(x + width, y, 0);
            gl.Vertex(x + width, y - depth, 0);
            gl.Vertex(x - width, y - depth, 0);
            gl.End();
        }


        /**
         * Checks if the position of a movable is inside
         * the actual liquid
         */
        public bool Contains(Movable movable)
        {
            var p = movable.Position;
            //return p.x > this.x - this.width &&
            //    p.x < this.x + this.width &&
            //    p.y < this.y;

            return p.x > this.x - this.width &&
                p.x < this.x + this.width &&
                p.y > this.y - this.depth &&
                p.y < this.y + this.depth;
        }

        public Vector3 CalculateDragForce(Movable movable)
        {
            // Magnitude is coefficient * speed squared
            var speed = movable.Velocity.GetLength();
            var dragMagnitude = this.drag * speed * speed;

            // Direction is inverse of velocity
            var dragForce = movable.Velocity;
            dragForce *= -1;

            // Scale according to magnitude
            dragForce.Normalize();
            dragForce *= dragMagnitude;

            return dragForce;
        }
    }
}
