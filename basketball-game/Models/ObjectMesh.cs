using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using basketball_game.Utilities;

namespace basketball_game.Models
{
    public class ObjectMesh : Movable
    {
        public Vector3 Scale = new Vector3(0.5f, 0.5f, 0.5f);
        public float Radius = 0.5f;
        public string Type = "Cube";

        public ObjectMesh()
        {
            this.Position = new Vector3();
            this.Velocity = new Vector3();
            this.Acceleration = new Vector3();
            this.Rotation = 0;
        }

        public ObjectMesh(Vector3 initPos)
        {
            this.Position = initPos;
            this.Velocity = new Vector3();
            this.Acceleration = new Vector3();
            this.Rotation = 0;
        }

        public ObjectMesh(float x, float y, float z, int r)
        {
            this.Position = new Vector3();
            this.Velocity = new Vector3();
            this.Acceleration = new Vector3();
            this.Position.x = x;
            this.Position.y = y;
            this.Position.z = z;
            this.Rotation = r;
        }
        
        public void DrawCube(OpenGL gl)
        {

            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            //Front face
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            //Right face
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);
            //Back face
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);
            //Left face
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.End();
            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            //Top face
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.End();
            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            //Bottom face
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);
            gl.End();

            UpdateMotion();
        }

        public void DrawCircle(OpenGL gl, int Resolution = 50)
        {
            Resolution = (int)GameUtils.Constrain(Resolution, 10, 100);
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < Resolution; ii++)
            {
                double angle = 2.0f * Math.PI * ii / Resolution;
                double x = Radius * Math.Cos(angle);
                double y = Radius * Math.Sin(angle);
                gl.Vertex(x + this.Position.x, y + this.Position.y);
            }
            gl.End();

            UpdateMotion();
        }

        public void DrawLine(OpenGL gl, ObjectMesh origin, Vector3 target, float MultScale = 1.0f)
        {
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Vertex(origin.Position.x, origin.Position.y, 0);
            gl.Vertex((origin.Position.x + target.x) * MultScale, (origin.Position.y + target.y) * MultScale, 0);
            gl.End();

            UpdateMotion();
        }

        public void DrawBasketBall(OpenGL gl, int Resolution = 50)
        {
            this.Type = "Circle";
            Resolution = (int)GameUtils.Constrain(Resolution, 10, 100);
            int iRot = this.Rotation;
            //Draw Simple Circle - No Rotation yet
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < Resolution; ii++)
            {             
                double angle = 2.0f * Math.PI * ii / Resolution;
                double x = Radius * Math.Cos(angle);
                double y = Radius * Math.Sin(angle);
                gl.Vertex(x + this.Position.x, y + this.Position.y);
            }
            gl.End();
            //Draw Line from left tip of circle to right tip. iRot 360 = 0. Convert iRot to rads
            double iRotRads = 2.0f * Math.PI * iRot / 360;
            double xx = Radius * Math.Cos(iRotRads);
            double yy = Radius * Math.Sin(iRotRads);
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Vertex(this.Position.x - xx, this.Position.y - yy, 0);
            gl.Vertex(this.Position.x + xx, this.Position.y + yy, 0);
            gl.End();
            // Draw Line from top tip of cricle to bottom tip. Starting iRot is 90
            double iRotRads2 = 2.0f * Math.PI * (iRot + 90) / 360;
            double xxx = Radius * Math.Cos(iRotRads2);
            double yyy = Radius * Math.Sin(iRotRads2);
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Vertex(this.Position.x + xxx, this.Position.y + yyy, 0);
            gl.Vertex(this.Position.x - xxx, this.Position.y - yyy, 0);
            gl.End();

            /*Draw left curve
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < 50; ii++)
            {
                float theta = 2.0f * 3.1415926f * ii / 50;
                double x = (this.Scale.x / 1.5) * Math.Cos(theta);
                double y = (this.Scale.x / 1.5) * Math.Sin(theta);
                // INCOMPLETE
                x = this.Position.x - this.Scale.x + x; // INCOMPLETE
                y = this.Position.y + y;
                var length = Math.Sqrt((x + this.Scale.x * x + this.Scale.x) + (y * y));
                if (length < this.Scale.x)
                {
                    gl.Vertex(x, y);
                }
            }
            gl.End();
            //Draw right curve
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < 50; ii++)
            {
                float theta = 2.0f * 3.1415926f * ii / 50;
                double x = (this.Scale.x / 1.5) * Math.Cos(theta);
                double y = (this.Scale.x / 1.5) * Math.Sin(theta);
                gl.Vertex(this.Position.x + this.Scale.x + x, this.Position.y + y);
            }
            gl.End();
            */

            UpdateMotion();
        }

        private void UpdateMotion()
        {
            this.Velocity += this.Acceleration;
            this.Position += this.Velocity;
            this.Acceleration *= 0;
        }

        public bool HasCollidedWith(ObjectMesh target)
        {    
            if ((this.Type == "Circle") && (target.Type == "Circle"))
            {
                bool xHasNotCollided =
                    this.Position.x - this.Radius - (this.Velocity.x / 2) > target.Position.x + target.Radius ||
                    this.Position.x + this.Radius + (this.Velocity.x / 2) < target.Position.x - target.Radius;

                bool yHasNotCollided =
                    this.Position.y - this.Radius + (this.Velocity.y / 2) > target.Position.y + target.Radius ||
                    this.Position.y + this.Radius - (this.Velocity.y / 2) < target.Position.y - target.Radius;

                bool zHasNotCollided =
                    this.Position.z - this.Radius > target.Position.z + target.Scale.z ||
                    this.Position.z + this.Radius < target.Position.z - target.Scale.z;

                return !(xHasNotCollided || yHasNotCollided || zHasNotCollided);
            } else if ((this.Type == "Circle"))
            {
                bool xHasNotCollided =
                    this.Position.x - this.Radius - (this.Velocity.x / 2) > target.Position.x + target.Scale.x ||
                    this.Position.x + this.Radius + (this.Velocity.x / 2) < target.Position.x - target.Scale.x;

                bool yHasNotCollided =
                    this.Position.y - this.Radius + (this.Velocity.y / 2) > target.Position.y + target.Scale.y ||
                    this.Position.y + this.Radius - (this.Velocity.y / 2) < target.Position.y - target.Scale.y;

                bool zHasNotCollided =
                    this.Position.z - this.Radius > target.Position.z + target.Scale.z ||
                    this.Position.z + this.Radius < target.Position.z - target.Scale.z;

                return !(xHasNotCollided || yHasNotCollided || zHasNotCollided);
            } else
            {
                bool xHasNotCollided =
                    this.Position.x - this.Scale.x - (this.Velocity.x / 2) > target.Position.x + target.Scale.x ||
                    this.Position.x + this.Scale.x + (this.Velocity.x / 2) < target.Position.x - target.Scale.x;

                bool yHasNotCollided =
                    this.Position.y - this.Scale.y + (this.Velocity.y / 2) > target.Position.y + target.Scale.y ||
                    this.Position.y + this.Scale.y - (this.Velocity.y / 2) < target.Position.y - target.Scale.y;

                bool zHasNotCollided =
                    this.Position.z - this.Scale.z > target.Position.z + target.Scale.z ||
                    this.Position.z + this.Scale.z < target.Position.z - target.Scale.z;

                return !(xHasNotCollided || yHasNotCollided || zHasNotCollided);
            }
        }
    }
}