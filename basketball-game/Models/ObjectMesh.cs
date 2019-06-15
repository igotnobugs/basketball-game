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

        public void DrawCube(OpenGL gl, byte r = 28, byte g = 120, byte b = 186)
        {
            gl.Color(r, g, b);
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
            gl.Color(0, 0, 0);
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

        public void DrawLine(OpenGL gl, ObjectMesh origin, Vector3 target, float MultScale = 1.0f, byte r = 28, byte g = 120, byte b = 186)
        {
            gl.Color(r, g, b);
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Vertex(origin.Position.x, origin.Position.y, origin.Position.z);
            gl.Vertex((origin.Position.x + target.x) * MultScale, (origin.Position.y + target.y) * MultScale, origin.Position.z);
            gl.End();

            UpdateMotion();
        }

        public void DrawBasketBall(OpenGL gl, int Resolution = 50, byte r = 28, byte g = 120, byte b = 186)
        {
            this.Type = "Circle";
            Resolution = (int)GameUtils.Constrain(Resolution, 10, 100);
            int iRot = this.Rotation;
            gl.Color(r, g, b);
            //Draw Simple Circle - No Rotation yet
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < Resolution; ii++)
            {             
                double angle = 2.0f * Math.PI * ii / Resolution;
                double x = Radius * Math.Cos(angle);
                double y = Radius * Math.Sin(angle);
                gl.Vertex(x + this.Position.x, y + this.Position.y, this.Position.z);
            }
            gl.End();
            //Draw Line from left tip of circle to right tip. iRot 360 = 0. Convert iRot to rads
            double iRotRads = 2.0f * Math.PI * iRot / 360;
            double xx = Radius * Math.Cos(iRotRads);
            double yy = Radius * Math.Sin(iRotRads);
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Vertex(this.Position.x - xx, this.Position.y - yy, this.Position.z);
            gl.Vertex(this.Position.x + xx, this.Position.y + yy, this.Position.z);       
            gl.End();

            // Draw Line from top tip of cricle to bottom tip. Starting iRot is 90
            double iRotRads2 = 2.0f * Math.PI * (iRot + 90) / 360;
            double xxx = Radius * Math.Cos(iRotRads2);
            double yyy = Radius * Math.Sin(iRotRads2);
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Vertex(this.Position.x + xxx, this.Position.y + yyy, this.Position.z);
            gl.Vertex(this.Position.x - xxx, this.Position.y - yyy, this.Position.z);
            gl.End();

            //Draw left curve
            gl.Begin(OpenGL.GL_LINE_STRIP);
            for (int ii = 0; ii < Resolution; ii++)
            {
                double angle = 2.0f * Math.PI * ii / (Resolution / 2);
                double x = (Radius/1.5 * Math.Cos(angle));
                double y = (Radius/1.5 * Math.Sin(angle));

                Vector3 distanceVector = new Vector3((float)(xx - x), (float)(yy - y), 0);
                var distance = distanceVector.GetLength();

                if (distance < Radius)
                {
                    gl.Vertex(x + this.Position.x - xx, y + this.Position.y - yy, this.Position.z);
                }
            }
            gl.End();

            //Draw Right Curve
            //gl.Begin(OpenGL.GL_LINE_LOOP);
            gl.Begin(OpenGL.GL_LINE_STRIP);
            for (int ii = 0; ii < Resolution; ii++)
            {
                double angle = 2.0f * Math.PI * ii / (Resolution / 2);
                double x = Radius / 1.5 * Math.Cos(angle);
                double y = Radius / 1.5 * Math.Sin(angle);

                Vector3 distanceVector = new Vector3((float)(xx + x), (float)(yy + y), 0);
                var distance = distanceVector.GetLength();

                if (distance < Radius)
                {
                    gl.Vertex(x + this.Position.x + xx, y + this.Position.y + yy, this.Position.z);
                }
            }
            gl.End();
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