using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceInvader
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        //system attribues        
        Graphics g;

        private Timer t = new Timer();
        private List<Rectangle> ShotList = new List<Rectangle>();
        private List<Rectangle> EnemyList = new List<Rectangle>();
        private List<MyRectangle> PowerupList = new List<MyRectangle>(); //red = triple shot, orange =double shot; blue = mvoement speed; green = piercing -> TODO

        //player attributes
        Rectangle PlayerShip;
        MyRectangle PlayerShip2;

        private int movementspeed = 15;
        private int origmovementspeed = 15;
        private int shootspeed = 10;
        private int lifes = 3;
        private int move_x = 0;
        private int move_y = 0;
        private int Score = 0;

        //game attribues
        private int Difficulty = 1;
        private int TickCounter = 0;

        //enemy attribues
        private int SpawnAtTick = 200;
        private int spawnlimit = 20;
        private int enemyMoveSpeed = 2;

        //shot attributes        
        private bool piercingShots = false;
        private bool doubleShots = false;
        private bool tripleshots = false;
        private int movespeedPowerUps = 1;

        private void Form1_Load(object sender, EventArgs e)
        {
            ShotList.Clear();
            this.Text = "SpaceMeInvader";

            Console.WriteLine("starting game");
            Console.WriteLine("Width: " + this.Size.Width);
            Console.WriteLine("Height: " + this.Size.Height);
            
            PlayerShip = new Rectangle(this.Size.Width/2,this.Size.Height - this.Size.Height/2,20,20);
            //PlayerShip2 = new MyRectangle(new Rectangle(1,1,2,2), "Player");

            //todo show lifes as heart on the righ lower corner? maybe
            g = this.CreateGraphics();

            t.Interval = 10;
            t.Tick += new EventHandler(t_Tick);
            t.Start();
        }
        
        private void t_Tick(object sender, EventArgs e)
        {
            //code in here = gameloop

            g.Clear(this.BackColor);
            g.DrawRectangle(new Pen(Brushes.White, 3), PlayerShip);

            PlayerShip.X += move_x;
            PlayerShip.Y += move_y;

            this.Text = "SpaceMeInvader - Score: " + Score + "  - Difficulty: " + Difficulty;
            //generate enemies 
            GnerateAndMoveEnemies();

            MovePowerUps();
            CheckForPowerUpCollision();

            CheckForPlayerCollision();
            //check enemy collision -> lifes -> game over

            move_x = 0;
            move_y = 0;

            DrawShoots();
            //check shoot collision and award points if enemy hit and remove enemy and remove shot if not piercing
            CheckForShootEnemyCollision();
        }

        private void MovePowerUps()
        {
            for (int i = 0; i < PowerupList.Count(); i++)
            {
                MyRectangle tmpPowUp = PowerupList[i];
                //g.DrawRectangle(new Pen(Brushes.Blue, 3), tmpPowUp.ActualRectangle);
                if (tmpPowUp.ID == "doubleshot")
                {
                    g.FillRectangle(Brushes.Blue, tmpPowUp.ActualRectangle);
                }
                if (tmpPowUp.ID == "tripleshot")
                {
                    g.FillRectangle(Brushes.Orange, tmpPowUp.ActualRectangle);
                }
                
                
                if (tmpPowUp.ActualRectangle.Y + movespeedPowerUps < this.Size.Height)
                {
                    PowerupList.Remove(tmpPowUp);
                    Rectangle temprec = tmpPowUp.ActualRectangle;
                    temprec.Y += movespeedPowerUps;
                    tmpPowUp.ActualRectangle = temprec;
                    PowerupList.Add(tmpPowUp);
                }
                else
                {
                    PowerupList.Remove(tmpPowUp);
                    Console.WriteLine("powerup removed from list");
                }
            }
        }

        private void CheckForPowerUpCollision()
        {
            for(int i = 0; i < PowerupList.Count; i++)
            {
                if (Rectangle.Intersect(PowerupList[i].ActualRectangle, PlayerShip) != Rectangle.Empty)
                {                    
                    MyRectangle tmp = PowerupList[i];
                    if (tmp.ID == "doubleshot")
                    {
                        doubleShots = true;
                        tripleshots = false;
                        //todo start timer for double shot duration
                        Console.WriteLine("double shot active");
                        PowerupList.Remove(PowerupList[i]);
                    }
                    if (tmp.ID == "tripleshot")
                    {
                        tripleshots = true;
                        doubleShots = false;
                        //todo start timer for triple shot
                        Console.WriteLine("triple go!");
                        PowerupList.Remove(PowerupList[i]);
                    }
                }
            }

        }

        private void CheckForShootEnemyCollision()
        {
            for(int i = 0; i < ShotList.Count; i++)
            {
                for(int y = 0; y < EnemyList.Count; y++)
                {
                    if(Rectangle.Intersect(ShotList[i], EnemyList[y]) != Rectangle.Empty)
                    {
                        Score += 10;
                        CheckAndIncreaseDifficulty();

                        EnemyRemovedCheckDropTable(EnemyList[y]);

                        EnemyList.Remove(EnemyList[y]);

                        //todo dropchange -> Power Ups: piercing; Double/Triple Shot; movespeed; shootspeed
                        // and do list with timers for each powerup 

                        if (!piercingShots)
                        {
                            ShotList.Remove(ShotList[i]);
                        }
                    }
                }
            }

        }

        private void EnemyRemovedCheckDropTable(Rectangle para)
        {
            Random r = new Random();
            int val = r.Next(1, 99);
            if (val <= 10)
            {
                Rectangle doubleShotPup = new Rectangle(para.X, para.Y, para.Width/2, para.Height/2);
                MyRectangle doubleShotPupW = new MyRectangle(doubleShotPup, "doubleshot");

                //g.DrawRectangle(new Pen(Brushes.Blue, 3), doubleShotPupW.ActualRectangle);
                g.FillRectangle(Brushes.Blue, doubleShotPupW.ActualRectangle);
                PowerupList.Add(doubleShotPupW);
            }
            if (val >= 95)
            {
                Rectangle tripleShotPup = new Rectangle(para.X, para.Y, para.Width / 2, para.Height / 2);
                MyRectangle tripleShotPupW = new MyRectangle(tripleShotPup, "tripleshot");
                g.FillRectangle(Brushes.Orange, tripleShotPupW.ActualRectangle);
                PowerupList.Add(tripleShotPupW);
            }
        }

        private void CheckAndIncreaseDifficulty()
        {
            if ((Score % 100) == 0)
            {
                IncreaseDifficulty();
            }                     
        }

        private void CheckForPlayerCollision()
        {
            bool hitmarker = false;

            for(int i = 0; i < EnemyList.Count; i++)
            {
                if (Rectangle.Intersect(EnemyList[i], PlayerShip) != Rectangle.Empty)
                {
                    hitmarker = true;
                    EnemyList.Remove(EnemyList[i]);
                }
            }

            if (hitmarker)
            {
                if(lifes - 1 > 0)
                {
                    lifes--;
                    Console.WriteLine("life lost");
                    hitmarker = false;
                }
                else
                {
                    //game over                    
                    Console.WriteLine("game over");
                    this.Text = "Game Over";
                    t.Stop();
                }
            }

        }

        private void IncreaseDifficulty()
        {
            Difficulty += 1;
            enemyMoveSpeed += 1;
            spawnlimit += 10;
            if (SpawnAtTick - 50 > 0)
            {
                SpawnAtTick -= 50;
            }

        }

        private void GnerateAndMoveEnemies()
        {

            int spawnX;
            int spawnY;

            TickCounter += 1;

            //check and spawn
            if ((EnemyList.Count() < spawnlimit) && (TickCounter >= SpawnAtTick))
            {
                //spawn one new one and add to list

                //generate random pos on the upper border 
                Random r = new Random();
                spawnX = r.Next(20, this.Size.Width - 40);

                Random r2 = new Random();
                spawnY = r2.Next(20, this.Size.Height / 6);

                Console.WriteLine("enemy x: " + spawnX + ", y: " + spawnY);

                Rectangle enemy = new Rectangle(spawnX, spawnY, 20, 20);
                AddToEnemyList(enemy);
                //g.DrawRectangle(new Pen(Brushes.Red, 3), enemy);
                TickCounter = 0;
            }
            
            //move them!
            for (int i = 0; i < EnemyList.Count(); i++)
            {
                Rectangle tmpEnemy = EnemyList[i];
                g.DrawRectangle(new Pen(Brushes.Red, 3), tmpEnemy);

                if (tmpEnemy.Y + enemyMoveSpeed < this.Size.Height)
                {
                    EnemyList.Remove(tmpEnemy);
                    tmpEnemy.Y += enemyMoveSpeed;
                    EnemyList.Add(tmpEnemy);
                }
                else
                {
                    EnemyList.Remove(tmpEnemy);
                    Console.WriteLine("enemy removed from list");
                }
            }                        
        }

        private void AddToEnemyList(Rectangle enemy)
        {
            EnemyList.Add(enemy);
        }

        private void DrawShoots()
        {
            for (int i = 0; i < ShotList.Count; i++)
            {
                Rectangle TmpShoot = ShotList[i];
                g.DrawRectangle(new Pen(Brushes.White, 3), TmpShoot);
                //Console.WriteLine("new shoot: " + TmpShoot.X + ", " + TmpShoot.Y);
                
                if (TmpShoot.Y - shootspeed > 0)
                {
                    ShotList.Remove(TmpShoot);
                    TmpShoot.Y -= shootspeed;
                    ShotList.Add(TmpShoot);
                }
                else
                {
                    ShotList.Remove(TmpShoot);
                }
            }                
            
        }

        private void Form1_KeyDown(object sewnder, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                //shoot 
                Rectangle shoot = new Rectangle(PlayerShip.X + 8, PlayerShip.Y, 2, 10);
                AddShotToDrawer(shoot);

                if (doubleShots)
                {
                    Rectangle shoot2 = new Rectangle(PlayerShip.X + 1, PlayerShip.Y, 2, 10);
                    AddShotToDrawer(shoot2);
                }
                if (tripleshots)
                {
                    Rectangle shoot2 = new Rectangle(PlayerShip.X + 5, PlayerShip.Y, 2, 10);
                    Rectangle shoot3 = new Rectangle(PlayerShip.X + 11, PlayerShip.Y, 2, 10);

                    AddShotToDrawer(shoot2);
                    AddShotToDrawer(shoot3);
                }

            }
            if (e.KeyCode == Keys.W)
            {
                //move forward
                if (PlayerShip.Y - movementspeed > 0)
                {
                    move_y -= movementspeed;
                }                
            }
            if (e.KeyCode == Keys.A)
            {
                //move left
                if (PlayerShip.X - movementspeed > 0)
                {
                    move_x -= movementspeed;
                }
            }
            if (e.KeyCode == Keys.S)
            {
                //move back
                if (PlayerShip.Y + movementspeed < this.Size.Height) //todo set border better
                {
                    move_y += movementspeed;
                }
            }
            if (e.KeyCode == Keys.D)
            {
                //move right
                if (PlayerShip.Y + movementspeed < this.Size.Width) //todo set border better
                {
                    move_x += movementspeed;
                }
            }

        }

        private void AddShotToDrawer(Rectangle shoot_p)
        {
            ShotList.Add(shoot_p);
        }
    }
}
