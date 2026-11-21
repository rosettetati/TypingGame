using System;
using System.Drawing;
using System.Windows.Forms;

namespace TypingGame
{
    public partial class Form1 : Form
    {
        private Random rand = new Random();
        private int score = 0;
        private int pointsSinceLastSpeedUp = 0;
        private Label[] letterLabels;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true; // Ensures the form captures keyboard events before controls do

            letterLabels = new Label[] { lblLetter1, lblLetter2, lblLetter3 };
            ResetGame();
        }

        private void ResetGame()
        {
            score = 0;
            pointsSinceLastSpeedUp = 0;
            lblScore.Text = "Score: " + score;
            timerGame.Interval = 50; // Starting timer interval (speed)

            foreach (var lbl in letterLabels)
            {
                ResetLetterBox(lbl);
            }
            timerGame.Start();
        }

        private void ResetLetterBox(Label lbl)
        {
            // Assign a random letter from A to Z
            lbl.Text = ((char)rand.Next('A', 'Z' + 1)).ToString();

            // Random horizontal position at the top of the panel
            int maxX = gamePanel.Width - lbl.Width;
            if (maxX < 0) maxX = 0;
            int randomX = rand.Next(0, Math.Max(1, maxX));

            lbl.Location = new Point(randomX, 0);
        }

        private void timerGame_Tick(object sender, EventArgs e)
        {
            int verticalSpeed = 5; // Pixels moved down per tick

            foreach (var lbl in letterLabels)
            {
                lbl.Top += verticalSpeed;

                // Check if letter reached the bottom of the game area
                if (lbl.Top >= gamePanel.Height)
                {
                    score -= 5;
                    lblScore.Text = "Score: " + score;
                    CheckGameOver();
                    ResetLetterBox(lbl);
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!timerGame.Enabled) return;

            char pressedChar = char.ToUpper((char)e.KeyValue);
            bool matched = false;

            // Check if pressed key matches any visible letter box
            foreach (var lbl in letterLabels)
            {
                if (lbl.Text == pressedChar.ToString())
                {
                    matched = true;
                    score += 1;
                    pointsSinceLastSpeedUp++;
                    ResetLetterBox(lbl);
                    break; // Match only one letter box per keystroke
                }
            }

            if (matched)
            {
                // Increase speed (decrease interval) after every 5 points earned
                if (pointsSinceLastSpeedUp >= 5)
                {
                    pointsSinceLastSpeedUp = 0;
                    if (timerGame.Interval > 15) // Minimum speed cap
                    {
                        timerGame.Interval -= 5;
                    }
                }
            }
            else
            {
                // Pressed a key that is not on the screen
                score -= 1;
            }

            lblScore.Text = "Score: " + score;
            CheckGameOver();
        }

        private void CheckGameOver()
        {
            if (score < 0)
            {
                timerGame.Stop();
                MessageBox.Show($"Game Over! Your score dropped below zero.\nFinal Score: {score}", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetGame();
            }
        }
    }
}