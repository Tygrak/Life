using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Life{
    public class Canvas{
        public GameWindow window;
        public Life lif;
        public Canvas(int width, int height){
            this.window = new GameWindow(width, height, GraphicsMode.Default, "Life");
        }

        public void initialize(){
            GL.ClearColor(0,0,0,0);
            window.Resize += resizeWindow;
            window.RenderFrame += renderFrame;
            window.Run(20, 20);
        }

        void renderFrame(object obj, EventArgs args){
            GL.Clear(ClearBufferMask.ColorBufferBit);
            lif.nextGeneration();
            lif.drawGrid();
            window.Title = "Life Generation: " + lif.generation.ToString();
            window.SwapBuffers();
        }

        void resizeWindow(object obj, EventArgs args){
            GL.Viewport(0, 0, window.Width, window.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, 100, 0, 100, -10, 10);
            GL.MatrixMode(MatrixMode.Modelview);
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