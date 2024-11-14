using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2024_11_14_Dolgozat
{
    public partial class Form1 : Form
    {
        Timer moveCharacter = new Timer();
        Timer moveApple = new Timer();
        Timer HandIncrease = new Timer();
        Timer HandDecrease = new Timer();
        Timer fallDown = new Timer();

        int points = 10;
        int armWidth = 25;
        int basketCap = 20;
        int punchMultiplier = 10;
        int hitCounter = 0;
        bool turnCharacter = false; // ha jobbra fordul akkor true
        bool isFalling = false;
        bool goLeft = true;
        bool goRight = false;
        List<PictureBox> apples = new List<PictureBox>();
        int valueA = 10;
        int valueB = 30;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.BackColor = Color.LightBlue;
            label1.Text = $"A gyűjtött almák száma: {points}";
            label2.Text = $"A kosár tehetbírása: {basketCap} alma";
            label3.Text = "Nagyobb kosár: ";
            label4.Text = "Gyorsabb szedés: ";
            button1.Text = $"-{valueA} alma";
            button2.Text = $"-{valueB} alma";
            trunk.SendToBack();
            arm.Width = armWidth;
            button1.Text = "-10 alma";
            button2.Text = "-30 alma";
            label5.Text = $"{hitCounter}";
            Start();
        }


        void Start()
        {
            MoveCharacter();
            BasketIncrease();
            FastCollect();
        }

        void MoveCharacter()
        {
            moveCharacter.Interval = 50;
            moveCharacter.Tick += (s, e) =>
            {
                if (goLeft)
                {
                    MoveCharacterLeft();
                    if (arm.Bounds.IntersectsWith(trunk.Bounds))
                    {
                        moveCharacter.Stop();
                        goLeft = false;
                        Punch();
                        moveCharacter.Start();
                    }
                }
                else if (goRight)
                {
                    MoveCharacterRight();
                    if (arm.Left >= basket.Left)
                    {
                        moveCharacter.Stop();
                        TurnCharacter();
                        goRight = false;
                        moveCharacter.Start();
                    }
                }
            };
            moveCharacter.Start();
        }
        void MoveCharacterLeft()
        {
            goRight = false;
            goLeft = true;
            arm.Left -= 5;
            head.Left -= 5;
            body.Left -= 5;
        }

        void MoveCharacterRight()
        {
            goRight = true;
            goLeft = false;
            arm.Left += 5;
            head.Left += 5;
            body.Left += 5;
        }


        void TurnCharacter()
        {
            turnCharacter = !turnCharacter;
            if (turnCharacter)
                arm.Left += 20;

            if (!turnCharacter)
                arm.Left -= 20;
        }

        void Punch()
        {
            hitCounter = 0;
            goRight = false;
            if (!isFalling)
            {
                HandIncrease.Interval = 200;
                HandIncrease.Tick += (s, e) =>
                {
                    
                    if (!turnCharacter)
                    {
                        arm.Left -= 5;
                    }
                    HandIncrease.Stop();
                    
                    HandDecrease.Start();
                };
                HandIncrease.Start();
                label5.Text = $"{hitCounter}";

                HandDecrease.Interval = 200;
                HandDecrease.Tick += (s, e) =>
                {

                    if (!turnCharacter) {
                        arm.Left += 5;
                    }
                    HandDecrease.Stop();

                    label5.Text = $"{hitCounter}";
                    if (hitCounter < punchMultiplier )
                        HandIncrease.Start();
                   
                    Hit();
                    
                };
                HandDecrease.Start();
               
            }
          
        }

        void Hit()
        {
            
            hitCounter++;
            if (hitCounter == punchMultiplier)
            {
                GenerateApple();
                AppleFall();

              
            }
        }

        public void GenerateApple()
        {
            apples.Clear();
            PictureBox alma = new PictureBox();
            alma.BackColor = System.Drawing.Color.Red;
            alma.Left = 200;
            alma.Top = foliage.Bottom;
            alma.Name = "alma";
            alma.Size = new System.Drawing.Size(17, 17);
            alma.BringToFront();
            apples.Add(alma);
            this.Controls.Add(alma);
            isFalling = true;
        }


        void AppleFall()
        {
            foreach (PictureBox alma in apples)
            {
               
                alma.BringToFront();
                fallDown.Interval = 50;
                fallDown.Tick += (s, e) =>
                {
                    alma.Top += 5;
                    if (arm.Bounds.IntersectsWith(alma.Bounds))
                    {
                        fallDown.Stop();
                        goLeft = false;

                        TurnCharacter();

                        MoveAppleX();
                        goRight = true;
                        MoveCharacter();
                        
                        if (goRight)
                            alma.Left = arm.Right - alma.Width / 2;
                    }
                };
                fallDown.Start();
            }
        }

        void MoveAppleX()
        {
            moveApple.Start();

            if (!goLeft)
            {
                foreach (PictureBox alma in apples)
                    alma.Left += 20;    //fordulás miatti léptetés
                moveApple.Interval = 50;
                moveApple.Tick += (s, e) =>
                {
                    foreach (PictureBox alma in apples)
                    {
                        if (alma.Left < basket.Left)
                            alma.Left += 10;
                        if (alma.Left >= basket.Left)
                        {
                            alma.Top += 10;
                            if (alma.Bottom >= basket.Top)
                            {
                                moveApple.Stop();
                                alma.Hide();
                                
                                this.Controls.Remove(alma);
                                isFalling = false;
                                goRight = false;
                                goLeft = true;
                                arm.Left = body.Left -20;

                                if (points < basketCap)
                                    points++;
                                label1.Text = $"A gyűjtött almák száma: {points}";
                                hitCounter = 0;
                            }
                        }
                    }
                };
            }
        }

        void BasketIncrease()
        {
            button1.Click += (s, e) =>
            {
                if (points >= 10)
                {
                    basketCap += 5;
                    points -= 10;
                    valueA += 2;
                    label1.Text = $"A gyűjtött almák száma: {points}";
                    label2.Text = $"A kosár tehetbírása: {basketCap} alma";
                    button1.Text = $"-{valueA} alma";
                }
                else
                    MessageBox.Show("Nincs elég almád");
            };
        }


        void FastCollect()
        {
            button2.Click += (s, e) =>
            {
                if (points >= valueB && punchMultiplier > 3)
                {

                    punchMultiplier--;
                    points -= 30;
                    valueB += 30;
                    label1.Text = $"A gyűjtött almák száma: {points}";
                    button2.Text = $"-{valueB} alma";
                }
                else
                    MessageBox.Show("Nincs elég almád");
            };
        }


    }
}