﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;

namespace TheGame
{
    


    public class image
    {
        public String file;
        public int x, y;
        public float w, h;
        Texture texture;
        public Sprite sprite;
        public image(String name, int X, int Y, float W, float H)
        {
            file = name;
            x = X;
            y = Y;
            w = W;
            h = H;
            texture = new Texture("textures\\" + file);
            sprite = new Sprite(texture);
            sprite.Scale = new SFML.System.Vector2f(w, h);
            sprite.Position = new SFML.System.Vector2f(x, y);
        }
    }


    class Program
    {
        static RenderWindow win;
        

        static void Main(string[] args)
        {
            Vector2f winSize = new Vector2f(SFML.Window.VideoMode.DesktopMode.Width, SFML.Window.VideoMode.DesktopMode.Height);
            Vector2f picSize = new Vector2f(1440.0f, 900.0f);
            int i=0,j=0;
            image pixel = new image("pix.png",0,0,1,1);image bg = new image("bg.png",0,0,1,1);
            bg.sprite.Scale = new Vector2f(winSize.X/picSize.X, winSize.Y / picSize.Y);
            win = new RenderWindow(new SFML.Window.VideoMode((uint)winSize.X, (uint)winSize.Y), "TheGame",SFML.Window.Styles.Fullscreen);//style:SFML.Window.Styles.Fullscreen
            win.SetVerticalSyncEnabled(true);
            Font f = new Font("18063.ttf");
            Text text=new Text("",f,50);
            
            text.Color = new Color(252,215,94);
            
            text.Position = new SFML.System.Vector2f(win.Size.X/2-300,win.Size.Y/2);

            SFML.System.Clock timer = new SFML.System.Clock();
            SFML.System.Clock timer2 = new SFML.System.Clock();

            win.SetMouseCursorVisible(true);
            int _terrainPoints = 2048;double _roughness,_seed,_seed2,_roughness2; int DATA_SIZE = _terrainPoints + 1;  // dolzhna byt' stepen' dvoiki + 1
            double[,] data = new double[DATA_SIZE, DATA_SIZE]; double[,] biom = new double[DATA_SIZE,DATA_SIZE];

            string n;
            FileStream save = new FileStream("textures\\save.txt", FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader save_read = new StreamReader(save); 
            _roughness = Convert.ToDouble(save_read.ReadLine());
            _seed= Convert.ToDouble(save_read.ReadLine());
            _roughness2 = Convert.ToDouble(save_read.ReadLine());
            _seed2 = Convert.ToDouble(save_read.ReadLine());
            save.Close(); 
            save_read.Close(); 
            int count = 0;bool start = true;
            win.Clear(Color.Black);
            win.Draw(bg.sprite);
            win.Display();
            while (win.IsOpen)
            {
                if (start) {
                    win.Clear(Color.Black);
                    win.Draw(bg.sprite);
                    win.Display();
                }
                win.DispatchEvents();
                if (SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.F))
                {
                    start = false;
                    FileStream n_file = new FileStream("textures\\n.txt", FileMode.OpenOrCreate, FileAccess.Read);
                    StreamReader n_read = new StreamReader(n_file);
                    n = Convert.ToString(n_read.ReadLine());
                    n_file.Close();
                    n_read.Close();
                    FileStream n_2 = new FileStream("textures\\n.txt", FileMode.Truncate);
                    StreamWriter n_w = new StreamWriter(n_2); n_w.WriteLine(Convert.ToString(Convert.ToInt32(n)+1));
                    n_w.Close(); n_2.Close();
                    count = 0;
                    win.Clear();win.Display();
                    biom[0, 0] = biom[0, DATA_SIZE - 1] = biom[DATA_SIZE - 1, 0] = biom[DATA_SIZE - 1, DATA_SIZE - 1] = _seed2;
                    data[0, 0] = data[0, DATA_SIZE - 1] = data[DATA_SIZE - 1, 0] = data[DATA_SIZE - 1, DATA_SIZE - 1] = _seed;
                    double h = _roughness,h2=_roughness2;
                    Random r = new Random();

                    for (int sideLength = DATA_SIZE - 1;
                        sideLength >= 2;
                        sideLength /= 2, h /= 2.0)
                    {
                        int halfSide = sideLength / 2;
                        for (int x = 0; x < DATA_SIZE - 1; x += sideLength)
                        {
                            for (int y = 0; y < DATA_SIZE - 1; y += sideLength)
                            {
                                count++;
                                double avg = data[x, y] + data[x + sideLength, y] + data[x, y + sideLength] + data[x + sideLength, y + sideLength];
                                double avg2 = biom[x, y] + biom[x + sideLength, y] + biom[x, y + sideLength] + biom[x + sideLength, y + sideLength];
                                avg /= 4.0; avg2 /= 4.0;
                                data[x + halfSide, y + halfSide] = avg + (r.NextDouble() * 2 * h) - h; biom[x + halfSide, y + halfSide] = avg2 + (r.NextDouble() * 2 * h2) - h2;
                                if (data[x + halfSide, y + halfSide] > 2.0) data[x + halfSide, y + halfSide] = 2.0; if (biom[x + halfSide, y + halfSide] > 2.0) biom[x + halfSide, y + halfSide] = 2.0;
                                if (data[x + halfSide, y + halfSide] < -2.0) data[x + halfSide, y + halfSide] = -2.0; if (biom[x + halfSide, y + halfSide] < -2.0) biom[x + halfSide, y + halfSide] = -2.0;
                                if (count % 41890 == 0)
                                {
                                    win.Clear();
                                    text.DisplayedString = "Heightmap generation " + Convert.ToString(count / 41890) + "%";
                                    win.Draw(text);
                                    win.Display();
                                }
                            }
                        }
                        for (int x = 0; x < DATA_SIZE - 1; x += halfSide)
                        {
                            for (int y = (x + halfSide) % sideLength; y < DATA_SIZE - 1; y += sideLength)
                            {
                                count++;
                                double avg = data[(x - halfSide + DATA_SIZE) % DATA_SIZE, y] + data[(x + halfSide) % DATA_SIZE, y] + data[x, (y + halfSide) % DATA_SIZE] + data[x, (y - halfSide + DATA_SIZE) % DATA_SIZE];
                                double avg2 = biom[(x - halfSide + DATA_SIZE) % DATA_SIZE, y] + biom[(x + halfSide) % DATA_SIZE, y] + biom[x, (y + halfSide) % DATA_SIZE] + biom[x, (y - halfSide + DATA_SIZE) % DATA_SIZE];
                                avg /= 4.0;avg2 /= 4.0;
                                avg = avg + (r.NextDouble() * 2 * h) - h; avg2 = avg2 + (r.NextDouble() * 2 * h2) - h2;
                                data[x, y] = avg; biom[x, y] = avg2;
                                if (x == 0) data[DATA_SIZE - 1, y] = avg; if (x == 0) biom[DATA_SIZE - 1, y] = avg2;
                                if (y == 0) data[x, DATA_SIZE - 1] = avg; if (y == 0) biom[x, DATA_SIZE - 1] = avg2;
                                if (data[x, y] > 2.0) data[x, y ] = 2.0; if (biom[x, y] > 2.0) biom[x, y] = 2.0;
                                if (data[x, y] < -2.0) data[x, y] = -2.0; if (biom[x, y] < -2.0) biom[x, y] = -2.0;
                                if (count % 41890 == 0)
                                {
                                    win.Clear();
                                    text.DisplayedString = "Heightmap generation " + Convert.ToString(count/ 41890) +"%";
                                    win.Draw(text);
                                    win.Display();
                                }
                            }
                        }
                        h2 /= 2.0;
                    }
                    count = 0;
                    RenderTexture load = new RenderTexture(2048,1024);
                    
                    RenderTexture grad = new RenderTexture(400, 100);
                    
                    Image grad_i = new Image("textures\\grad.png");
                    for (i = 0; i < 400; i++)
                    {
                        Color col = grad_i.GetPixel(Convert.ToUInt32(i), 0);
                        Color col2 = grad_i.GetPixel(Convert.ToUInt32(i), 99);
                        int R,G,B;
                        Color fin;
                        for (j = 1; j < 99; j++)
                        {
                            R = (col2.R - col.R)*j/99;
                            G = (col2.G - col.G)*j/99;
                            B = (col2.B - col.B)*j/99;
                            fin = new Color(Convert.ToByte(R+col.R), Convert.ToByte(G + col.G), Convert.ToByte(B + col.B));
                            grad_i.SetPixel(Convert.ToUInt32(i), Convert.ToUInt32(j), fin);
                        }
                    }
                    Texture grad_d = new Texture(grad_i);
                    Sprite grad_s = new Sprite(grad_d);
                    grad.Draw(grad_s);
                    grad.Texture.CopyToImage().SaveToFile("lol.png");
                    for (i = 0; i < _terrainPoints; i++)
                    {
                        for (j = 0; j < _terrainPoints; j++)
                        {
                            count++;
                            pixel.sprite.Position = new SFML.System.Vector2f(i / 2+1024, j / 2);
                            if (biom[i, j] < -0.6) pixel.sprite.TextureRect = new IntRect(new SFML.System.Vector2i(Convert.ToInt32(200.0 / 4 * (data[i, j] + 2)), 100), new SFML.System.Vector2i(1, 1));
                            if (biom[i, j] > 0.6) pixel.sprite.TextureRect = new IntRect(new SFML.System.Vector2i(Convert.ToInt32(200.0 / 4 * (data[i, j] + 2)), 199), new SFML.System.Vector2i(1, 1));
                            if (biom[i, j] <= 0.6 && biom[i, j] >= -0.6)
                            {
                                pixel.sprite.TextureRect = new IntRect(new SFML.System.Vector2i(Convert.ToInt32(200.0 / 4 * (data[i, j] + 2)), Convert.ToInt32((biom[i, j] + 0.6) * 82)+100), new SFML.System.Vector2i(1, 1));
                                
                            }
                            load.Draw(pixel.sprite);
                            pixel.sprite.Position = new SFML.System.Vector2f(i / 2, j / 2);
                            if (biom[i,j]<-0.6) pixel.sprite.TextureRect = new IntRect(new SFML.System.Vector2i(Convert.ToInt32(200.0 / 4 * (data[i, j] + 2)), 0), new SFML.System.Vector2i(1, 1));
                            if (biom[i, j] > 0.6) pixel.sprite.TextureRect = new IntRect(new SFML.System.Vector2i(Convert.ToInt32(200.0 / 4 * (data[i, j] + 2)), 99), new SFML.System.Vector2i(1, 1));
                            if (biom[i, j] <= 0.6 && biom[i, j] >= -0.6)
                            { pixel.sprite.TextureRect = new IntRect(new SFML.System.Vector2i(Convert.ToInt32(200.0 / 4 * (data[i, j] + 2)), Convert.ToInt32((biom[i, j] + 0.6) * 82)), new SFML.System.Vector2i(1, 1));
                            }
                            load.Draw(pixel.sprite);
                            if (count % 41890 == 0)
                            {

                                win.Clear();
                                text.DisplayedString = "Landscape rendering " + Convert.ToString(count / 41890) + "%";
                                win.Draw(text);
                                win.Display();
                            }
                            
                            
                            
                        }
                    }
                    win.Clear();
                    Sprite map = new Sprite(load.Texture);
                    map.TextureRect = new IntRect(0,0,1024,1024);
                    map.Position = new SFML.System.Vector2f(100, 100);
                    map.Scale = new Vector2f(0.7f,0.7f);
                    win.Draw(map); 
                    map.TextureRect = new IntRect(1024, 0, 1024, 1024);
                    map.Position = new SFML.System.Vector2f(900, 100);
                    win.Draw(map);
                    win.Display();
                    map.Texture.CopyToImage().SaveToFile("landscapes\\map"+n+".png");
                }
                if (SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.Escape))
                {
                    win.Close();
                }

            }
        }
        
    }
}