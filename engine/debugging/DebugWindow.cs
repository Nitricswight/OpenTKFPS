using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTKFPS.engine.singletons;

namespace OpenTKFPS.engine.debugging
{
    public class DebugWindow
    {

        public virtual void Update(){

        }

        public virtual void Close(){
            DebugWindowManager.RemoveDebugWindow(this);
        }
    }
}