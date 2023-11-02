using System.Diagnostics;
using System.Linq.Expressions;
using System.Numerics;
using DrawEllipse;
using DVec;
using ImGuiNET;
using ImGuiSDL2CS;
using SDL2;
using Vector2 = DVec.Vector2;

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
            : base("Draw Ellipse")
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
        private float GetTilt {get {return _tiltAngle * -1;}}
        private float _eccentricy = 0.15f;
        private double _semiMajor = 200;
        private double _semiMinor = 200 * Math.Sqrt(1 - 0.15f * 0.15f);
        private float _startAngle = 0;

        private float _sweepAngle = 1.57079632679f;
        private float _endAng = 1.57079632679f;
        private Vector2 _startPos = new Vector2();
        private Vector2 _endPos = new Vector2();
        private Vector2 _focalPoint = new Vector2(0, 0);
        private Vector2 _ctrPoint = new Vector2();
        private double _linEcc = 0;
        private int _numPoints = 60;
        private long _profilet = 0;

        private SDL.SDL_Point[] _angleLinesFcl = new SDL.SDL_Point[3]
            { new SDL.SDL_Point(), new SDL.SDL_Point(), new SDL.SDL_Point() };
        
        private SDL.SDL_Point[] _angleLinesCtr = new SDL.SDL_Point[3]
            { new SDL.SDL_Point(), new SDL.SDL_Point(), new SDL.SDL_Point() };

        private int _selectFuncIndex = 0;

        private double _sr;
        private double _er;
        
        private string[] _funcNames = new[]
        {
            "From Paper", 
            "Using Matrix",
            "Cheats Circle",
            "ArcRadiusFromFocal using angles",
            "ArcRadiusFromFocal using positons",
            "ArcRadiusFromFocal using ref points",
            "keplerPoints "
        };

        private string _errorMsg = "";
        //private 
        private Stopwatch _stopwatch = new Stopwatch();
        private DVec.Vector2[] _points = new DVec.Vector2[60];
        public void UIControls()
        {
            if (ImGui.Begin("Ctrl"))
            {
                if (ImGui.Combo("Function:", ref _selectFuncIndex, _funcNames, _funcNames.Length))
                {
                    OnChange();
                }

                if (ImGui.SliderAngle("Tilt (LoP)", ref _tiltAngle))
                {
                    OnChange();
                }
                
                if(ImGui.SliderFloat("Eccentricity", ref _eccentricy, 0, 2))
                {
                    if (_eccentricy >= 1)
                        _semiMajor = -200;
                    else
                    {
                        _semiMajor = 200;
                    }
                    _semiMinor = _semiMajor * Math.Sqrt(1 - _eccentricy * _eccentricy);
                    _linEcc = Math.Sqrt((_semiMajor * _semiMajor) - (_semiMinor * _semiMinor));
                    _angleLinesFcl[1] = new SDL.SDL_Point()
                    {
                        //x = (int)(-linEcc * Math.Cos(-_tiltAngle)),
                        //y = (int)(-linEcc * Math.Sin(-_tiltAngle))
                        x = 0,
                        y = 0
                    };
                    OnChange();
                }

                if (ImGui.SliderAngle("StartAngle", ref _startAngle, -360, 360))
                {
                    OnChange();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(_startAngle.ToString("0.####"));
                }
                ImGui.SameLine();
                ImGui.Text(_startPos.X.ToString("0.##") + "," + _startPos.Y.ToString("0.##"));

                if (ImGui.SliderAngle("Sweep", ref _sweepAngle, -360, 360))
                {
                    _endAng = _startAngle + _sweepAngle;
                    OnChange();
                }
                
                if (ImGui.SliderAngle("End", ref _endAng, -360, 360))
                {
                    _sweepAngle = _endAng - _startAngle;
                    OnChange();
                }
                
                ImGui.SameLine();
                ImGui.Text(_endPos.X.ToString("0.##") + "," + _endPos.Y.ToString("0.##"));
                ImGui.Text("a: " + _semiMajor);
                ImGui.Text("sr: " + _sr);
                ImGui.Text("er: " + _er);
                if (ImGui.Button("Profile 100000x"))
                {
                    
                    _stopwatch.Restart();
                    for (int i = 0; i < 100000; i++)
                    {
                        CallFunction(_selectFuncIndex);
                    }
                    _stopwatch.Stop();
                    _profilet = _stopwatch.ElapsedMilliseconds;
                }
                ImGui.SameLine();
                ImGui.Text(_profilet.ToString() + "ms");
                CallFunction(_selectFuncIndex);
                
                ImGui.Text(_errorMsg);

            }
        }

        void OnChange()
        {
            TiltAngle();
            StartAngle();
            SweepAngle();
        }

        private void TiltAngle()
        {
            _ctrPoint.X = -_linEcc * Math.Cos(GetTilt);
            _ctrPoint.Y = -_linEcc * Math.Sin(GetTilt);
            _angleLinesCtr[1] = new SDL.SDL_Point()
            {
                x = (int)_ctrPoint.X,
                y = (int)_ctrPoint.Y,
            };            
        }
        
        private void StartAngle()
        {
            _sr = EllipseFormula.RadiusFromFocal(_semiMajor, _eccentricy, GetTilt, _startAngle);
            var foo = Math.Sin(_startAngle);
            _startPos.X = _sr * Math.Cos(_startAngle);
            _startPos.Y = _sr * Math.Sin(_startAngle);
            var ang = Math.Atan2(_startPos.Y, _startPos.X);
            _angleLinesFcl[0] = new SDL.SDL_Point()
            {
                x = (int)_startPos.X,
                y = (int)_startPos.Y
            };
            
            _angleLinesCtr[0] = _angleLinesFcl[0];


        }

        private void SweepAngle()
        {
            
            _er = EllipseFormula.RadiusFromFocal(_semiMajor, _eccentricy, GetTilt, _endAng);
                    
            _endPos.X = _er * Math.Cos(_endAng);
            _endPos.Y = _er * Math.Sin(_endAng);
            _angleLinesFcl[2] = new SDL.SDL_Point()
            {
                x = (int)_endPos.X,
                y = (int)_endPos.Y
            };
            _angleLinesCtr[2] = _angleLinesFcl[2];
        }
        

        private void CtrPnt()
        {
            
        }

        public void CallFunction(int func)
        {
            try
            {
                switch (func)
                {
                    case 0:
                    {
                        _points = EllipseFormula.EllipseArrayFromPaper(_semiMajor, _semiMinor, GetTilt, _numPoints);
                        break;
                    }
                    case 1:
                    {
                        _points = EllipseFormula.EllipseFullMtxSweep(_semiMajor, _eccentricy, GetTilt, _startAngle,
                            _sweepAngle, 64);
                        break;
                    }
                    case 2:
                    {
                        _points = EllipseFormula.CheatsCircle(_semiMajor, _eccentricy, GetTilt, _startAngle, _sweepAngle, _numPoints);
                        break;
                    }
                    case 3:
                    {
                        _points = EllipseFormula.ArcRadiusFromFocal(_semiMajor, _eccentricy, GetTilt, _startAngle, _sweepAngle, _numPoints);
                    }
                        break;
                    case 4:
                    {
                        _points = EllipseFormula.ArcRadiusFromFocal(_semiMajor, _eccentricy, GetTilt, _startPos, _endPos, _numPoints);
                    }
                        break;
                    case 5:
                    {
                        if (_points.Length != _numPoints)
                            _points = new Vector2[_numPoints];
                        EllipseFormula.ArcRadiusFromFocalRefPoints(_semiMajor, _eccentricy, GetTilt, _startPos, _endPos, ref _points);
                    }
                        break;
                    case 6:
                    {
                        _points = EllipseFormula.KeplerPoints(_semiMajor, _eccentricy, GetTilt, _startPos, _endPos, _numPoints);
                    }
                        break;
                }
                _errorMsg = "";
            }
            catch(Exception e)
            {
                _errorMsg = "This Function doesn't handle the given input:\n" + e.Message;
            }
        }

    public void Draw(IntPtr rendererPtr)
        {
            byte red = 255;
            byte grn = 0;
            byte blu = 255;
            byte alph = 255;
            
            SDL.SDL_SetRenderDrawColor(rendererPtr, red, grn, blu, alph);

            var cx = (int) _state.MainWinSize.X / 2;
            var cy = (int) _state.MainWinSize.Y / 2;
            foreach (var tpl in _points)
            {
                SDL.SDL_RenderDrawPoint(rendererPtr, (int)(cx + tpl.X), (int)(cy - tpl.Y));
            }
            
            
            red = 100;
            grn = 0;
            blu = 0;
            alph = 25;
            SDL.SDL_SetRenderDrawColor(rendererPtr, red, grn, blu, alph);
            var point0 = new SDL.SDL_Point(){
                x = _angleLinesFcl[0].x + cx, 
                y = -_angleLinesFcl[0].y + cy
            };
            for (int i = 1; i < _angleLinesFcl.Length; i++)
            {
                var point1 = new SDL.SDL_Point(){
                    x = _angleLinesFcl[i].x + cx, 
                    y = -_angleLinesFcl[i].y + cy
                };
                SDL.SDL_RenderDrawLine(rendererPtr, point0.x, point0.y, point1.x, point1.y);
                point0 = point1;
            }
            
            
            red = 50;
            grn = 50;
            blu = 0;
            alph = 25;
            SDL.SDL_SetRenderDrawColor(rendererPtr, red, grn, blu, alph);
            point0 = new SDL.SDL_Point(){
                x = _angleLinesCtr[0].x + cx, 
                y = -_angleLinesCtr[0].y + cy
            };
            for (int i = 1; i < _angleLinesCtr.Length; i++)
            {
                var point1 = new SDL.SDL_Point(){
                    x = _angleLinesCtr[i].x + cx, 
                    y = -_angleLinesCtr[i].y + cy
                };
                SDL.SDL_RenderDrawLine(rendererPtr, point0.x, point0.y, point1.x, point1.y);
                point0 = point1;
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