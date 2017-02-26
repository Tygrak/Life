using System;
using System.Drawing;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Life{
    public class Canvas{
        public GameWindow window;
        public Life lif;
        public int width;
        public int height;
        public double camSize = 100;
        public Vector2d camPos = new Vector2d(50, 50);
        public bool paused = false;
        KeyboardState keyboardState, lastKeyboardState;
        MouseState mouseState, lastMouseState;
        public Canvas(int width, int height){
            this.window = new GameWindow(width, height, GraphicsMode.Default, "Life");
            this.height = height;
            this.width = width;
            window.WindowBorder = WindowBorder.Fixed;
        }

        public void initialize(){
            GL.ClearColor(0,0,0,0);
            window.Resize += resizeWindow;
            window.RenderFrame += renderFrame;
            window.UpdateFrame += updateFrame;
            window.Run(20, 20);
            //window.Run();
        }

        void renderFrame(object obj, EventArgs args){
            GL.Clear(ClearBufferMask.ColorBufferBit);
            if(!paused){
                lif.nextGeneration();
            }
            lif.drawGrid();
            window.Title = "Life Generation: " + lif.generation.ToString();
            GL.Finish();
            window.SwapBuffers();
        }

        void updateFrame(object obj, EventArgs args){
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetCursorState();
            if(keyPress(Key.Q)){
                zoomCam(-5);
            }
            if(keyPress(Key.E)){
                zoomCam(5);
            }
            if(keyPress(Key.W)){
                moveCam(0, 5);
            }
            if(keyPress(Key.A)){
                moveCam(-5, 0);
            }
            if(keyPress(Key.S)){
                moveCam(0, -5);
            }
            if(keyPress(Key.D)){
                moveCam(5, 0);
            }
            if(keyPress(Key.Space)){
                paused = !paused;
            }
            if(keyPress(Key.I)){
                if(paused){
                    makeImage();
                }
            }
            if(keyPress(Key.N)){
                if(paused){
                    lif.nextGeneration();
                }
            }
            if(keyPress(Key.F4)){
                window.Exit();
            }
            if(keyPress(Key.Escape)){
                string input = Console.ReadLine();
                input = input.ToLower();
                if(input == "clear"){
                    lif.clearGrid();
                    paused = true;
                } else if(input.StartsWith("randomise")){
                    if(input.Contains(" ")){
                        input = input.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries)[1];
                        int a = 0;
                        if(int.TryParse(input, out a)){
                            lif.randomiseGrid(90, a);
                        } else{
                            lif.randomiseGrid(90);
                        }
                    } else{
                        lif.randomiseGrid(90);
                    }
                    paused = true;
                } else if(input.StartsWith("setsize") || input.StartsWith("gridsize")){
                    if(input.Contains(" ")){
                        input = input.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries)[1];
                        int a = 0;
                        if(int.TryParse(input, out a)){
                            lif.setGridSize(a);
                        }
                    }
                    paused = true;
                } else if(input.StartsWith("setrule")){
                    if(input.Contains(" ")){
                        input = input.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries)[1];
                        string[] rules = input.Split(new string[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
                        lif.ruleSpawn = new int[rules[0].Length-1];
                        lif.ruleStay = new int[rules[1].Length-1];
                        for (int i = rules[0].Length-1; i > 0; i--){
                            int a = 0;
                            if(int.TryParse(rules[0][i].ToString(), out a)){
                                lif.ruleSpawn[i-1] = a;
                            }
                        }
                        for (int i = rules[1].Length-1; i > 0; i--){
                            int a = 0;
                            if(int.TryParse(rules[1][i].ToString(), out a)){
                                lif.ruleStay[i-1] = a;
                            }
                        }
                    }
                    paused = true;
                } else if(input == "seed"){
                    Console.WriteLine(lif.seed);
                } else if(input == "quit" || input == "exit"){
                    window.Exit();
                } else if(input == "pause"){
                    paused = !paused;
                }
            }
            if(mousePress(MouseButton.Left)){
                if(paused){
                    intVector2 mousePos = getMousePosition();
                    //Console.WriteLine("Mouse Position: " + mousePos.x.ToString() + ", " + mousePos.y.ToString());
                    if(mousePos.x > 0 && mousePos.x < width && mousePos.y > 0 && mousePos.y < height){
                        Vector2 worldPos = screenToWorldPosition(mousePos);
                        mousePos.x = (int) Math.Round(worldPos.X/lif.squareSize - 0.5);
                        mousePos.y = (int) Math.Round(worldPos.Y/lif.squareSize - 0.5);
                        if(mousePos.x > -1 && mousePos.x < lif.gridSize && mousePos.y > -1 && mousePos.y < lif.gridSize){
                            if(lif.grid[mousePos.x, mousePos.y] == 0){
                                lif.grid[mousePos.x, mousePos.y] = 1;
                            } else{
                                lif.grid[mousePos.x, mousePos.y] = 0;
                            }
                        }
                    }
                }
            }
            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;
        }

        public bool keyPress(Key key){
            return (keyboardState[key] && (keyboardState[key] != lastKeyboardState[key]));
        }

        public bool mousePress(MouseButton button){
            return (mouseState[button] && (mouseState[button] != lastMouseState[button]));
        }

        public intVector2 getMousePosition(){
            intVector2 pos = new intVector2();
            pos.x = mouseState.X - window.X;
            pos.y = mouseState.Y - window.Y - 25; //TODO: Remove magic
            return pos;
        }

        public Vector2 screenToWorldPosition(intVector2 screenPos){
            screenPos.y = -(screenPos.y - height);
            Vector2 worldPos = new Vector2();
            worldPos.X = (float) (screenPos.x / (width/camSize) + camPos.X-camSize/2);
            worldPos.Y = (float) (screenPos.y / (width/camSize) + camPos.Y-camSize/2);
            return worldPos;
        }

        public Vector2 screenToWorldPosition(int x, int y){
            Vector2 worldPos = new Vector2();
            worldPos.X = (float) (x / (width/camSize) + camPos.X-camSize/2);
            worldPos.Y = (float) (y / (width/camSize) + camPos.Y-camSize/2);
            return worldPos;
        }

        void updateCam(){
            changeOrtho(camPos.X-camSize/2, camPos.Y-camSize/2, camPos.X+camSize/2, camPos.Y+camSize/2);
        }

        void zoomCam(double amount){
            if(camSize+amount<=100 && camSize+amount>=10){
                camSize += amount;
                if(camPos.X + camSize/2 > 100){
                    camPos.X = 100 - camSize/2;
                } else if(camPos.X - camSize/2 < 0){
                    camPos.X = 0 + camSize/2;
                }
                if(camPos.Y + camSize/2 > 100){
                    camPos.Y = 100 - camSize/2;
                } else if(camPos.Y - camSize/2 < 0){
                    camPos.Y = 0 + camSize/2;
                }
                updateCam();
            }
        }

        void moveCam(double x, double y){
            if(camPos.X + x + camSize/2 > 100){
                camPos.X = 100 - camSize/2;
            } else if(camPos.X + x - camSize/2 < 0){
                camPos.X = 0 + camSize/2;
            } else{
                camPos.X += x;
            }
            if(camPos.Y + y + camSize/2 > 100){
                camPos.Y = 100 - camSize/2;
            } else if(camPos.Y + y - camSize/2 < 0){
                camPos.Y = 0 + camSize/2;
            } else{
                camPos.Y += y;
            }
            updateCam();
        }

        public void changeOrtho(double x1, double y1, double x2, double y2){
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(x1, x2, y1, y2, -10, 10);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        void resizeWindow(object obj, EventArgs args){
            GL.Viewport(0, 0, window.Width, window.Height);
            updateCam();
        }

        public void drawSquare(float x1, float y1, float x2, float y2){
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(1.00, 1.00, 1.00);
            GL.Vertex2(x1, y1);
            GL.Vertex2(x2, y1);
            GL.Vertex2(x2, y2);
            GL.Vertex2(x1, y2);
            GL.End();
        }

        public void makeImage(){
            int w = window.Width;
            int h = window.Height;
            Bitmap bmp = new Bitmap(w, h);
            System.Drawing.Imaging.BitmapData data = 
                  bmp.LockBits(new Rectangle(0, 0, window.Width, window.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, w, h, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            bmp.Save("image.png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }

    public class intVector2{
        public int x;
        public int y;
        public intVector2(int x, int y){
            this.x = x;
            this.y = y;
        }

        public intVector2(){
            this.x = 0;
            this.y = 0;
        }
    }
}