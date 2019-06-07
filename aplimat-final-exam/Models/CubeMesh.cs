using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aplimat_final_exam.Models
{
    public class CubeMesh : Movable
    {
        public Vector3 Scale = new Vector3(0.5f, 0.5f, 0.5f);

        public CubeMesh()
        {
            this.Position = new Vector3();
            this.Velocity = new Vector3();
            this.Acceleration = new Vector3();
        }
        public CubeMesh(Vector3 initPos)
        {
            this.Position = initPos;
            this.Velocity = new Vector3();
            this.Acceleration = new Vector3();

        }

        public CubeMesh(float x, float y, float z)
        {
            this.Position = new Vector3();
            this.Velocity = new Vector3();
            this.Acceleration = new Vector3();
            this.Position.x = x;
            this.Position.y = y;
            this.Position.z = z;
        }

        public void Draw(OpenGL gl)
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

        public void DrawCircle(OpenGL gl) 
        {
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < 100; ii++)   
            {
                float theta = 2.0f * 3.1415926f * ii / 100;
                double x = this.Scale.x * Math.Cos(theta);
                double y = this.Scale.x * Math.Sin(theta);
                gl.Vertex(x + this.Position.x, y + this.Position.y);
            }
            gl.End();

            UpdateMotion();
        }

        public void DrawLine(OpenGL gl, CubeMesh origin, float fLinePointXMod, float fLinePointYMod, float LineScale = 0)
        {
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Vertex(origin.Position.x, origin.Position.y, 0);
            var fLineEndPointX = origin.Position.x + LineScale;
            var fLineEndPointY = origin.Position.y + LineScale;
            gl.Vertex(fLineEndPointX + fLinePointXMod, fLineEndPointY + fLinePointYMod, 0);
            gl.End();
            UpdateMotion();
        }

        public void DrawBasketBall(OpenGL gl)
        {
            //Draw Simple Circle
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < 100; ii++)
            {
                float theta = 2.0f * 3.1415926f * ii / 100;
                double x = this.Scale.x * Math.Cos(theta);
                double y = this.Scale.x * Math.Sin(theta);
                gl.Vertex(x + this.Position.x, y + this.Position.y);
            }
            gl.End();
            //Draw Line from left tip of circle to right tip.
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y, 0);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y, 0);
            gl.End();
            // Draw Line from top tip of cricle to bottom tip.
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Vertex(this.Position.x, this.Position.y + this.Scale.x, 0);
            gl.Vertex(this.Position.x, this.Position.y - this.Scale.x, 0);
            gl.End();
            //Draw left curve
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


            UpdateMotion();
        }

        private void UpdateMotion()
        {
            this.Velocity += this.Acceleration;
            this.Position += this.Velocity;
            this.Acceleration *= 0;
        }

        public bool HasCollidedWith(CubeMesh target)
        {
            bool xHasNotCollided =
                this.Position.x - this.Scale.x - (this.Velocity.x / 2) > target.Position.x + target.Scale.x ||
                this.Position.x + this.Scale.x + (this.Velocity.x / 2) < target.Position.x - target.Scale.x;
                //this.Position.x - this.Scale.x > target.Position.x + target.Scale.x ||
                //this.Position.x + this.Scale.x < target.Position.x - target.Scale.x;

            bool yHasNotCollided =
                this.Position.y - this.Scale.y > target.Position.y + target.Scale.y ||
                this.Position.y + this.Scale.y < target.Position.y - target.Scale.y;

            bool zHasNotCollided =
                this.Position.z - this.Scale.z > target.Position.z + target.Scale.z ||
                this.Position.z + this.Scale.z < target.Position.z - target.Scale.z;

            return !(xHasNotCollided || yHasNotCollided || zHasNotCollided);
        }

        //Collision for Small Leaves
        public bool HasDelicateCollidedWith(CubeMesh target)
        {
            bool xHasNotCollided =
                this.Position.x - this.Scale.x - (this.Velocity.x / 2) > target.Position.x + target.Scale.x ||
                this.Position.x + this.Scale.x + (this.Velocity.x / 2) < target.Position.x - target.Scale.x;
            //this.Position.x - this.Scale.x > target.Position.x + target.Scale.x ||
            //this.Position.x + this.Scale.x < target.Position.x - target.Scale.x;

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
