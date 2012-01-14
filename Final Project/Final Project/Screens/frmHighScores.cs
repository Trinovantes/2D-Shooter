using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Final_Project.Screens
{
    public partial class frmHighScores : Form
    {
        public frmHighScores()
        {
            InitializeComponent();
            Application.Idle += new EventHandler(Pause);
        }

        private void Pause(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            /* 
             * prevent double popup bug
             * - caused when xna loads form via mouse click and closes form via Enter key
             * - will register both inputs as true when form closes since xna update 
             *   cycle pause while form is active (evaluating true when the mouse is
             *   over any menu will activate said menu item immediately)
             * - focus on lstScores will prevent Enter key from closing form on btnClose
             */
            lstScores.Focus();

            this.Close();
        }

        public void UpdateScores(int[] scores)
        {
            Array.Sort(scores);

            lstScores.Items.Clear();

            foreach (int score in scores.Reverse<int>())
            {
                if (score > 0)
                {
                    lstScores.Items.Add(String.Format("{0:N0}", score));
                }
            }
        }
    }
}
