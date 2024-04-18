using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RZinampanQGame
{
    public partial class MazePlayForm : Form
    {
        private int movesCount = 0;
        private int remainingBoxesCount = 0;      
        private PictureBox selectedBox;
        private PictureBox[,] pictureBoxGrid;

        public MazePlayForm()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            movesCount = 0;
            remainingBoxesCount = CalculateRemainingBoxes();
            UpdateScores();
        }

        private void UpdateScores()
        {
            txtMoves.Text = $"{movesCount}";
            txtRemainingBoxes.Text = $"{remainingBoxesCount}";
        }

        private int CalculateRemainingBoxes()
        {
            // Calculate the initial number of remaining boxes
            return pnlGame.Controls.OfType<PictureBox>().Count(pb => pb.Tag is int tag && (tag == 5 || tag == 6));
        }

        private void loadGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Q-Game Files (*.qgame)|*.qgame"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LoadLevelFromFile(openFileDialog.FileName);
                    InitializeGame();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading the level: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadLevelFromFile(string filePath)
        {
            try
            {
                using (var reader = new System.IO.StreamReader(filePath))
                {
                    // Read rows and columns from the file
                    int rows, columns;
                    if (int.TryParse(reader.ReadLine(), out rows) && int.TryParse(reader.ReadLine(), out columns))
                    {
                        // Create a grid based on rows and columns
                        CreateGrid(rows, columns);

                        // Read and place objects (boxes, walls, doors) on the grid
                        for (int i = 0; i < rows; i++)
                        {
                            for (int j = 0; j < columns; j++)
                            {
                                // Read the coordinates and value from the file
                                int row, col, value;
                                if (int.TryParse(reader.ReadLine(), out row) &&
                                    int.TryParse(reader.ReadLine(), out col) &&
                                    int.TryParse(reader.ReadLine(), out value))
                                {
                                    // Create and place PictureBox based on the value
                                    CreateAndPlacePictureBox(value, row, col);
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid format in the level file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading the level: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateGrid(int rows, int columns)
        {
            pictureBoxGrid = new PictureBox[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    pictureBoxGrid[i, j] = CreatePictureBox(i * 50, j * 50);
                    pnlGame.Controls.Add(pictureBoxGrid[i, j]);
                }
            }
        }

        private PictureBox CreatePictureBox(int top, int left)
        {
            var pictureBox = new PictureBox
            {
                Enabled = true,
                Visible = true,
                Width = 50,
                Height = 50,
                Location = new System.Drawing.Point(left, top),
                BorderStyle = BorderStyle.FixedSingle,
            };

            pictureBox.Click += PictureBox_Click;
            return pictureBox;
        }

        private void CreateAndPlacePictureBox(int value, int row, int col)
        {
            var pictureBox = pictureBoxGrid[row, col];

            try
            {
                SetPictureBoxImage(pictureBox, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting image for value {value}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetPictureBoxImage(PictureBox pictureBox, int value)
        {
            switch (value)
            {
                case 1:
                    pictureBox.Image = null;
                    pictureBox.Tag = 1;
                    break;
                case 2:
                    pictureBox.Image = Properties.Resources.Brickwall;
                    pictureBox.Tag = 2;
                    break;
                case 3:
                    pictureBox.Image = Properties.Resources.RedDoor;
                    pictureBox.Tag = 3;
                    break;
                case 4:
                    pictureBox.Image = Properties.Resources.GreenDoor;
                    pictureBox.Tag = 4;
                    break;
                case 5:
                    pictureBox.Image = Properties.Resources.RedBox;
                    pictureBox.Tag = 5;
                    remainingBoxesCount++;
                    break;
                case 6:
                    pictureBox.Image = Properties.Resources.GreenBox;
                    pictureBox.Tag = 6;
                    remainingBoxesCount++;
                    break;
                default:
                    break;
            }
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox clickedBox)
            {
                int value = int.Parse(clickedBox.Tag.ToString());

                if (value == 5 || value == 6)
                {
                    selectedBox = clickedBox;
                }
                else
                {
                    MessageBox.Show("Please select a box before using the controller.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void HandleMove(Direction direction)
        {
            if (selectedBox == null)
            {
                MessageBox.Show("Please select a box before using the controller.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int boxTag = int.Parse(selectedBox.Tag.ToString());

            // Calculate the new position based on the direction
            int newTop = selectedBox.Top;
            int newLeft = selectedBox.Left;

            switch (direction)
            {
                case Direction.Left:
                    newLeft -= 50;
                    break;
                case Direction.Right:
                    newLeft += 50;
                    break;
                case Direction.Up:
                    newTop -= 50;
                    break;
                case Direction.Down:
                    newTop += 50;
                    break;
            }

            // Check if the new position is within the bounds of the panel
            if (newTop >= 0 && newTop < pnlGame.Height && newLeft >= 0 && newLeft < pnlGame.Width)
            {
                // Check for collisions with walls or doors
                Control[] collisions = pnlGame.Controls.Cast<Control>()
                    .Where(c => c.Bounds.IntersectsWith(new Rectangle(newLeft, newTop, selectedBox.Width, selectedBox.Height)))
                    .ToArray();

                bool isCollisionWithWall = collisions.Any(c => int.Parse(c.Tag.ToString()) == 2);
                bool isCollisionWithRedDoor = boxTag == 5 && collisions.Any(c => int.Parse(c.Tag.ToString()) == 3 && c.Visible);
                bool isCollisionWithGreenDoor = collisions.Any(c => boxTag == 6 && int.Parse(c.Tag.ToString()) == 4);

                if (isCollisionWithWall)
                {
                    // Collision with a wall, do nothing
                }
                else if (isCollisionWithRedDoor && boxTag == 5)
                {
                    // Red box moving into a red door, allow the move and make the red box disappear
                    selectedBox.Top = newTop;
                    selectedBox.Left = newLeft;
                    selectedBox.Visible = false;
                    remainingBoxesCount--;

                    // Check if both boxes have entered their doors
                    if (remainingBoxesCount == 0)
                    {
                        ShowGameEndMessage();
                        InitializeGame();
                    }
                    else
                    {
                        // Switch to the other box after the first one reaches the goal
                        selectedBox = remainingBoxesCount == 1
                            ? pnlGame.Controls.OfType<PictureBox>().FirstOrDefault(c => (int)c.Tag == 6)
                            : pnlGame.Controls.OfType<PictureBox>().FirstOrDefault(c => (int)c.Tag == 5);
                    }
                }
                else if (isCollisionWithGreenDoor && boxTag == 6)
                {
                    // Green box moving into a green door, allow the move and make the green box disappear
                    selectedBox.Top = newTop;
                    selectedBox.Left = newLeft;
                    selectedBox.Visible = false;
                    remainingBoxesCount--;

                    // Check if both boxes have entered their doors
                    if (remainingBoxesCount == 0)
                    {
                        ShowGameEndMessage();
                        InitializeGame();
                    }
                    else
                    {
                        // Switch to the other box after the first one reaches the goal
                        selectedBox = remainingBoxesCount == 1
                            ? pnlGame.Controls.OfType<PictureBox>().FirstOrDefault(c => (int)c.Tag == 6)
                            : pnlGame.Controls.OfType<PictureBox>().FirstOrDefault(c => (int)c.Tag == 5);
                    }
                }
                else
                {
                    // Allow the move for the selected box if there is no collision with walls or doors
                    selectedBox.Top = newTop;
                    selectedBox.Left = newLeft;
                }
            }

            movesCount++;
            UpdateScores();
        }


        private void ShowGameEndMessage()
        {
            MessageBox.Show("Congratulations! Level Completed.", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            HandleMove(Direction.Left);
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            HandleMove(Direction.Right);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            HandleMove(Direction.Up);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            HandleMove(Direction.Down);
        }

        private void closeGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ControlPanelForm newControlPanelForm = new ControlPanelForm();
            newControlPanelForm.Show();

            Visible = false;
        }

        public enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }
    }
}