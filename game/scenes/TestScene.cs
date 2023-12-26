using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTKFPS.engine.debugging;
using OpenTKFPS.engine.graphics;
using OpenTKFPS.engine.singletons;
using OpenTKFPS.engine.structure;
using OpenTKFPS.engine.structure.actors;
using OpenTKFPS.engine.structure.actors.renderableActors;
using OpenTKFPS.game.customActors;

namespace OpenTKFPS.game.scenes
{
    public class TestScene : Scene
    {
        float[] vertices = {
            -0.5f,0.5f,0.0f,
            0.5f,-0.5f,0.0f,
            -0.5f,-0.5f,0.0f,
            0.5f,0.5f,0.0f
        };

        float[] uvs = {
            0.0f,0.0f,
            1.0f,1.0f,
            0.0f,1.0f,
            1.0f,0.0f
        };

        uint[] indices = {
            0,1,2,3,1,0
        };

        Mesh quad;
        StandardMaterial sMat;

        public override void Begin()
        {
            quad = MeshLoader.LoadMesh(vertices, uvs,null, indices);
            sMat = new StandardMaterial(Color4.White, "assets/textures/Image_0.png");

            root = new Actor();
            ViewportActor viewport = new ViewportActor(200,112,true, Color4.CornflowerBlue);

            Actor3D trees = SceneLoader.AssimpLoadScene("assets/models/bunny.fbx");
            trees.local_scale = Vector3.One * 1f;

            viewport.AddChild(trees);




            FlyCam cam = new FlyCam(new Vector3(0,0,-3f), Vector3.One, Vector3.Zero);
            viewport.AddChild(cam);
            viewport.AddChild(new MeshActor3D(quad, sMat, Vector3.Zero, Vector3.Zero, Vector3.One));
            cam.setCurrent();
            root.AddChild(viewport);
            root.Begin();

            

            DebugWindowManager.AddDebugWindow(new TreeDebugger(this));
            DebugWindowManager.AddDebugWindow(new ActorInspector());
            DebugWindowManager.AddDebugWindow(new CheatsWindow());
            DebugWindowManager.AddDebugWindow(new ProfilerWindow(true));
        }
    }
}