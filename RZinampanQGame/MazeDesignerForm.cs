/*
 * Assignment2
 * Q Puzzle Game
 * 
 * Revision History:
 *  Romnick Zinampan October.27.2023
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RZinampanQGame
{
    public partial class MazeDesignerForm : Form
    {
        private int selectedTool = 1;
        private int saveGameCounter = 1;

        public MazeDesignerForm()
        {
            InitializeComponent();

            foreach (Control control in pnlGrid.Controls)
            {
                if (control is PictureBox pictureBox)
                {
                    pictureBox.Click += PictureBox_Click;
                }
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            int rows, columns;

            if (pnlGrid.Controls.Count > 0)
            {
                // Ask the user if they want to leave the current level and start a new one
                var result = MessageBox.Show("Do you want to abandon the current level and start a new one?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    // If the user doesn't want to start a new level
                    return;
                }
                else
                {
                    // If the user accepts, clear the grid
                    pnlGrid.Controls.Clear();
                }
            }

            if (int.TryParse(txtRows.Text, out rows) && int.TryParse(txtColumns.Text, out columns))
            {
                if (rows <= 0 || columns <= 0)
                {
                    MessageBox.Show("Please enter valid row and column numbers. (Both Must be Positive numbers)", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < columns; j++)
                        {
                            PictureBox pictureBox = new PictureBox();
                            pictureBox.Visible = true;
                            pictureBox.Width = 50;
                            pictureBox.Height = 50;
                            pictureBox.Location = new System.Drawing.Point(j * 50, i * 50);
                            pictureBox.BorderStyle = BorderStyle.FixedSingle;

                            pictureBox.Tag = 1;

                            pictureBox.Click += PictureBox_Click;
                            pnlGrid.Controls.Add(pictureBox);
                            pictureBox.Enabled = true;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter valid row and column numbers. (Both Must be Integers)", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender is PictureBox pictureBox)
                {
                    if (selectedTool == 1)
                    {
                        pictureBox.Image = null;
                    }
                    else if (selectedTool == 2)
                    {
                        pictureBox.Image = Properties.Resources.Brickwall;
                        pictureBox.Tag = 2;
                    }
                    else if (selectedTool == 3)
                    {
                        pictureBox.Image = Properties.Resources.RedDoor;
                        pictureBox.Tag = 3;
                    }
                    else if (selectedTool == 4)
                    {
                        pictureBox.Image = Properties.Resources.GreenDoor;
                        pictureBox.Tag = 4;
                    }
                    else if (selectedTool == 5)
                    {
                        pictureBox.Image = Properties.Resources.RedBox;
                        pictureBox.Tag = 5;
                    }
                    else if (selectedTool == 6)
                    {
                        pictureBox.Image = Properties.Resources.GreenBox;
                        pictureBox.Tag = 6;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Q-Game Files (*.qgame)|*.qgame";

            string suggestedFilename = $"savegame{saveGameCounter}.qgame";
            saveFileDialog.FileName = suggestedFilename;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveFileDialog.FileName))
                    {
                        int rows, columns;

                        if (!int.TryParse(txtRows.Text, out rows) || !int.TryParse(txtColumns.Text, out columns) || rows <= 0 || columns <= 0)
                        {
                            MessageBox.Show("Please enter valid row and column numbers. (Both Must be Integers)", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        writer.WriteLine(rows);
                        writer.WriteLine(columns);

                        int doorCount = 0;
                        int boxCount = 0;
                        int wallCount = 0;

                        foreach (Control control in pnlGrid.Controls)
                        {
                            if (control is PictureBox pictureBox)
                            {
                                int Value = (int)pictureBox.Tag;

                                // Write the row and column indices
                                writer.WriteLine(pictureBox.Top / 50);
                                writer.WriteLine(pictureBox.Left / 50);

                                writer.WriteLine(Value);

                                if (Value == 2)
                                {
                                    wallCount++;
                                }
                                else if (Value == 3 || Value == 4)
                                {
                                    doorCount++;
                                }
                                else if (Value == 5 || Value == 6)
                                {
                                    boxCount++;
                                }
                            }
                        }

                        string confirmationMessage = $"Level saved successfully.\nTotal number of Doors: {doorCount}\nTotal number of Boxes: {boxCount}\nTotal number of Walls: {wallCount}\n";
                        MessageBox.Show(confirmationMessage, "Level Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        saveGameCounter++;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while saving the level: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btnGreenBox_Click(object sender, EventArgs e)
        {
            selectedTool = 6;
        }

        private void btnRedBox_Click(object sender, EventArgs e)
        {
            selectedTool = 5;
        }

        private void btnGreenDoor_Click(object sender, EventArgs e)
        {
            selectedTool = 4;
        }

        private void btnRedDoor_Click(object sender, EventArgs e)
        {
            selectedTool = 3;
        }

        private void btnWall_Click(object sender, EventArgs e)
        {
            selectedTool = 2;
        }

        private void btnNone_Click(object sender, EventArgs e)
        {
            selectedTool = 1;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ControlPanelForm newControlPanelForm = new ControlPanelForm();
            newControlPanelForm.Show();

            Visible = false;
        }
    }
}