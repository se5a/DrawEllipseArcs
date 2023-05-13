﻿// See https://aka.ms/new-console-template for more information

using System.Numerics;
using DrawEllipse;
using ImGuiNET;
using ImGuiSDL2CS;
using SDL2;

public class Program
{
    static SDL2Window Instance;
    [STAThread]
    public static void Main()
    {
        SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        Instance = new MainWindow();
        Instance.Run();
        Instance.Dispose();
    }

    public class MainWindow : ImGuiSDL2CSWindow
    {

        //private MemoryEditor _MemoryEditor = new MemoryEditor();
        //private byte[] _MemoryEditorData;

        //private FileDialog _Dialog = new FileDialog(false, false, true, false, false, false);

        Vector3 backColor = new Vector3(0 / 255f, 0 / 255f, 28 / 255f);

        int mouseDownX;
        int mouseDownY;
        int mouseDownAltX;
        int mouseDownAltY;

        private GlobalUIState _state;

        public MainWindow()
            : base("Test App")
        {

            _state = new GlobalUIState(this);
            //_uiState.MainWinSize = this.Size;
            //_uiState.ShowMetrixWindow = true;
            // Create any managed resources and set up the main game window here.
            /*
            _MemoryEditorData = new byte[1024];
            Random rnd = new Random();
            for (int i = 0; i < _MemoryEditorData.Length; i++)
            {
                _MemoryEditorData[i] = (byte)rnd.Next(255);
            }
            */
            backColor = new Vector3(0 / 255f, 0 / 255f, 28 / 255f);

            //_state.GalacticMap = new GalacticMapRender(this, _state);
            //_uiState.MapRendering = new SystemMapRendering(this, _uiState);


            OnEvent = MyEventHandler;
        }

        private bool MyEventHandler(SDL2Window window, SDL.SDL_Event e)
        {

            int mouseX;
            int mouseY;
            SDL.SDL_GetMouseState(out mouseX, out mouseY);

            if (!ImGuiSDL2CSHelper.HandleEvent(e, ref g_MouseWheel, g_MousePressed))
                return false;

            if (e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN &&
                e.button.button == 1 & !ImGui.GetIO().WantCaptureMouse)
            {
                _state.onFocusMoved();
                //_state.Camera.IsGrabbingMap = true;
                //_state.Camera.MouseFrameIncrementX = e.motion.x;
                //_state.Camera.MouseFrameIncrementY = e.motion.y;
                mouseDownX = mouseX;
                mouseDownY = mouseY;
            }

            if (e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONUP && e.button.button == 1)
            {
                //_uiState.onFocusMoved();
                //_state.Camera.IsGrabbingMap = false;

                if (mouseDownX == mouseX && mouseDownY == mouseY) //click on map.  
                {
                    //_state.MapClicked(_state.Camera.WorldCoordinate_m(mouseX, mouseY), MouseButtons.Primary);//sdl and imgu use different numbers for buttons.
                }
            }

            if (e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN &&
                e.button.button == 3 & !ImGui.GetIO().WantCaptureMouse)
            {
                _state.onFocusMoved();
                mouseDownAltX = mouseX;
                mouseDownAltY = mouseY;
            }

            if (e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONUP && e.button.button == 3)
            {
                _state.onFocusMoved();
                //_state.Camera.IsGrabbingMap = false;

                if (mouseDownAltX == mouseX && mouseDownAltY == mouseY) //click on map.  
                {
                    //_state.MapClicked(_state.Camera.WorldCoordinate_m(mouseX, mouseY), MouseButtons.Alt);//sdl and imgu use different numbers for buttons.
                }
            }
/*
            if (_state.Camera.IsGrabbingMap && e.type == SDL.SDL_EventType.SDL_MOUSEMOTION)
            {
                int deltaX = _state.Camera.MouseFrameIncrementX - e.motion.x;
                int deltaY = _state.Camera.MouseFrameIncrementY - e.motion.y;
                _state.Camera.WorldOffset_m(deltaX, deltaY);

                _state.Camera.MouseFrameIncrementX = e.motion.x;
                _state.Camera.MouseFrameIncrementY = e.motion.y;

            }
*/

            if (e.type == SDL.SDL_EventType.SDL_KEYUP)
            {
                if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE)
                {
                    //MainMenuItems mainMenu = MainMenuItems.GetInstance();
                    //mainMenu.ToggleActive();
                }
            }

            if (e.type == SDL.SDL_EventType.SDL_MOUSEWHEEL & !ImGui.GetIO().WantCaptureMouse)
            {
                _state.onFocusMoved();
                if (e.wheel.y > 0)
                {
                    //_state.Camera.ZoomIn(mouseX, mouseY);
                }
                else if (e.wheel.y < 0)
                {
                    //_state.Camera.ZoomOut(mouseX, mouseY);
                }
            }

            return true;
        }



