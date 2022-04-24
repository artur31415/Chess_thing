using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Chess_thing
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        Form1 Game;
        bool IsAI = false;
        bool IsTime = false;
        int[] TimerTime = new int[2];
        

        private void MainMenu_Load(object sender, EventArgs e)
        {
            string[] _Times = { "00:05", "00:30", "10:00", "15:00", "20:00", "25:00", "30:00" };
            for (int i = 0; i < _Times.GetLength(0); ++i)
            {
                Times.Items.Add(_Times[i]);
            }
            Times.SelectedIndex = 0;
            Times.Visible = false;
            AI.Enabled = false;
        }

        private void Begin_Click(object sender, EventArgs e)
        {
            Game = new Form1(IsTime, TimerTime, IsAI);
            Game.Show();
            Comeback.Start();
            this.Hide();
        }

        private void Options(object sender, EventArgs e)
        {
            if ((CheckBox)sender == AI)
            {
                IsAI = !IsAI;
            }
            else if ((CheckBox)sender == TIME)
            {
                IsTime = !IsTime;
                Times.Visible = IsTime;
            }
        }

        private void Times_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] aTime = Times.SelectedItem.ToString().Split(':');
           //TimerTime[0] = int.Parse(aTime[0]);
            //MessageBox.Show(aTime[0] + ":" + aTime[1]);
            try
            {
                TimerTime[1] = int.Parse(aTime[0]);
                TimerTime[0] = int.Parse(aTime[1]);
                //MessageBox.Show(TimerTime[0].ToString("00") + ":" + TimerTime[1].ToString("00"));
            }
            catch (Exception)
            {
            }
        }

        private void Comeback_Tick(object sender, EventArgs e)
        {
            if (Game.GoBack)
            {
                Game.Hide();
                this.Show();
                
                Comeback.Stop();
            }
        }
    }
}
