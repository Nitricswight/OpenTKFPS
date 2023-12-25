using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTKFPS.engine.graphics;
using OpenTKFPS.engine.singletons;
using OpenTKFPS.engine.structure.actors;

namespace OpenTKFPS.engine.structure
{
    public class ViewportActor : Actor
    {
        public int framebufferID;
        public int colourTexture;
        public int depthTexture;

        public Color4 clearColour;

        public bool keepAspect;

        public int width, height;

        int windowWidth, windowHeight;

        public ViewportActor(int resolutionX, int resolutionY, bool keepAspect, Color4 clear, string name = "Viewport") : base(name){
            this.width = resolutionX;
            this.height = resolutionY;
            this.keepAspect = keepAspect;
            this.clearColour = clear;
            framebufferID = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferID);

            colourTexture = TextureLoader.loadEmptyTexture(resolutionX, resolutionY, TextureMinFilter.Nearest, TextureMagFilter.Nearest, TextureWrapMode.ClampToEdge);

            //Debug.WriteLine("colTex: " + colourTexture.ToString());

            

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, colourTexture, 0);

            depthTexture = TextureLoader.loadEmptyTexture(resolutionX, resolutionY, TextureMinFilter.Nearest, TextureMagFilter.Nearest, TextureWrapMode.ClampToEdge, PixelInternalFormat.DepthComponent24, PixelFormat.DepthComponent);
            
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depthTexture, 0);
            

            FramebufferErrorCode error = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if(error != FramebufferErrorCode.FramebufferComplete){
                Debug.WriteLine(error.ToString());
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        }

        public override void Render(float deltaTime)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferID);
            GL.Viewport(0,0,width,height);
            GL.ClearColor(clearColour);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //Debug.WriteLine("viewport render");
            base.Render(deltaTime);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0,0,Renderer.windowSize.X,Renderer.windowSize.Y);

            Renderer.DrawScreenQuad(this);
        }

        public override void End()
        {
            GL.DeleteFramebuffer(framebufferID);
            base.End();
        }

        public override void ExposeToInspector()
        {
            base.ExposeToInspector();
            ImGui.Checkbox("keepAspect", ref keepAspect);
        }
    }
}