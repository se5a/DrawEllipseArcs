using ImGuiNET;
using ImGuiSDL2CS;
using SDL2;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;
using System.Runtime.CompilerServices;


namespace DrawEllipse;

  public class GlobalUIState
    {
        public bool debugnewgame = true;
        //internal PulsarGuiWindow distanceRulerWindow { get; set; }


        //internal FactionVM FactionUIState;
        

        internal bool ShowMetrixWindow;
        internal bool ShowImgDbg;
        internal bool ShowDemoWindow;
        internal bool ShowDamageWindow;
        internal IntPtr rendererPtr;
        internal Guid _lastContextMenuOpenedEntityGuid = Guid.Empty;




        internal DateTime PrimarySystemDateTime; //= new DateTime();



        //internal Camera Camera;// = new Camera();
        internal ImGuiSDL2CSWindow ViewPort;
        internal System.Numerics.Vector2 MainWinSize { get {return ViewPort.Size;}}


        internal List<float> DrawNameZoomLvl = new List<float>();

        internal Dictionary<string, IntPtr> SDLImageDictionary = new Dictionary<string, IntPtr>();
        internal Dictionary<string, int> GLImageDictionary = new Dictionary<string, int>();






        internal GlobalUIState(ImGuiSDL2CSWindow viewport)
        {
            ViewPort = viewport;

            var windowPtr = viewport.Handle;
            SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_DRIVER, "opengl");
            //var surfacePtr = SDL.SDL_GetWindowSurface(windowPtr);
            rendererPtr = SDL.SDL_CreateRenderer(windowPtr, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);





            //Camera = new Camera(viewport);







        }


        //checks wether any event changed the mouse position after a new mouse click, indicating the user is doing something else with the mouse as he was doing before.
        internal void onFocusMoved(){
            _lastContextMenuOpenedEntityGuid = Guid.Empty;
        }

    }