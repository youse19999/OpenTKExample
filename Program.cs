using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace OpenTK2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using (Game game = new Game(800, 600, "LearnOpenTK"))
            {
                game.Run();
            }
        }
    }
    public class Game : GameWindow
    {
        public int VertixBufferObject;
        public int VertexArrayObject;
        Shader shader;
        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title })
        {
            
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            KeyboardState input = KeyboardState;
            if(input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            SwapBuffers();
        }
        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            VertixBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertixBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Meshes.Triangle.vertices.Length * sizeof(float), Meshes.Triangle.vertices, BufferUsageHint.StaticDraw);
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            shader = new Shader("shader.vert", "shader.frag");
            shader.Use();
        }
        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            GL.DeleteBuffer(VertixBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteProgram(shader.Handle);
            shader.Dispose();
        }
    }
    public class Shader
    {
        public int Handle;
        public int VertexShader;
        public int FragmentShader;
        private bool disposedValue = false;
        public Shader(string vertexPath,string fragmentPath)
        {
            string VertexShaderSource = File.ReadAllText(vertexPath);
            string FragmentShaderSource = File.ReadAllText(fragmentPath);

            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);
            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);
            GL.CompileShader(VertexShader);
            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int success);
            if(success == 0)
            {
                //エラーログの出力の場所（バーテクス）
                string infoLog = GL.GetShaderInfoLog(VertexShader);
                Console.WriteLine(infoLog);
            }

            GL.CompileShader(FragmentShader);

            GL.GetShader(FragmentShader,ShaderParameter.CompileStatus,out success);//docsには、out int successと書いてあるけど、再宣言できないのでout successsに変更
            if(success == 0)
            {
                //エラーログの出力の場所（フラグメント）
                string infolog = GL.GetShaderInfoLog(FragmentShader);
                Console.WriteLine(infolog);
            }
            //ハンドル（対象のコンピュータなどに、一定の情報を握らせること）の処理
            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out success);//docsには、out int successと書いてあるけど、再宣言できないのでout successsに変更
            if (success == 0)
            {
                //エラーログの出力の場所（ハンドル）
                string infoLog = GL.GetProgramInfoLog(Handle);
                Console.WriteLine(infoLog);
            }
            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }
        public void Use()
        {
            GL.UseProgram(Handle);
        }
        protected virtual void Dispose(bool disposing)
        {
            //Disposeしてないなら
            if(!disposing)
            {
                GL.DeleteProgram(Handle);
            }
        }
        ~Shader()
        {
            if (disposedValue == false)
            {
                Console.WriteLine("Dispose処理がされてません！まったく、ちゃんとDispose処理を事前に行ってください！");
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
    public class Meshes
    {
        public static class Triangle
        {
            public static float[] vertices =
            {
                -0.5f,-0.5f,0.0f,
                0.5f,-0.5f,0.0f,
                0.0f,0.5f,0.0f
            };
        }
    }
}
