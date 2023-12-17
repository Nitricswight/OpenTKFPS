using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenTKFPS.engine.singletons
{
    public static class InputManager
    {
        private static KeyboardState keys;
        private static MouseState mouse;


        public static void UpdateInput(KeyboardState _keys, MouseState _mouse){
            keys = _keys;
            mouse = _mouse;
        }

        public static bool IsKeyJustPressed(Keys k){
            return keys.IsKeyPressed(k);
        }

        public static bool IsKeyPressed(Keys k){
            return keys.IsKeyDown(k);
        }

        public static bool IsKeyJustReleased(Keys k){
            return keys.IsKeyReleased(k);
        }

        public static bool IsMouseButtonJustPressed(MouseButton mb){
            return mouse.IsButtonPressed(mb);
        }

        public static bool IsMouseButtonPressed(MouseButton mb){
            return mouse.IsButtonDown(mb);
        }

        public static bool IsMouseButtonJustReleased(MouseButton mb){
            return mouse.IsButtonReleased(mb);
        }

        public static Vector2 GetMouseScreenPosition(){
            return mouse.Position;
        }

        public static Vector2 GetMouseDelta(){
            return mouse.Delta;
        }

        public static Vector2 GetMouseScrollDelta(){
            return mouse.ScrollDelta;
        }

        public static Vector2 GetMouseScrollPosition(){
            return mouse.Scroll;
        }
    }
}