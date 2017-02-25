using System;
//using System.Drawing;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Life{
    public class Canvas{
        public GameWindow window;
        public Life lif;
        public double camSize = 100;
        public Vector2d camPos = new Vector2d(50, 50);
        public bool paused = false;
        KeyboardState keyboardState, lastKeyboardState;
        public Canvas(int width, int height){
            this.window = new GameWindow(width, height, GraphicsMode.Default, "Life");
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
            if(keyPress(Key.N)){
                if(paused){
                    lif.nextGeneration();
                }
            }
            lastKeyboardState = keyboardState;
        }

        public bool keyPress(Key key){
            return (keyboardState [key] && (keyboardState [key] != lastKeyboardState [key]) );
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
    }
}