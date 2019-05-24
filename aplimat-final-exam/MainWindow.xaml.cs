using aplimat_final_exam.Models;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using aplimat_final_exam.Utilities;


namespace aplimat_final_exam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        private Vector3 mousePos = new Vector3();

        #region Initialization
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            gl.Enable(OpenGL.GL_DEPTH_TEST);

            float[] global_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] light0pos = new float[] { 0.0f, 5.0f, 10.0f, 1.0f };
            float[] light0ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light0diffuse = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            float[] lmodel_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);

            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_LIGHT0);
            gl.Enable(OpenGL.GL_LINE_SMOOTH);

            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }
        #endregion
        
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            mousePos.x = (float)position.X - (float)Width / 2.0f;
            mousePos.y = -((float)position.Y - (float)Height / 2.0f);
            Console.WriteLine(mousePos.x + " " + mousePos.y);

        }

        private void ManageKeyPress()
        {

        }
        
        
        #region Static Objects
        private  CubeMesh Ground = new CubeMesh()
        {
            Position = new Vector3(0, -35, 0),
            Scale = new Vector3(500, 0.5f, 0)
        };

        private CubeMesh Pole = new CubeMesh()
        {
            Position = new Vector3(50, -15, 0),
            Scale = new Vector3(0.5f, 20, 0)
        };

        private CubeMesh Board= new CubeMesh()
        {
            Position = new Vector3(48.5f, 3.0f, 0),
            Scale = new Vector3(0.5f, 4.5f, 0)
        };

        private Liquid Net = new Liquid(45.5f, 1.5f, 2.5f, 0.7f, 1);

        private CubeMesh RingEdge = new CubeMesh()
        {
            Position = new Vector3(42.5f, 1.0f, 0),
            Scale = new Vector3(0.5f, 0.5f, 0)
        };
        #endregion

        #region Moveable Objects
        private CubeMesh Ball = new CubeMesh()
        {
            Position = new Vector3(BallDefaultX, BallDefaultY, 0),
            Scale = new Vector3(2, 2, 0),
            Mass = 15
        };

        private CubeMesh Line = new CubeMesh();

        private List<CubeMesh> Leaves = new List<CubeMesh>();

        private Attractor Meteor = new Attractor()
        {
            Scale = new Vector3(20, 20, 0),
            Position = new Vector3(30, 500, 0),
            Mass = 50
        };
        #endregion

        #region Variables
        private bool bBallThrown = false;
        private bool bSpaceHeld = false;
        private bool bShowLine = true;
        private bool bWindy = false;
        private bool bMeteorFell = false;

        private float fIncrements = 0.2f;
        private float fXModifier = 1.0f;
        private float fYModifier = 1.0f;

        private int counter = 360; //Gets reduced
        private int maxCounter = 360; // Used for checking

        //Ball Position
        private static float BallDefaultX = -40;
        private static float BallDefaultY = -32;

        private int lastround = 0;
        private int round = 1;
        private int score = 0;
        private float fRandom = 0;
        private float fGaussian = 0;
        private float fLeaveGaussian = 0;
        private float fLeaveRandom = 0;
        private double power = 0;
        private Vector3 RightWind = new Vector3(-0.2f, 0, 0);
        private Vector3 fMeteorFalling = new Vector3(0, -10.0f, 0);



        #endregion

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            this.Title = "APLIMAT Final Exam | Basketball";
            OpenGL gl = args.OpenGL;

            // Clear The Screen And The Depth Buffer
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Move Left And Into The Screen
            gl.LoadIdentity();
            gl.Translate(0.0f, 0.0f, -100.0f);

            //Draw All Objects
            gl.Color(0.0, 0.0, 1.5);
            Ball.DrawCircle(gl);

            gl.Color(0.0, 0.5, 0.0);
            Ground.Draw(gl);
            gl.Color(0.0, 0.5, 0.5);
            Pole.Draw(gl);
            gl.Color(0.0, 0.5, 0.5);
            Board.Draw(gl);
            gl.Color(0.0, 0.5, 0.5);
            RingEdge.Draw(gl);
            Net.Draw(gl, 50, 50, 50);

            #region Meteor Fell Score When > 20
            if (score > 20)
            {
                Meteor.DrawCircle(gl);
                Meteor.ApplyForce(fMeteorFalling);
                if (Meteor.Position.y <= -30)
                {
                    Meteor.Velocity.y *= 0;
                    Meteor.Position.y = -30;
                    bMeteorFell = true;
                }
            }
            #endregion

            #region WINDY Score when > 10
            //WINDY!!!!!!!!!!!!!! --- SCORE > 10
            if (score > 10)
            {
                bWindy = true;
                CubeMesh Leave = new CubeMesh()
                {
                    Scale = new Vector3(0.5f, 0.5f, 0),
                    Mass = 0.5f
                };
                fLeaveGaussian = (float)Randomizer.Gaussian(-0, 10);
                fLeaveRandom = (float)Randomizer.Generate(0.1f, 0.5f);
                Leave.Position = new Vector3(fLeaveGaussian + 200, fLeaveGaussian, 0);
                Leave.Mass = fLeaveRandom;
                Leaves.Add(Leave);
                foreach (var leaf in Leaves)
                {
                    gl.Color(0.5, 0.7, 0.0);
                    leaf.Draw(gl);
                    leaf.ApplyGravity();
                    leaf.ApplyForce(RightWind);
                    leaf.Velocity.Clamp(-0.8f, -0.8f, 0);
                    if (leaf.HasDelicateCollidedWith(Ground))
                    {

                        leaf.Position.y = Ground.Position.y + Ground.Scale.y + leaf.Scale.y;
                        leaf.Velocity.y = (-(leaf.Velocity.y) / 2);
                        leaf.ApplyFriction();

                    }
                    else
                    {
                        leaf.ApplyGravity();
                        leaf.ApplyFriction();
                    }
                }
            }
            #endregion

            #region Line Draw 
            //Line Code
            if (bShowLine == true)
            {
                gl.Color(1.0, 0.6, 0.6);
                var LineX = Ball.Velocity.x + fXModifier;
                var LineY = Ball.Velocity.y + fYModifier;
                if (bBallThrown == true)
                {
                    Line.DrawLine(gl, Ball, (Ball.Velocity.x * 5), (Ball.Velocity.y * 5), 0);
                }
                else
                {
                    Line.DrawLine(gl, Ball, LineX * 5, LineY * 5, 0);
                }
            }
            #endregion

            #region SANITY CODE
            //You pressed Space
            if (bBallThrown == true) {
                counter--;
                if (counter > maxCounter - 2) {
                    Ball.Velocity.x = 0 + fXModifier;
                    Ball.Velocity.y = 0 + fYModifier;
                }
                Ball.Velocity.x = (float)(Math.Truncate((double)Ball.Velocity.x * 100.0) / 100.0);
                Ball.Velocity.y = (float)(Math.Truncate((double)Ball.Velocity.y * 100.0) / 100.0);
            }
            #endregion

            #region Scoring
            if (Net.Contains(Ball))
            {
                if (lastround < round)
                {
                    score++;
                    fRandom = (float)Randomizer.Generate(-10, 10);
                    fGaussian = (float)Randomizer.Gaussian(1, 2);
                }
                Ball.ApplyForce(Net.CalculateDragForce(Ball) * 0.01f);            
            }
            #endregion

            #region Reset Code
            if (counter < 0)
            {                
                Pole.Position.x += fRandom;
                Board.Position.x += fRandom; 
                RingEdge.Position.x += fRandom;
                Net.x += fRandom;
                if (fRandom < 1)
                {
                    if (fRandom > -1)
                    {
                        Board.Position.y += fGaussian;
                        Net.y += fGaussian;
                        RingEdge.Position.y += fGaussian;
                    }
                }

                if (Pole.Position.x > 80)
                {
                    Pole.Position.x = 50;
                    Board.Position.x = 48.5f;
                    RingEdge.Position.x = 42.5f;
                    Net.x = 45.5f;
                }
                counter = maxCounter;
                lastround = round;
                round++;
            }

            if (counter == maxCounter)
            {
                bSpaceHeld = false;
                bBallThrown = false;
                Ball.Velocity.x *= 0.0f;
                Ball.Velocity.y *= 0.0f;
                Ball.Position.x = BallDefaultX;
                Ball.Position.y = BallDefaultY;
                fRandom *= 0;
                fGaussian *= 0;

            }
            #endregion

            #region Controls
            // These controls only available when Ball is not yet thrown
            if (bBallThrown == false) 
            {
                //Increment Adjuster
                if (Keyboard.IsKeyDown(Key.Q)) 
                { if (fIncrements > 0.03f) fIncrements -= 0.02f; }
                if (Keyboard.IsKeyDown(Key.E))
                { fIncrements += 0.02f; }

                //Adjust power and angle using Increments Max 100
                power = (Math.Truncate(Math.Sqrt((fYModifier * fYModifier) + (fXModifier * fXModifier)) * 100.0) / 100.0);
                power *= 10;
                if (power < 300)
                {
                    if (Keyboard.IsKeyDown(Key.D))
                    { fXModifier += fIncrements; }
                    if (Keyboard.IsKeyDown(Key.W))
                    { fYModifier += fIncrements; }
                }
                if (Keyboard.IsKeyDown(Key.A))
                { fXModifier -= fIncrements; }
                if (Keyboard.IsKeyDown(Key.S))
                { fYModifier -= fIncrements; }
                
            }

            //Play Ball
            if (Keyboard.IsKeyDown(Key.Space))
            {
                if (bSpaceHeld == false)
                bBallThrown = true;
                bSpaceHeld = true;
            }
                
            //Restart
            if (Keyboard.IsKeyDown(Key.R))
            {
                if (counter < maxCounter)
                {
                    counter = -2;
                }
            }

            //Line Visibility
            if (Keyboard.IsKeyDown(Key.F))
            {
                if (bShowLine == true)
                {bShowLine = false;} else { bShowLine = true;}
            }

            //Force Random
            if (Keyboard.IsKeyDown(Key.X))
            {
                fRandom = (float)Randomizer.Generate(-10, 10);
                fGaussian = (float)Randomizer.Gaussian(1, 2);
                counter = -2;
            }

            //Add Score Cheat
            if (Keyboard.IsKeyDown(Key.C))
            {
                fRandom = (float)Randomizer.Generate(-10, 10);
                fGaussian = (float)Randomizer.Gaussian(1, 2);
                counter = -2;
                score++;
            }
            #endregion

            #region Text
            //Get Angle
            var angle = (Math.Truncate((Math.Atan(fYModifier / fXModifier) * 180 / Math.PI) * 100.0) / 100.0);


            //Below
            gl.DrawText(250, 20, 1, 0, 0, "Arial", 20, "Angle: " + angle);
            gl.DrawText(400, 5, 1, 0, 0, "Arial", 15, "Increments: " + fIncrements);
            gl.DrawText(400, 20, 1, 0, 0, "Arial", 20, "Power: " + power);
            gl.DrawText(900, 20, 1, 0, 0, "Arial", 15, "Reset in: " + counter / 2);

            //Above
            gl.DrawText(5, 600, 1, 0, 0, "Arial", 30, "Score: " + score);
            gl.DrawText(5, 630, 1, 0, 0, "Arial", 15, "Round: " + round);

            if (bWindy == true)
            {
                gl.DrawText(5, 530, 1, 0, 0, "Arial", 40, "Windy");
            }

            if (bMeteorFell == true)
            {
                gl.DrawText(5, 430, 1, 0, 0, "Arial", 50, "What's That?!");
            }

            //gl.DrawText(150, 550, 1, 0, 0, "Arial", 15, "Ball Velocity: x: " + Ball.Velocity.x + " y: " + Ball.Velocity.y);
            //gl.DrawText(350, 600, 1, 0, 0, "Arial", 15, "Ball in Air: " + fRandom);
            //gl.DrawText(700, 600, 1, 0, 0, "Arial", 15, fIncrements + " ");
            #endregion

            #region Physics and Collision
            if (Ball.HasCollidedWith(Ground))
            {
                Ball.Velocity.y = (-(Ball.Velocity.y) / 2);
                Ball.Position.y = Ground.Position.y + Ground.Scale.y + Ball.Scale.y;
                Ball.ApplyFriction();

                if (bWindy == true)
                {
                    Ball.ApplyForce(RightWind);
                }
                if (bMeteorFell == true)
                {
                    Ball.ApplyForce(Meteor.CalculateAttraction(Ball));
                }
            }
            else
            {
                Ball.ApplyGravity();
                Ball.ApplyFriction();
                if (bWindy == true)
                {
                    Ball.ApplyForce(RightWind);
                }
                if (bMeteorFell == true)
                {
                    Ball.ApplyForce(Meteor.CalculateAttraction(Ball));
                }
            }

            if (Ball.HasCollidedWith(Board))
            {
                if (Ball.Position.y > Board.Position.y + Board.Scale.y)
                {
                    Ball.Velocity.y = (-(Ball.Velocity.y) / 2);
                }
                Ball.Velocity.x = (-(Ball.Velocity.x) / 1.5f);
            }

            if (Ball.HasCollidedWith(Meteor))
            {
                if (Ball.Position.y > Meteor.Position.y + Meteor.Scale.y)
                {
                    Ball.Velocity.y = (-(Ball.Velocity.y) / 2);
                }
                Ball.Velocity.x = (-(Ball.Velocity.x) / 1.5f);
            }

            if (Ball.HasCollidedWith(RingEdge))
            {
                if (Ball.Position.y > RingEdge.Position.y + RingEdge.Scale.y)
                {
                    Ball.Velocity.y = (-(Ball.Velocity.y) / 2);
                }
                Ball.Velocity.x = (-(Ball.Velocity.x) / 1.5f);
            }

            if (Ball.HasCollidedWith(Pole))
            {
                Ball.Velocity.x = (-(Ball.Velocity.x) / 1.5f);
            }
            #endregion
        }
    }
}
