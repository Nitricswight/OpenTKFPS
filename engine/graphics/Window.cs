using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTKFPS.engine.structure;
using System.Diagnostics;
using ImGuiNET;
using OpenTKFPS.engine.debugging;
using OpenTKFPS.engine.singletons;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenTKFPS.engine.graphics
{
    public class MyWindowSettings : NativeWindowSettings{
        public MyWindowSettings(int width, int height, string title){
            ClientSize = new Vector2i(width, height);
            Title = title;
        }
    }

    public class Window : GameWindow
    {
        Scene? scene = null;

        ImGuiController imguiController;


        public Window(int width, int height, string title, Scene? startScene = null) : base(GameWindowSettings.Default, new MyWindowSettings(width, height, title)){
            Load += Window_Load;
            UpdateFrame += WindowUpdate;
            RenderFrame += WindowRender;
            Unload += WindowUnload;
            Resize += WindowResize;
            TextInput += WindowTextInput;
            MouseWheel += WindowMouseWheel;
            CenterWindow();
            if(startScene != null){
                LoadScene(startScene);
            }
            Run();
        }

        private void WindowMouseWheel(MouseWheelEventArgs args)
        {
            imguiController.MouseScroll(args.Offset);
        }

        private void WindowTextInput(TextInputEventArgs args)
        {
            imguiController.PressChar((char)args.Unicode);
        }

        private void WindowResize(ResizeEventArgs args)
        {
            GL.Viewport(0,0,args.Width, args.Height);
            imguiController.WindowResized(args.Width, args.Height);
            
            Renderer.SetWindowSize(ClientSize);
            
        }

        private void WindowUnload()
        {
            MeshLoader.CleanUp();
            MaterialLoader.CleanUp();
            TextureLoader.CleanUp();
        }

        private void WindowRender(FrameEventArgs args)
        {
            float delta = (float)args.Time;
            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            scene?.Render(delta);

            //ImGui.DockSpaceOverViewport();
            
            
            imguiController.Render();
            ImGuiController.CheckGLError("End of frame");
            SwapBuffers();
        }

        private void WindowUpdate(FrameEventArgs args)
        {
            float delta = (float)args.Time;

            InputManager.UpdateInput(KeyboardState, MouseState);
            //Debug.WriteLine("updating window");
            //Debug.WriteLine((scene == null).ToString());
            if(scene != null){
                scene.Update(delta);
            }

            
            
            imguiController.Update(this,delta);
            DebugWindowManager.UpdateAllWindows(delta);
            TimeManager.Update(delta);
            Renderer.UpdateMatrices();
        }

        private void Window_Load()
        {
            imguiController = new ImGuiController(ClientSize.X, ClientSize.Y);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
        }

        public void LoadScene(Scene newScene){
            if(scene != null){
                scene.End();
            }

            scene = newScene;
            scene.Begin();
            Debug.WriteLine("new scene begins.");
        }
    }
}