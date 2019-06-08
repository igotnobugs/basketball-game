using basketball_game.Models;
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
using basketball_game.Utilities;


namespace basketball_game
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
            //gl.Disable(OpenGL.GL_LIGHTING);
            //gl.Enable(OpenGL.GL_LIGHT0);
            gl.Enable(OpenGL.GL_LINE_SMOOTH);

            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }
        #endregion
        
        
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            mousePos.x = (float)position.X - (float)Width / 2.0f;
            mousePos.y = -((float)position.Y - (float)Height / 2.0f);
            //Console.WriteLine(mousePos.x + " " + mousePos.y);
        }
        

        private void ManageKeyPress()
        {

        }
        
        
        #region Static Objects
        private  ObjectMesh Ground = new ObjectMesh()
        {
            Position = new Vector3(0, -35, 0),
            Scale = new Vector3(500, 0.5f, 0)
        };

        private ObjectMesh Pole = new ObjectMesh()
        {
            Position = new Vector3(50, -15, 0),
            Scale = new Vector3(0.5f, 20, 0)
        };

        private static ObjectMesh Board= new ObjectMesh()
        {
            Position = new Vector3(48.5f, 3.0f, 0),
            Scale = new Vector3(0.5f, 4.5f, 0)
        };

        private Liquid RimNet = new Liquid(Board.Position.x - 3, 1.5f, 2.5f, 1.0f, 1);
        private Liquid CenNet = new Liquid(Board.Position.x - 3, 0.5f, 2.5f, 2.0f, 1);

        private ObjectMesh RingEdge = new ObjectMesh()
        {
            Position = new Vector3(Board.Position.x - 6, 1.0f, 0),
            Scale = new Vector3(0.5f, 0.5f, 0)
        };
        #endregion

        #region Moveable Objects
        private ObjectMesh Ball = new ObjectMesh()
        {
            Position = BallDefaultPos,
            Radius = 2.0f,
            Mass = 15,
            Rotation = 60
        };



        private ObjectMesh Line = new ObjectMesh();

        private List<ObjectMesh> Leaves = new List<ObjectMesh>();

        private Attractor Meteor = new Attractor()
        {
            Position = new Vector3(30, 500, 0),
            Radius = 20.0f,
            Mass = 50
        };
        #endregion

        #region Variables
        //Game
        private bool bBallThrown = false;
        private bool bSpaceHeld = false;
        private bool bWindy = false;
        private bool bMeteorFell = false;
        private bool bScored = false;
        private int iCounter = 60; //Gets reduced
        private int iMaxCounter = 60; // Used for checking
        private int iRound = 1;
        private int iScore = 0;
        private float fRandom = 0;
        private float fGaussian = 0;
        private float fLeaveGaussian = 0;
        private float fLeaveRandom = 0;
        private double dPower = 0;
        private Vector3 vRightWind = new Vector3(-0.2f, 0, 0);
        private Vector3 vMeteorFalling = new Vector3(0, -10.0f, 0);

        //Ball Position
        private static Vector3 BallDefaultPos = new Vector3(-40.0f, -25.0f, 0);

        //Line and Aiming Variables 
        private bool bShowLine = true;
        private bool bLineButtonPressed = false;
        //private bool bShowSimulationLine = false;
        private float fLineLength;
        private Vector3 vModLine = new Vector3(3.0f, 3.0f, 0);
        private Vector3 fModifier = new Vector3(1.0f, 1.0f, 0);
        private float fIncrements = 0.7f;
        private float fMinIncrements = 0.3f;
        private float fMaxIncrements = 1.5f;
        private double fAimAngle;



        #endregion

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            this.Title = "APLIMAT Final Exam | Basketball Game";
            #region OpenGL Draw 
            OpenGL gl = args.OpenGL;

            // Clear The Screen And The Depth Buffer
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Move screen back
            gl.LoadIdentity();
            gl.Translate(0.0f, 0.0f, -100.0f);

            //Draw All Objects
            gl.Color(0.0, 0.0, 1.5);
            Ball.DrawBasketBall(gl, 60);
            gl.Color(0.0, 0.5, 0.0);
            Ground.DrawCube(gl);
            gl.Color(0.0, 0.5, 0.5);
            Pole.DrawCube(gl);
            gl.Color(0.0, 0.5, 0.5);
            Board.DrawCube(gl);
            gl.Color(0.0, 0.5, 0.5);
            RingEdge.DrawCube(gl);
            RimNet.Draw(gl, 50, 50, 50);
            CenNet.Draw(gl, 40, 40, 40);
            
            //Aiming Line Draw
            if (bShowLine == true)
            {
                gl.Color(1.0, 0.6, 0.6);
                if (bBallThrown == true)
                {
                    Line.DrawLine(gl, Ball, Ball.Velocity, 1);
                }
                else
                {
                    Line.DrawLine(gl, Ball, vModLine, 1);
                }
            }

            //
            #endregion

            #region WINDY Score when > 10
            //WINDY!!!!!!!!!!!!!! --- SCORE > 10
            if (iScore >= 10)
            {
                bWindy = true;
                ObjectMesh Leave = new ObjectMesh()
                {
                    Scale = new Vector3(0.5f, 0.5f, 0),
                    Mass = 0.3f
                };
                fLeaveGaussian = (float)Randomizer.Gaussian(-0, 10);
                fLeaveRandom = (float)Randomizer.Generate(0.1f, 0.5f);
                Leave.Position = new Vector3(fLeaveGaussian + 200, fLeaveGaussian, 0);
                Leave.Mass = fLeaveRandom;
                Leaves.Add(Leave);
                foreach (var leaf in Leaves)
                {
                    gl.Color(0.5, 0.7, 0.0);
                    leaf.DrawCube(gl);
                    leaf.ApplyGravity();
                    leaf.ApplyForce(vRightWind);
                    leaf.Velocity.Clamp(-0.2f, -0.2f, 0);
                    if (leaf.HasCollidedWith(Ground))
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

            #region Meteor Fell Score When > 20
            if (iScore >= 20)
            {
                Meteor.DrawCircle(gl);
                Meteor.ApplyForce(vMeteorFalling);
                if (Meteor.Position.y <= -30)
                {
                    Meteor.Velocity.y *= 0;
                    Meteor.Position.y = -30;
                    bMeteorFell = true;
                }
            }
            #endregion        

            #region Ball Thrown Code
            //You pressed Space
            if (bBallThrown == true) {
                iCounter--;
                if (iCounter > iMaxCounter - 2) {
                    Ball.Velocity = vModLine / 1.5f;              
                }
                Ball.Rotation += 10;
            }
            #endregion

            #region Scoring
            if (CenNet.Contains(Ball)) 
            {
                Console.WriteLine("Ball is in CenNet...");
                if (!bScored)
                {
                    iScore++;
                    fRandom = (float)Randomizer.Generate(-10, 10);
                    fGaussian = (float)Randomizer.Gaussian(1, 2);
                    Console.WriteLine("Player Scores!");
                    bScored = true;
                }
                Ball.ApplyForce(RimNet.CalculateDragForce(Ball) * 1);
                Ball.ApplyForce(CenNet.CalculateDragForce(Ball) * 2);          
            }
            #endregion

            #region Reset Code
            if (iCounter < 0)
            {
                Pole.Position.x += fRandom;
                Board.Position.x += fRandom; 
                RingEdge.Position.x += fRandom;
                RimNet.x += fRandom;
                CenNet.x += fRandom;

                if ((fRandom < 1) && (fRandom > -1))
                {
                    Board.Position.y += fGaussian;
                    RimNet.y += fGaussian;
                    CenNet.y += fGaussian;
                    RingEdge.Position.y += fGaussian;       
                }

                if ((Pole.Position.x > 70) || (Pole.Position.x < 30))
                {
                    Pole.Position.x = 50;
                    Board.Position.x = 48.5f;
                    RingEdge.Position.x = 42.5f;
                    RimNet.x = 45.5f;
                    CenNet.x = 45.5f;
                }
                iCounter = iMaxCounter;
                bScored = false;
                iRound++;
            }

            if (iCounter == iMaxCounter)
            {
                bSpaceHeld = false;
                bBallThrown = false;
                Ball.Velocity.x *= 0.0f;
                Ball.Velocity.y *= 0.0f;
                Ball.Position = BallDefaultPos;
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
                { fIncrements -= 0.1f; }
                else if (Keyboard.IsKeyDown(Key.E))
                { fIncrements += 0.2f; }
                fIncrements = GameUtils.Constrain(fIncrements, fMinIncrements, fMaxIncrements);

                //Adjust Power and Angle modified by Increments
                fLineLength = vModLine.GetLength();
                dPower = (Math.Truncate(fLineLength * 100.0) / 100.0);
                dPower *= 10;
                if (Keyboard.IsKeyDown(Key.W) && (dPower < 100))
                {

                        fAimAngle = Math.Atan((vModLine.y) / (vModLine.x) );
                        vModLine.x += fIncrements * (float)Math.Cos(fAimAngle);
                        vModLine.y += fIncrements * (float)Math.Sin(fAimAngle);
                }
                if (Keyboard.IsKeyDown(Key.S) && (dPower > 10))
                {
                    fAimAngle = Math.Atan((vModLine.y) / (vModLine.x));
                    vModLine.x -= fIncrements * (float)Math.Cos(fAimAngle);
                    vModLine.y -= fIncrements * (float)Math.Sin(fAimAngle);
                }
                if (Keyboard.IsKeyDown(Key.D))
                {
                    fAimAngle = Math.Atan(vModLine.y / vModLine.x);
                    fAimAngle -= (Math.PI / 180) + ((Math.PI / 90) * fIncrements);
                    vModLine.x = fLineLength * (float)Math.Cos(fAimAngle);
                    vModLine.y = fLineLength * (float)Math.Sin(fAimAngle);
                }
                if (Keyboard.IsKeyDown(Key.A))
                {
                    fAimAngle = Math.Atan(vModLine.y / vModLine.x);
                    fAimAngle += (Math.PI/180) + ((Math.PI / 90) * fIncrements);
                    vModLine.x = fLineLength * (float)Math.Cos(fAimAngle);
                    vModLine.y = fLineLength * (float)Math.Sin(fAimAngle);
                }
            }

            //Play Ball
            if (Keyboard.IsKeyDown(Key.Space))
            {
                if (bSpaceHeld == false)
                bBallThrown = true;
                bSpaceHeld = true;
            }
                
            //Restart
            if (Keyboard.IsKeyDown(Key.R) && (iCounter < iMaxCounter))
            {
                 iCounter = -2;
            }

            //Line Visibility
            if (Keyboard.IsKeyDown(Key.F) && !bLineButtonPressed)
            {
                if (bShowLine == true)
                {
                    bShowLine = false;
                    bLineButtonPressed = true;
                }
                else
                {
                    bShowLine = true;
                    bLineButtonPressed = true;
                }    
            }
            if (Keyboard.IsKeyUp(Key.F))
            {
                bLineButtonPressed = false;
            }

            //Force Random
            if (Keyboard.IsKeyDown(Key.X))
            {
                fRandom = (float)Randomizer.Generate(-10, 10);
                fGaussian = (float)Randomizer.Gaussian(1, 2);
                iCounter = -2;
            }

            //Add Score Cheat
            if (Keyboard.IsKeyDown(Key.C))
            {
                fRandom = (float)Randomizer.Generate(-10, 10);
                fGaussian = (float)Randomizer.Gaussian(1, 2);
                iCounter = -2;
                iScore++;
            }
            #endregion

            #region Text
            //Get Angle
            var angle = (Math.Truncate((Math.Atan(vModLine.y / vModLine.x) * 180 / Math.PI) * 100.0) / 100.0);

            //Below
            gl.DrawText(250, 20, 1, 0, 0, "Arial", 20, "Angle: " + angle);
            gl.DrawText(400, 5, 1, 0, 0, "Arial", 15, "Increments: " + fIncrements);
            gl.DrawText(400, 20, 1, 0, 0, "Arial", 20, "Power: " + dPower);
            gl.DrawText(900, 20, 1, 0, 0, "Arial", 15, "Reset in: " + iCounter);

            //Above
            gl.DrawText(5, 600, 1, 0, 0, "Arial", 30, "Score: " + iScore);
            gl.DrawText(5, 630, 1, 0, 0, "Arial", 15, "Round: " + iRound);
            gl.DrawText(5, 660, 1, 0, 0, "Arial", 15, "Ball Position " + Ball.Position);
            gl.DrawText(5, 690, 1, 0, 0, "Arial", 15, "Distance : " + GameUtils.GetDistanceBetween(Meteor, Ball));
            gl.DrawText(5, 720, 1, 0, 0, "Arial", 15, "Distance : " + GameUtils.GetAngleBetween(Meteor, Ball));


            //LENGTH OF LINE
            //gl.DrawText(5, 690, 1, 0, 0, "Arial", 15, "LENGTH OF LINE: " + Math.Sqrt( (Math.Pow(fModifier.x,2)) + (Math.Pow(fModifier.y, 2)) ) ); 

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
                Ball.Position.y = Ground.Position.y + Ground.Scale.y + Ball.Radius;
                Ball.ApplyFriction();

                if (bWindy == true)
                {
                    Ball.ApplyForce(vRightWind);
                }
                if (bMeteorFell == true)
                {
                    Ball.ApplyForce(Meteor.CalculateAttraction(Ball));
                }
            }
            else
            {
                Ball.ApplyGravity(0.2f);
                Ball.ApplyFriction();
                if (bWindy == true)
                {
                    Ball.ApplyForce(vRightWind);
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

            //Ball Collided with Meteor
            //Get distance between Ball and Metoer then check against its scale
            if (GameUtils.GetDistanceBetween(Meteor, Ball) < (Ball.Radius + Meteor.Radius) + 2.0f)
            {
                Ball.Velocity.x = (-(Ball.Velocity.x) / 1.5f);
                Ball.Velocity.y = (-(Ball.Velocity.y) / 1.5f);
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
