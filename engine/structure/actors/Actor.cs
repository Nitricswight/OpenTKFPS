using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ImGuiNET;

namespace OpenTKFPS.engine.structure.actors
{
    public class Actor
    {
        public Guid id;

        public Actor parent = null;
        public enum State{
            INITIALISING,
            ACTIVE,
            PAUSED,
            DEAD
        }

        public State state = State.INITIALISING;

        List<Actor> children = new List<Actor>();
        List<Actor> toBeAdded = new List<Actor>();
        List<Actor> toBeDeleted = new List<Actor>();

        public string name;

        public Actor(string name = "Actor"){
            this.name = name;
            this.id = Guid.NewGuid();
        }

        public virtual void Begin(){
            foreach(Actor child in children){
                child.parent = this;
                child.Begin();
            }

            state = State.ACTIVE;
        }

        public virtual void Update(float deltaTime){
            if(state == State.PAUSED){
                return;
            }

            toBeAdded.Clear();
            toBeDeleted.Clear();

            foreach(Actor child in children){
                if(child.state == State.DEAD){
                    toBeDeleted.Add(child);
                    continue;
                }

                child.Update(deltaTime);
            }

            foreach(Actor a in toBeAdded){
                children.Add(a);
                a.parent = this;
                a.Begin();
            }

            foreach(Actor a in toBeDeleted){
                a.End();
                children.Remove(a);
            }
        }

        public virtual void Render(float deltaTime){
            foreach(Actor child in children){
                child.Render(deltaTime);
            }
        }

        public virtual void End(){
            foreach(Actor child in children){
                child.End();
            }
        }

        public void AddChild(Actor child){
            if(state == State.INITIALISING){
                children.Add(child);
            }
            else{
                toBeAdded.Add(child);
            }

        }

        public List<Actor> GetChildren(){
            return children;
        }

        public virtual void ExposeToInspector(){
            ImGui.Text("ID: " + id.ToString());
        }
    }
}