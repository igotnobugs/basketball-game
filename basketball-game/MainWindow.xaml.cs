using SharpGL;
using System;
using System.Threading;
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
using basketball_game.Models;

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
            this.Loaded += new RoutedEventHandler(delegate (object sender, RoutedEventArgs args)
            {
                Top = 0;
                Left = 50;               
            });
        }
                
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

        private ObjectMesh simulatedBall = new ObjectMesh()
        {
            Position = BallDefaultPos,
            Radius = 0.0f,
            Mass = 15,
            Rotation = 0
        };

        private ObjectMesh showBall = new ObjectMesh()
        {
            Position = new Vector3(0, 10, 50),
            Radius = 6.0f,
            Mass = 15,
            Rotation = 0
        };

        private ObjectMesh Line = new ObjectMesh();

        private ObjectMesh Line2 = new ObjectMesh();

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
        public bool enabledMetoer = false;
        public string gameStage = "Shoot to score";

        //Ball Position
        private static Vector3 BallDefaultPos = new Vector3(-40.0f, -25.0f, 50);

        //Line and Aiming Variables 
        private bool showLine = true;
        //private bool bShowSimulationLine = false;
        private float lineLength;
        private Vector3 modifierVector = new Vector3(3.0f, 3.0f, 0);
        private Vector3 simulatedVector = new Vector3(3.0f, 3.0f, 0);
        //private Vector3 fModifier = new Vector3(1.0f, 1.0f, 0);
        private float increments = 0.7f;
        private float minIncrements = 0.3f;
        private float maxIncrements = 1.5f;
        private double aimAngle;
        private bool isDrawSimulatedPath;

        //Camera, View variables
        private Vector3 mousePos = new Vector3();
        private Vector3 mouseVector = new Vector3();
        private Vector3 movementVector = new Vector3();
        private int zoom;
        private double eyex = 0;
        private double eyey = 0;
        private double eyez = 20.0f;
        private double cenx = 0;
        private double ceny = 0;
        private double cenz = 0;
        private float sensitivity = 0.5f;

        //Controls
        private Key shootKey = Key.Space;
        private Key upKey = Key.W;
        private Key downKey = Key.S;
        private Key leftKey = Key.A;
        private Key rightKey = Key.D;
        private Key increaseKey = Key.Q;
        private Key decreaseKey = Key.E;
        private Key resetKey = Key.R;
        private Key cheatKey = Key.C;
        private Key randomizeKey = Key.X;
        private Key toggleLineKey = Key.F;
        private Key toggleMovementKey = Key.Tab;
        private Key quitKey = Key.Escape;

        #endregion

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            mousePos.x = (float)position.X - (float)Width / 2.0f;
            mousePos.y = -((float)position.Y - (float)Height / 2.0f);
            //Console.WriteLine((mousePos.x)+ " " + (mousePos.y));
        }


        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {

            OpenGL gl = args.OpenGL;

            //Set Background Color
            gl.ClearColor(0.7f, 0.7f, 0.9f, 0.0f);

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            float[] global_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] light0pos = new float[] { 1.0f, 1.0f, 1.0f, 0.0f };
            float[] light0ambient = new float[] { 0.0f, 0.0f, 0.0f, 1.0f };
            float[] light0diffuse = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] light0specular = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] lmodel_ambient = new float[] { 1.2f, 1.2f, 1.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            gl.ColorMaterial(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);

            gl.Enable(OpenGL.GL_LINE_SMOOTH);
            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }

        private void OpenGLControl_Resized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(40.0f, (double)Width / (double)Height, 0.01, 350.0);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            Title = "basketball-game";


            OpenGL gl = args.OpenGL;
            // Clear The Screen And The Depth Buffer
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);    

            gl.LoadIdentity();
            //Manipulate Camera
            gl.LookAt(0 - mouseVector.x - movementVector.x, 0 + mouseVector.y - movementVector.y, 20.0f + zoom, 0 - movementVector.x, 0 + mouseVector.y, zoom, 0, 1, 0);
            //Then Translate
            gl.Translate(0.0f, 10.0f, -150.0f);


            //Draw 3D Objects    
            #region Draw 3D Objects
            Ball.DrawBasketBall(gl, 60, 0, 0, 0);
            Ground.DrawCube(gl, 0, 120, 0);
            Pole.DrawCube(gl, 100, 100, 100);
            Board.DrawCube(gl, 0, 200, 200);
            RingEdge.DrawCube(gl, 0, 200, 200);

            RimNet.Draw(gl, 50, 50, 50);
            CenNet.Draw(gl, 40, 40, 40);
            //showBall.DrawBasketBall(gl, 60, 0, 0, 200);

            //simulatedBall.DrawCircle(gl, 1);
            //Aiming Line Draw
            if (showLine)
            {
                if (isBallThrown)
                {
                    Line.DrawLine(gl, Ball, Ball.Velocity, 1, 200, 60, 60);
                    //Line.DrawSimulatedPath(gl, Ball, Ball.Velocity, 1, 200, 60, 60);

                }
                else
                {
                    Line.DrawLine(gl, Ball, modifierVector, 1, 200, 60, 60);


                    //Line2.DrawSimulatedPath(gl, simulatedBall, simulatedVector, 1, 200, 0, 0);
                }
            }
            //End Drawing Most 3D Objects
            #endregion

            #region Draw Text


            var angle = (Math.Truncate((Math.Atan(modifierVector.y / modifierVector.x) * 180 / Math.PI) * 100.0) / 100.0);
            //Above
            gl.DrawText(5, 630, 1, 0, 0, "Arial", 15, "Round: " + curRound);
            gl.DrawText(5, 600, 1, 0, 0, "Arial", 30, "Score: " + curScore);
            gl.DrawText(5, 570, 1, 0, 0, "Arial", 20, "Camera Movement Enabled: " + Keyboard.IsKeyToggled(toggleMovementKey).ToString());
            gl.DrawText(5, 480, 1, 0, 0, "Arial", 30, gameStage);

            //Below
            gl.DrawText(230, 20, 1, 0, 0, "Arial", 15, "Angle: " + angle);
            gl.DrawText(400, 5, 1, 0, 0, "Arial", 15, "Increments: " + increments);
            gl.DrawText(400, 20, 1, 0, 0, "Arial", 15, "Power: " + power);
            gl.DrawText(900, 20, 1, 0, 0, "Arial", 15, "Reset in: " + curCounter);

            
            #endregion


            showBall.Rotation += 5;
            #region WINDY Score when > 10
            //WINDY!!!!!!!!!!!!!! --- SCORE > 10
            if (curScore >= 10)
            {
                isWindy = true;
                gameStage = "It's a bit windy";
                ObjectMesh Leave = new ObjectMesh()
                {
                    Scale = new Vector3(0.5f, 0.5f, 0),
                    Mass = 0.3f
                };
                leaveRandGaussian = (float)Randomizer.Gaussian(-0, 10);
                leaveRandNorm = (float)Randomizer.Generate(0.1f, 0.5f);
                Leave.Position = new Vector3(leaveRandGaussian + 150, leaveRandGaussian, 30);
                Leave.Mass = leaveRandNorm;
                Leaves.Add(Leave);
                foreach (var leaf in Leaves)
                {
                    leaf.DrawCube(gl, 0, 60, 0);
                    leaf.ApplyGravity();
                    leaf.ApplyForce(rightWindVector);
                    leaf.Velocity.Clamp(-0.2f, -0.2f, 0);
                    if (leaf.HasCollidedWith(Ground))
                    {
                        leaf.Position.y = Ground.Position.y + Ground.Scale.y + leaf.Scale.y;
                        leaf.Velocity.y = (-(leaf.Velocity.y));
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
            if ((curScore >= 20) && (enabledMetoer))
            {
                gameStage = "Some kind of meteor...";
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
            if (isBallThrown) {
                curCounter--;
                if (curCounter > maxCounter - 2) {
                    Ball.Velocity = modifierVector / 1.5f;
                }
                Ball.Rotation += 10;
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
                //isSpaceHeld = false;
                isBallThrown = false;
                Ball.Velocity.x *= 0.0f;
                Ball.Velocity.y *= 0.0f;
                Ball.Position = BallDefaultPos;
                randNorm *= 0;
                randGaussian *= 0;
                isScored = false;
            }
            #endregion

            #region Controls
            // These controls only available when Ball is not yet thrown
            if (!isBallThrown)
            {
                //Increment Adjuster
                if (Keyboard.IsKeyDown(increaseKey) && !Keyboard.IsKeyToggled(toggleMovementKey))
                { increments -= 0.1f; }
                else if (Keyboard.IsKeyDown(decreaseKey) && !Keyboard.IsKeyToggled(toggleMovementKey))
                { increments += 0.2f; }
                increments = GameUtils.Constrain(increments, minIncrements, maxIncrements);

                //Adjust Power and Angle modified by Increments
                lineLength = modifierVector.GetLength();
                power = (Math.Truncate(lineLength * 100.0) / 100.0);
                power *= 10;


                if (Keyboard.IsKeyDown(upKey) && (power < 100) && !Keyboard.IsKeyToggled(toggleMovementKey))
                {
                    aimAngle = Math.Atan((modifierVector.y) / (modifierVector.x));
                    modifierVector.x += increments * (float)Math.Cos(aimAngle);
                    modifierVector.y += increments * (float)Math.Sin(aimAngle);
                }
                if (Keyboard.IsKeyDown(downKey) && (power > 10) && !Keyboard.IsKeyToggled(toggleMovementKey))
                {
                    aimAngle = Math.Atan((modifierVector.y) / (modifierVector.x));
                    modifierVector.x -= increments * (float)Math.Cos(aimAngle);
                    modifierVector.y -= increments * (float)Math.Sin(aimAngle);
                }
                if (Keyboard.IsKeyDown(rightKey) && !Keyboard.IsKeyToggled(toggleMovementKey))
                {
                    aimAngle = Math.Atan(modifierVector.y / modifierVector.x);
                    aimAngle -= (Math.PI / 180) + ((Math.PI / 90) * increments);
                    modifierVector.x = lineLength * (float)Math.Cos(aimAngle);
                    modifierVector.y = lineLength * (float)Math.Sin(aimAngle);
                }
                if (Keyboard.IsKeyDown(leftKey) && !Keyboard.IsKeyToggled(toggleMovementKey))
                {
                    aimAngle = Math.Atan(modifierVector.y / modifierVector.x);
                    aimAngle += (Math.PI / 180) + ((Math.PI / 90) * increments);
                    modifierVector.x = lineLength * (float)Math.Cos(aimAngle);
                    modifierVector.y = lineLength * (float)Math.Sin(aimAngle);
                }
            }

            //Play Ball
            if (Keyboard.IsKeyDown(shootKey))
            {
                isBallThrown = true;
            }

            //Restart
            if (Keyboard.IsKeyDown(resetKey) && (curCounter < maxCounter))
            {
                curCounter = -2;
            }

            if (Keyboard.IsKeyToggled(toggleLineKey))
            {
                showLine = false;
                isDrawSimulatedPath = false;
            }
            else
            {
                showLine = true;
                isDrawSimulatedPath = true;
            }

            //Force Random
            if (Keyboard.IsKeyDown(randomizeKey))
            {
                randNorm = (float)Randomizer.Generate(-10, 10);
                randGaussian = (float)Randomizer.Gaussian(1, 2);
                curCounter = -2;
            }

            //Add Score Cheat
            if (Keyboard.IsKeyDown(cheatKey))
            {
                randNorm = (float)Randomizer.Generate(-10, 10);
                randGaussian = (float)Randomizer.Gaussian(1, 2);
                curCounter = -2;
                curScore++;
            }

            if (Keyboard.IsKeyToggled(toggleMovementKey))
            {
                mouseVector = new Vector3(mousePos.x / (50 * (1 / sensitivity)), mousePos.y / (30 * (1 / sensitivity)), 0);
                eyex = 0 - mouseVector.x + movementVector.x;
                eyey = 0 + mouseVector.y - movementVector.y;
                eyez += zoom;
                cenx = 0 - movementVector.x;
                ceny = 0 + mouseVector.y;
                cenz = zoom;

                if (Keyboard.IsKeyDown(increaseKey))
                {
                    zoom += 1;
                }
                if (Keyboard.IsKeyDown(decreaseKey))
                {
                    zoom -= 1;
                }
                if (Keyboard.IsKeyDown(upKey))
                {
                    //movementVector.y -= 1;
                    zoom -= 1;
                }
                if (Keyboard.IsKeyDown(downKey))
                {
                    //movementVector.y -= -1;
                    zoom += 1;
                }
                if (Keyboard.IsKeyDown(leftKey))
                {
                    movementVector.x -= -1;
                }
                if (Keyboard.IsKeyDown(rightKey))
                {
                    movementVector.x -= 1;
                }
            }

            if (Keyboard.IsKeyDown(quitKey))
            {
                Environment.Exit(0);
            }
            #endregion

            #region Physics and Collision
            //Simulated Ball


            //Main Ball
            if (Ball.HasCollidedWith(Ground))
            {
                Ball.Velocity.y = (-(Ball.Velocity.y) / 2);
                Ball.Position.y = Ground.Position.y + Ground.Scale.y + Ball.Radius;
                Ball.ApplyFriction();

                if (isWindy)
                {
                    Ball.ApplyForce(rightWindVector);
                }
                if (isMeteorFall)
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

