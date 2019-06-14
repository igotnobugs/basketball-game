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
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            gl.Enable(OpenGL.GL_DEPTH_TEST);
           
            float[] global_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] light0pos = new float[] { 0.0f, 0.0f, 10.0f, 0.0f };
            float[] light0ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light0diffuse = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            float[] lmodel_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);

            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            //gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            //gl.Enable(OpenGL.GL_LIGHTING);        
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Enable(OpenGL.GL_LINE_SMOOTH);

            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }

        private void OpenGLControl_Resized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            //  Get the OpenGL object.
            OpenGL gl = args.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            //  Create a perspective transformation.
            gl.Perspective(40.0f, (double)Width / (double)Height, 0.01, 350.0);

            //  Use the 'look at' helper function to position and aim the camera.
            //gl.LookAt(0, 0, 20.0f, 0, 0, 0, 0, 1, 0);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private Vector3 mouseVector = new Vector3(0,0,0);


                
        #region Static Objects
        private  ObjectMesh Ground = new ObjectMesh()
        {
            Position = new Vector3(0, -35, 50),
            Scale = new Vector3(90, 1.5f, 50)
        };

        private ObjectMesh Box = new ObjectMesh()
        {
            Position = new Vector3(0, 0, 50),
            Scale = new Vector3(5, 5, 1)
        };

        private ObjectMesh Pole = new ObjectMesh()
        {
            Position = new Vector3(50, -15, 50),
            Scale = new Vector3(0.5f, 20, 1)
        };

        private static ObjectMesh Board= new ObjectMesh()
        {
            Position = new Vector3(48.5f, 3.0f, 50),
            Scale = new Vector3(0.5f, 4.5f, 1)
        };

        private Liquid RimNet = new Liquid(Board.Position.x - 3, 1.5f, 50, 2.5f, 1.0f, 1);
        private Liquid CenNet = new Liquid(Board.Position.x - 3, 0.5f, 50, 2.5f, 2.0f, 1);

        private ObjectMesh RingEdge = new ObjectMesh()
        {
            Position = new Vector3(Board.Position.x - 6, 1.0f, 50),
            Scale = new Vector3(0.5f, 0.5f, 0)
        };
        #endregion

        #region Moveable Objects
        private ObjectMesh Ball = new ObjectMesh()
        {
            Position = BallDefaultPos,
            Radius = 2.0f,
            Mass = 15,
            Rotation = 0
        };

        private ObjectMesh Line = new ObjectMesh();

        private List<ObjectMesh> Leaves = new List<ObjectMesh>();

        private Attractor Meteor = new Attractor()
        {
            Position = new Vector3(30, 500, 50),
            Radius = 20.0f,
            Mass = 50
        };
        #endregion

        #region Variables
        //Game
        private bool isBallThrown = false;
        private bool isSpaceHeld = false;
        private bool isWindy = false;
        private bool isMeteorFall = false;
        private bool isScored = false;
        private int curCounter = 60; //Gets reduced
        private int maxCounter = 60; // Used for checking
        private int curRound = 1;
        private int curScore = 0;
        private float randNorm = 0;
        private float randGaussian = 0;
        private float leaveRandNorm = 0;
        private float leaveRandGaussian = 0;
        private double power = 0;
        private Vector3 rightWindVector = new Vector3(-0.2f, 0, 0);
        private Vector3 meteorFallingVector = new Vector3(0, -10.0f, 0);

        //Ball Position
        private static Vector3 BallDefaultPos = new Vector3(-40.0f, -25.0f, 50);

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
        private double aimAngle;

        //Camera, View variables
        private Vector3 lookAtVector = new Vector3();
        private Vector3 mousePos = new Vector3();
        private Vector3 movementVector = new Vector3();
        private int zoom;

        #endregion

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            mousePos.x = (float)position.X - (float)Width / 2.0f;
            mousePos.y = -((float)position.Y - (float)Height / 2.0f);
            mouseVector = new Vector3(mousePos.x / 100, mousePos.y / 100, 0);
            //Console.WriteLine((mousePos.x)+ " " + (mousePos.y));
        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            
            Title = "basketball-game";

            #region OpenGL Draw 
            OpenGL gl = args.OpenGL;

            // Clear The Screen And The Depth Buffer
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            // Move screen back
            gl.LoadIdentity();
            gl.LookAt(0 - mouseVector.x - movementVector.x, 0 - mouseVector.y, 20.0f + zoom, 0 - movementVector.x, 0 + mouseVector.y, zoom, 0, 1, 0);
            gl.Translate(0.0f , 0.0f, -150.0f);

            //Draw All Objects
            gl.Color(0.0, 0.0, 1.5);
            Ball.DrawBasketBall(gl, 60);
            //gl.Color(0.0, 0.5, 0.0);          
            Ground.DrawCube(gl, 0, 120, 0);
            gl.Color(0.3, 0.3, 0.3);          
            Pole.DrawCube(gl);
            gl.Color(0.0, 0.5, 0.5);
            Board.DrawCube(gl);
            gl.Color(0.0, 0.5, 0.5);
            RingEdge.DrawCube(gl);

            RimNet.Draw(gl, 50, 50, 50);
            CenNet.Draw(gl, 40, 40, 40);

            //Box.DrawCube(gl);

           
            //Aiming Line Draw
            if (bShowLine == true)
            {
                gl.Color(1.0, 0.6, 0.6);
                if (isBallThrown == true)
                {
                    Line.DrawLine(gl, Ball, Ball.Velocity, 1);
                }
                else
                {
                    Line.DrawLine(gl, Ball, vModLine, 1);
                }
            }
            #endregion

            #region WINDY Score when > 10
            //WINDY!!!!!!!!!!!!!! --- SCORE > 10
            if (curScore >= 10)
            {
                isWindy = true;
                ObjectMesh Leave = new ObjectMesh()
                {
                    Scale = new Vector3(0.5f, 0.5f, 0),
                    Mass = 0.3f
                };
                leaveRandGaussian = (float)Randomizer.Gaussian(-0, 10);
                leaveRandNorm = (float)Randomizer.Generate(0.1f, 0.5f);
                Leave.Position = new Vector3(leaveRandGaussian + 200, leaveRandGaussian, 0);
                Leave.Mass = leaveRandNorm;
                Leaves.Add(Leave);
                foreach (var leaf in Leaves)
                {
                    gl.Color(0.5, 0.7, 0.0);
                    leaf.DrawCube(gl);
                    leaf.ApplyGravity();
                    leaf.ApplyForce(rightWindVector);
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
            if (curScore >= 20)
            {
                Meteor.DrawCircle(gl);
                Meteor.ApplyForce(meteorFallingVector);
                if (Meteor.Position.y <= -30)
                {
                    Meteor.Velocity.y *= 0;
                    Meteor.Position.y = -30;
                    isMeteorFall = true;
                }
            }
            #endregion        

            #region Ball Thrown Code
            //You pressed Space
            if (isBallThrown == true) {
                curCounter--;
                if (curCounter > maxCounter - 2) {
                    Ball.Velocity.x = vModLine.x / 1.5f;
                    Ball.Velocity.y = vModLine.y / 1.5f;
                }
                Ball.Rotation += 10;
                lookAtVector = Ball.Position;
            }
            #endregion

            #region Scoring
            if (CenNet.Contains(Ball)) 
            {
                Console.WriteLine("Ball is in CenNet...");
                if (!isScored)
                {
                    curScore++;
                    randNorm = (float)Randomizer.Generate(-10, 10);
                    randGaussian = (float)Randomizer.Gaussian(1, 2);
                    Console.WriteLine("Player Scores!");
                    isScored = true;
                }
                Ball.ApplyForce(RimNet.CalculateDragForce(Ball) * 1);
                Ball.ApplyForce(CenNet.CalculateDragForce(Ball) * 2);          
            }
            #endregion

            #region Reset Code
            if (curCounter < 0)
            {
                Pole.Position.x += randNorm;
                Board.Position.x += randNorm; 
                RingEdge.Position.x += randNorm;
                RimNet.x += randNorm;
                CenNet.x += randNorm;

                if ((randNorm < 1) && (randNorm > -1))
                {
                    Board.Position.y += randGaussian;
                    RimNet.y += randGaussian;
                    CenNet.y += randGaussian;
                    RingEdge.Position.y += randGaussian;       
                }

                if ((Pole.Position.x > 70) || (Pole.Position.x < 30))
                {
                    Pole.Position.x = 50;
                    Board.Position.x = 48.5f;
                    RingEdge.Position.x = 42.5f;
                    RimNet.x = 45.5f;
                    CenNet.x = 45.5f;
                }
                curCounter = maxCounter;
                isScored = false;
                curRound++;
            }

            if (curCounter == maxCounter)
            {
                isSpaceHeld = false;
                isBallThrown = false;
                Ball.Velocity.x *= 0.0f;
                Ball.Velocity.y *= 0.0f;
                Ball.Position = BallDefaultPos;
                randNorm *= 0;
                randGaussian *= 0;
            }
            #endregion

            #region Controls
            // These controls only available when Ball is not yet thrown
            if (isBallThrown == false)
            {
                //Increment Adjuster
                if (Keyboard.IsKeyDown(Key.Q) && !Keyboard.IsKeyToggled(Key.CapsLock))
                { fIncrements -= 0.1f; }
                else if (Keyboard.IsKeyDown(Key.E) && !Keyboard.IsKeyToggled(Key.CapsLock))
                { fIncrements += 0.2f; }
                fIncrements = GameUtils.Constrain(fIncrements, fMinIncrements, fMaxIncrements);

                //Adjust Power and Angle modified by Increments
                fLineLength = vModLine.GetLength();
                power = (Math.Truncate(fLineLength * 100.0) / 100.0);
                power *= 10;


                if (Keyboard.IsKeyDown(Key.W) && (power < 100) && !Keyboard.IsKeyToggled(Key.CapsLock))
                {

                        aimAngle = Math.Atan((vModLine.y) / (vModLine.x) );
                        vModLine.x += fIncrements * (float)Math.Cos(aimAngle);
                        vModLine.y += fIncrements * (float)Math.Sin(aimAngle);
                }
                if (Keyboard.IsKeyDown(Key.S) && (power > 10) && !Keyboard.IsKeyToggled(Key.CapsLock))
                {
                    aimAngle = Math.Atan((vModLine.y) / (vModLine.x));
                    vModLine.x -= fIncrements * (float)Math.Cos(aimAngle);
                    vModLine.y -= fIncrements * (float)Math.Sin(aimAngle);
                }
                if (Keyboard.IsKeyDown(Key.D) && !Keyboard.IsKeyToggled(Key.CapsLock))
                {
                    aimAngle = Math.Atan(vModLine.y / vModLine.x);
                    aimAngle -= (Math.PI / 180) + ((Math.PI / 90) * fIncrements);
                    vModLine.x = fLineLength * (float)Math.Cos(aimAngle);
                    vModLine.y = fLineLength * (float)Math.Sin(aimAngle);
                }
                if (Keyboard.IsKeyDown(Key.A) && !Keyboard.IsKeyToggled(Key.CapsLock))
                {
                    aimAngle = Math.Atan(vModLine.y / vModLine.x);
                    aimAngle += (Math.PI/180) + ((Math.PI / 90) * fIncrements);
                    vModLine.x = fLineLength * (float)Math.Cos(aimAngle);
                    vModLine.y = fLineLength * (float)Math.Sin(aimAngle);
                }
            }

            //Play Ball
            if (Keyboard.IsKeyDown(Key.Space))
            {
                if (isSpaceHeld == false)
                isBallThrown = true;
                isSpaceHeld = true;
            }
                
            //Restart
            if (Keyboard.IsKeyDown(Key.R) && (curCounter < maxCounter))
            {
                 curCounter = -2;
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
                randNorm = (float)Randomizer.Generate(-10, 10);
                randGaussian = (float)Randomizer.Gaussian(1, 2);
                curCounter = -2;
            }

            //Add Score Cheat
            if (Keyboard.IsKeyDown(Key.C))
            {
                randNorm = (float)Randomizer.Generate(-10, 10);
                randGaussian = (float)Randomizer.Gaussian(1, 2);
                curCounter = -2;
                curScore++;
            }


            if (Keyboard.IsKeyDown(Key.Q) && Keyboard.IsKeyToggled(Key.CapsLock))
            {
                zoom += 1;
            }
            if (Keyboard.IsKeyDown(Key.E) && Keyboard.IsKeyToggled(Key.CapsLock))
            {
                zoom -= 1;
            }
            if (Keyboard.IsKeyDown(Key.W) && Keyboard.IsKeyToggled(Key.CapsLock))
            {
                movementVector.y -= 1;
            }
            if (Keyboard.IsKeyDown(Key.S) && Keyboard.IsKeyToggled(Key.CapsLock))
            {
                movementVector.y -= -1;
            }
            if (Keyboard.IsKeyDown(Key.A) && Keyboard.IsKeyToggled(Key.CapsLock))
            {
                movementVector.x -= -1;
            }
            if (Keyboard.IsKeyDown(Key.D) && Keyboard.IsKeyToggled(Key.CapsLock))
            {
                movementVector.x -= 1;
            }

            if (Keyboard.IsKeyDown(Key.Escape))
            {
                Environment.Exit(0);
            }
            #endregion

            #region Text

            var angle = (Math.Truncate((Math.Atan(vModLine.y / vModLine.x) * 180 / Math.PI) * 100.0) / 100.0);
            //Below
            //I don't know why this text is corrupted
            gl.DrawText(99930, 20, 1, 0, 0, "Arial", 10, "Angle: " + angle);


            gl.DrawText(230, 20, 1, 0, 0, "Arial", 15, "Angle: " + angle);
            gl.DrawText(400, 5, 1, 0, 0, "Arial", 15, "Increments: " + fIncrements);
            gl.DrawText(400, 20, 1, 0, 0, "Arial", 15, "Power: " + power);
            gl.DrawText(900, 20, 1, 0, 0, "Arial", 15, "Reset in: " + curCounter);

            //Above

            gl.DrawText(5, 630, 1, 0, 0, "Arial", 15, "Round: " + curRound);
            gl.DrawText(5, 600, 1, 0, 0, "Arial", 30, "Score: " + curScore);
            gl.DrawText(5, 570, 1, 0, 0, "Arial", 20, "Camera Movement Enabled: " + Keyboard.IsKeyToggled(Key.CapsLock).ToString());

            if (isWindy == true)
            {
                gl.DrawText(5, 530, 1, 0, 0, "Arial", 40, "Windy");
            }

            if (isMeteorFall == true)
            {
                gl.DrawText(5, 430, 1, 0, 0, "Arial", 30, "What's That?!");
            }
            #endregion

            #region Physics and Collision
            if (Ball.HasCollidedWith(Ground))
            {
                Ball.Velocity.y = (-(Ball.Velocity.y) / 2);
                Ball.Position.y = Ground.Position.y + Ground.Scale.y + Ball.Radius;
                Ball.ApplyFriction();

                if (isWindy == true)
                {
                    Ball.ApplyForce(rightWindVector);
                }
                if (isMeteorFall == true)
                {
                    Ball.ApplyForce(Meteor.CalculateAttraction(Ball));
                }
            }
            else
            {
                Ball.ApplyGravity(0.2f);
                Ball.ApplyFriction();
                if (isWindy == true)
                {
                    Ball.ApplyForce(rightWindVector);
                }
                if (isMeteorFall == true)
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
