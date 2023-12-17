using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OpenTKFPS.engine.structure.actors;

namespace OpenTKFPS.engine.structure
{
    public class Scene
    {
        public Actor root;

        public Scene(){

        }

        public virtual void Begin(){
            root = new Actor();
            root.Begin();
        }

        public void Update(float deltaTime){
            //Debug.WriteLine("updating scene");
            root.Update(deltaTime);
        }

        public void Render(float deltaTime){
            root.Render(deltaTime);
        }

        public void End(){
            root.End();
        }
    }
}