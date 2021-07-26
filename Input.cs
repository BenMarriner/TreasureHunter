using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TreasureHunter
{
    // Keys that both the editor and game use
    interface IEssentialKeyEvents
    {
        public void SpacebarPressed();
    }

    // Keys that are specific to the editor
    interface IEditorKeyEvents : IEssentialKeyEvents
    {
        public void UpArrowPressed();
        public void DownArrowPressed();
        public void LeftArrowPressed();
        public void RightArrowPressed();
        public void OneKeyPressed();
        public void TwoKeyPressed();
        public void ThreeKeyPressed();
        public void FourKeyPressed();
        public void FiveKeyPressed();
        public void SKeyPressed();
        public void TKeyPressed();
    }

    // Keys that are specific to the game
    interface IGameKeyEvents: IEssentialKeyEvents
    {
        public void UpArrowDown();
        public void DownArrowDown();
        public void LeftArrowDown();
        public void RightArrowDown();
        public void RKeyPressed();
    }

    static class Input
    {
        // List of all objects that are listening out for keystrokes
        private static List<object> EventListeners { get; } = new List<object>();

        // Calls the Splashkit procedure for processing keypresses, then checks which keys are being pressed
        public static void ProcessInput()
        {
            SplashKit.ProcessEvents();

            foreach (object listener in EventListeners)
            {
                // Essential Key Events
                if (SplashKit.KeyTyped(KeyCode.SpaceKey))
                    (listener as IEssentialKeyEvents).SpacebarPressed();

                switch (GlobalSettings.ProgramMode)
                {
                    // Editor Key Events
                    case 0:
                        if (SplashKit.KeyTyped(KeyCode.UpKey))
                            (listener as IEditorKeyEvents).UpArrowPressed();

                        if (SplashKit.KeyTyped(KeyCode.DownKey))
                            (listener as IEditorKeyEvents).DownArrowPressed();

                        if (SplashKit.KeyTyped(KeyCode.LeftKey))
                            (listener as IEditorKeyEvents).LeftArrowPressed();

                        if (SplashKit.KeyTyped(KeyCode.RightKey))
                            (listener as IEditorKeyEvents).RightArrowPressed();

                        if (SplashKit.KeyTyped(KeyCode.Num1Key))
                            (listener as IEditorKeyEvents).OneKeyPressed();

                        if (SplashKit.KeyTyped(KeyCode.Num2Key))
                            (listener as IEditorKeyEvents).TwoKeyPressed();

                        if (SplashKit.KeyTyped(KeyCode.Num3Key))
                            (listener as IEditorKeyEvents).ThreeKeyPressed();

                        if (SplashKit.KeyTyped(KeyCode.Num4Key))
                            (listener as IEditorKeyEvents).FourKeyPressed();

                        if (SplashKit.KeyTyped(KeyCode.Num5Key))
                            (listener as IEditorKeyEvents).FiveKeyPressed();

                        if (SplashKit.KeyTyped(KeyCode.SKey))
                            (listener as IEditorKeyEvents).SKeyPressed();

                        if (SplashKit.KeyTyped(KeyCode.TKey))
                            (listener as IEditorKeyEvents).TKeyPressed();
                        break;
                    // Game Key Events
                    case 1:
                        if (SplashKit.KeyDown(KeyCode.UpKey))
                            (listener as IGameKeyEvents).UpArrowDown();

                        if (SplashKit.KeyDown(KeyCode.DownKey))
                            (listener as IGameKeyEvents).DownArrowDown();

                        if (SplashKit.KeyDown(KeyCode.LeftKey))
                            (listener as IGameKeyEvents).LeftArrowDown();

                        if (SplashKit.KeyDown(KeyCode.RightKey))
                            (listener as IGameKeyEvents).RightArrowDown();

                        if (SplashKit.KeyTyped(KeyCode.RKey))
                            (listener as IGameKeyEvents).RKeyPressed();
                        break;
                }
            }     
        }

        // Subscribe an object that inherits the key event interfaces as a listener
        public static void AddListener(object listener)
        {
            if (listener is IEssentialKeyEvents)
                EventListeners.Add(listener);
        }

        // Unsubscribe a listener
        public static void RemoveListener(object listener)
        {
            if (EventListeners.Contains(listener))
                EventListeners.Remove(listener);
        }
    }
}
