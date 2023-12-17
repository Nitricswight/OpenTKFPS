using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTKFPS.engine.debugging;

namespace OpenTKFPS.engine.singletons
{
    public static class DebugWindowManager
    {
        private static List<DebugWindow> debugWindows = new List<DebugWindow>();

        private static bool showWindows = true;

        public static void AddDebugWindow(DebugWindow window){
            debugWindows.Add(window);
        }

        public static void RemoveDebugWindow(DebugWindow window){
            debugWindows.Remove(window);
        }

        public static void UpdateAllWindows(float deltaTime){
            if(InputManager.IsKeyJustPressed(Keys.Tab)){
                showWindows = !showWindows;
            }

            if(!showWindows){
                return;
            }

            foreach(DebugWindow window in debugWindows){
                window.Update();
            }
        }

        public static DebugWindow GetDebugWindow(Type windowType){
            foreach(DebugWindow dw in debugWindows){
                if(dw.GetType() == windowType){
                    return dw;
                }
            }

            return null;
        }
    }
}