        public override void ImGuiRender()
        {


            GL.ClearColor(backColor.X, backColor.Y, backColor.Z, 1f);
            GL.Clear(GL.Enum.GL_COLOR_BUFFER_BIT);

            //_uiState.MapRendering.Draw();

            // Render ImGui on top of the rest. this eventualy calls overide void ImGuiLayout();
            base.ImGuiRender();


        }

        public unsafe override void ImGuiLayout()
        {
            if (_state.ShowImgDbg)
            {

                ImGui.NewLine();
                SDL.SDL_RendererInfo renderInfo;
                SDL.SDL_GetRendererInfo(_state.rendererPtr, out renderInfo);
                ImGui.Text("SDL RenderInfo:");
                ImGui.Text("Name : " + renderInfo.name.ToString());
                ImGui.Text("Flags: " + renderInfo.flags.ToString());
                ImGui.Text("MaxTexH: " + renderInfo.max_texture_height.ToString());
                ImGui.Text("MaxTexW: " + renderInfo.max_texture_width.ToString());
                ImGui.Text("NumTxtFormats: " + renderInfo.num_texture_formats.ToString());
                //ImGui.Text("Flags: " +renderInfo.texture_formats.ToString());


                SDL.SDL_GetRenderDriverInfo(0, out renderInfo);
                ImGui.Text("SDL RenderDriverInfo:");
                ImGui.Text("Name : " + renderInfo.name.ToString());
                ImGui.Text("Flags: " + renderInfo.flags.ToString());
                ImGui.Text("MaxTexH: " + renderInfo.max_texture_height.ToString());
                ImGui.Text("MaxTexW: " + renderInfo.max_texture_width.ToString());
                ImGui.Text("NumTxtFormats: " + renderInfo.num_texture_formats.ToString());
                ImGui.NewLine();

                foreach (var kvp in _state.SDLImageDictionary)
                {
                    int h, w, a;
                    uint f;
                    int q = SDL.SDL_QueryTexture(kvp.Value, out f, out a, out w, out h);
                    if (q != 0)
                    {
                        ImGui.Text("QueryResult: " + q);
                        ImGui.Text(SDL.SDL_GetError());
                    }

                    ImGui.Image(kvp.Value, new System.Numerics.Vector2(w, h));
                }
            }

            if (_state.ShowMetrixWindow)
                ImGui.ShowMetricsWindow(ref _state.ShowMetrixWindow);
            if (_state.ShowDemoWindow)
            {
                ImGui.ShowDemoWindow();
                ImGui.ShowUserGuide();
            }


            /*
            foreach (var item in _state.LoadedWindows.Values.ToArray())
            {
                item.Display();
            }

            foreach (var item in _state.LoadedNonUniqueWindows.Values.ToArray())
            {
                item.Display();
            }*/




            var windowPtr = this.Handle;
            var rendererPtr = SDL.SDL_GetRenderer(windowPtr);
            Draw(rendererPtr);

            //var dispsize = ImGui.GetIO().DisplaySize;
            //var pos = new System.Numerics.Vector2(0, dispsize.Y - ImGui.GetFrameHeightWithSpacing());
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0,0), ImGuiCond.Always);
            var flags = ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration |
                        ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings |
                        ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav;
            UIControls();

        }

        private float _tiltAngle = 0;
        private float _eccentricy = 0.15f;
        private float _startAngle = 0;
        private float _endAngle = 1.57079632679f;
        public void UIControls()
        {
            if (ImGui.Begin("Ctrl"))
            {
                ImGui.SliderAngle("Angle", ref _tiltAngle);
                ImGui.SliderFloat("eccentricity", ref _eccentricy, 0, 1);
                ImGui.SliderAngle("StartAngle", ref _startAngle, -360, 360);
                ImGui.SliderAngle("Sweep", ref _endAngle, -360, 360);
            }
        }

    public void Draw(IntPtr rendererPtr)
        {
            byte red = 255;
            byte grn = 0;
            byte blu = 255;
            byte alph = 255;
            
            SDL.SDL_SetRenderDrawColor(rendererPtr, red, grn, blu, alph);
            //SDL.SDL_RenderDrawLine(rendererPtr, 0, 0, 256, 256);
            //var tpls = Stuff.EllipseArrayFromPaper(100, 200, _tiltAngle, 64 );
            //var tpls = Stuff.EllipsArrayPaper1(200, _eccentricy, _tiltAngle, _startAngle, _endAngle ,64 );
            var tpls = Stuff.EllipseFullMtxSweep(200, _eccentricy, _tiltAngle, _startAngle, _endAngle, 64);
            var cx = (int) _state.MainWinSize.X / 2;
            var cy = (int) _state.MainWinSize.Y / 2;
            foreach (var tpl in tpls)
            {
                SDL.SDL_RenderDrawPoint(rendererPtr, (int)(cx + tpl.X), (int)(cy + tpl.Y));
            }
        }
    }

    public enum MouseButtons
    {
        Primary,
        Alt,
        Middle
    }
    
}