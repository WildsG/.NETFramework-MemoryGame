using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Match_2
{
    public partial class Form1 : Form
    {

        struct ImageForGeneration {
            public Image Image;
            public bool Placed;

            public ImageForGeneration(Image image,bool placed = false)
            {
                Image = image;
                Placed = placed;
            }
        };


        Button[,] buttons;
        ImageForGeneration[,] images;
        
        int tableSize = 4; // must be even
        List<ImageForGeneration> allImages = new List<ImageForGeneration>();
        int image1_x = -1, image1_y = -1;
        int image2_x = -1, image2_y = -1;
        int imagesOpened = 0;
        int totalOpened = 0;
        Random rnd = new Random();
        double revealTime = 3;

        public Form1()
        {
            InitializeComponent();
            GenerateImages();
            GenerateTable();
        }

        private void GenerateImages()
        {
            var images = Properties.Resources.ResourceManager
                        .GetResourceSet(CultureInfo.CurrentCulture, true, true);
            foreach (DictionaryEntry entry in images)
                allImages.Add(new ImageForGeneration((Image)entry.Value));

            this.images = new ImageForGeneration[tableSize, tableSize];
            foreach(ImageForGeneration ifg in allImages.OrderBy(x => rnd.Next()).Take(8))
            {
                PlaceTile(ifg);
                PlaceTile(ifg);
            }
        }

        private void PlaceTile(ImageForGeneration imageToSet)
        {
            int x;
            int y;
            do
            {
                x = rnd.Next(0, 4);
                y = rnd.Next(0, 4);
            } while (images[x, y].Placed);
            images[x, y].Image = imageToSet.Image;
            images[x, y].Placed = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            revealTime--;
            if(revealTime <=0)
            {
                CloseOpened();
            }
        }

        private void GenerateTable()
        {
            Rectangle screenRectangle = RectangleToScreen(ClientRectangle);
            int titleHeight = screenRectangle.Top - Top;
            Size = new Size(400 + 16, 400 + titleHeight + 8);
            buttons = new Button[tableSize, tableSize];
            for (int i = 0; i < tableSize; i++)
            {
                for (int j = 0; j < tableSize; j++)
                {
                    Button btn = new Button
                    {
                        Size = new Size(100, 100),
                        Location = new Point(i * 100, j * 100),
                        Text = "",
                    };
                    buttons[i, j] = btn;
                    btn.Click += new EventHandler(Button_Click);
                    Controls.Add(btn);
                }
            }
        }

        private void CloseOpened()
        {
            if(image1_x >= 0 && image1_y >= 0 && image2_x >= 0 && image1_y >= 0)
            {
                if (images[image1_x, image1_y].Image == images[image2_x, image2_y].Image)
                    return;

                buttons[image1_x, image1_y].Image = null;
                buttons[image2_x, image2_y].Image = null;
                buttons[image1_x, image1_y].Click += Button_Click;
                buttons[image2_x, image2_y].Click += Button_Click;
                image1_x = -1;
                image1_y = -1;
                image2_x = -1;
                image2_y = -1;
                timer.Stop();
            }
           
        }

        private void Restart()
        {
            Controls.Clear();
            GenerateImages();
            GenerateTable();
            totalOpened = 0;
            image1_x = -1;
            image1_y = -1;
            image2_x = -1;
            image2_y = -1;
            allImages.Clear();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            for (int x = 0; x < tableSize; x++)
            {
                for (int y = 0; y < tableSize; y++)
                {
                    if (buttons[x, y].Equals(btn))
                    {
                        imagesOpened++;
                        switch (imagesOpened)
                        {
                            case 1:
                                CloseOpened();
                                image1_x = x;
                                image1_y = y;
                                break;
                            case 2:
                                image2_x = x;
                                image2_y = y;
                                break;
                        }
                        buttons[x, y].Click -= Button_Click;
                        buttons[x, y].Image = images[x, y].Image;
                    }
                }
            }
            if(imagesOpened == 2)
            {
                if (images[image1_x, image1_y].Image == images[image2_x, image2_y].Image)
                {
                    totalOpened += 2;
                    if(totalOpened == tableSize * tableSize)
                    {
                        if (MessageBox.Show("You Win", "Play Again?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            Restart();
                        else
                            Application.Exit();

                    }
                }
                else
                {
                    revealTime = 3;
                    timer.Start();
                }
                imagesOpened = 0;
            }
        }
    }
}
