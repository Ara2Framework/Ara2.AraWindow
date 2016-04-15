// Copyright (c) 2010-2016, Rafael Leonel Pontani. All rights reserved.
// For licensing, see LICENSE.md or http://www.araframework.com.br/license
// This file is part of AraFramework project details visit http://www.arafrework.com.br
// AraFramework - Rafael Leonel Pontani, 2016-4-14
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Ara2.Dev;

namespace Ara2.Components
{
    [Serializable]
    [AraDevComponent(vBase:true,vDisplayToolBar:false)]
    public class AraWindow : AraComponentVisualAnchorConteiner,IAraDev
    {
        public AraWindow(IAraObject ConteinerFather) :
            base(AraObjectClienteServer.Create(ConteinerFather, "Div"), ConteinerFather, "AraWindow")
        {
            Unload = new AraComponentEvent<DAraWindowUnload>(this, "Unload");
            //Unload += Window_Unload;

            Click = new AraComponentEvent<EventHandler>(this, "Click");
            DragStop = new AraComponentEvent<Action>(this, "DragStop");
            ResizeStop = new AraComponentEvent<Action>(this, "ResizeStop");
            EventInternal += MyEventInternal;
            SetProperty += Window_SetProperty;

            this.LeftChangeBefore += AraWindow_WHChangeBefore;
            this.TopChangeBefore += AraWindow_WHChangeBefore;
            this.WidthChangeBefore += AraWindow_WHChangeBefore;
            this.HeightChangeBefore += AraWindow_WHChangeBefore;

            Left = null;
            Top = null;

            if (AraTools.DeviceType == EDeviceType.Tablet || AraTools.DeviceType == EDeviceType.Phone)
            {
                this.Maximize = true;
                //foreach (AraWindow TmpJanela in Tick.GetTick().Session.GetObjectsByType(typeof(AraWindow)).Select(a => (AraWindow)a.Object))
                //{
                //    if (TmpJanela.InstanceID != this.InstanceID)
                //        TmpJanela.Minimized = true;
                //}
            }
        }

        void AraWindow_WHChangeBefore(AraDistance ToDistance)
        {
            if (ToDistance!=null)
                if (ToDistance.Unity != AraDistance.EUnity.px)
                    throw ExceptionOnlyPx;
        }

        private void Window_SetProperty(string vName, dynamic vValue)
        {
            switch (vName)
            {
                case "Minimized":
                    {
                        _Minimized = vValue;
                    }
                break;
                case "Maximized":
                    {
                        _Maximize = vValue;
                    }
                break;
                case "ZIndexWindow":
                    {
                        _ZIndexWindow = Convert.ToInt64(vValue);
                    }
                break;
                case "Resizable":
                {
                    _Resizable = vValue;
                }
                break;
            }
        }

        public static void LoadJSStatic()
        {
            Tick vTick = Tick.GetTick();
            vTick.Session.AddJs("Ara2/Components/AraWindow/AraWindows.js");
            vTick.Session.AddJs("Ara2/Components/AraWindow/AraWindow.js");
        }

        public  override void LoadJS()
        {
            AraWindow.LoadJSStatic();
        }

        public delegate void DAraWindowUnload(object vObjReturn);
        [AraDevEvent]
        public AraComponentEvent<DAraWindowUnload> Unload;
        [AraDevEvent]
        public AraComponentEvent<Action> DragStop;
        [AraDevEvent]
        public AraComponentEvent<Action> ResizeStop;
        [AraDevEvent]
        public AraComponentEvent<EventHandler> Click;

        private object ObjReturn = null;

        private void MyEventInternal(String vFunction)
        {
            switch (vFunction)
            {
                case "Unload":
                    if (Unload.InvokeEvent!=null)
                        Unload.InvokeEvent(ObjReturn);
                    this.TickScriptCall();
                    Tick.GetTick().Script.Send(" if (vObj) vObj.DisposeBefore();");
                    this.Dispose();
                    break;
                case "DragStop":
                    DragStop.InvokeEvent();
                    break;
                case "ResizeStop":
                    ResizeStop.InvokeEvent();
                    break;
                case "Click":
                    Click.InvokeEvent(this,new EventArgs());
                break;
            }
        }
        

        private bool PrimeiroShow = true;
        public AraEvent<Action> Active = new AraEvent<Action>();

        //public delegate void DWindowReturn(object vObject);
        //public event DWindowReturn WindowReturn;

        public void Show(bool vModal)
        {
            Show();
            Modal = vModal;
            
        }

        public void Show(bool vModal, DAraWindowUnload vWindowReturn)
        {
            Show();
            Modal = vModal;
            Unload += vWindowReturn;
            
        }

        public void Show(DAraWindowUnload vWindowReturn)
        {
            Show();
            Modal = true;
            Unload += vWindowReturn;
        }

        public void Show()
        {
            if (PrimeiroShow)
            {
                if (Active.InvokeEvent!=null)
                    Active.InvokeEvent();
                PrimeiroShow = true;
            }

            this.TickScriptCall();
            Tick.GetTick().Script.Send(" vObj.Show();\n");
            this.Visible = true;
        }

        private bool _Modal = false;
        [AraDevProperty(false)]
        public bool Modal
        {
            get { return _Modal; }
            set
            {
                _Modal = value;
                this.TickScriptCall();
                Tick.GetTick().Script.Send(" vObj.SetModal(" + (_Modal?"true":"false") + ");\n");
            }
        }

        private string _Title = "";
        [AraDevProperty("")]
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                this.TickScriptCall();
                Tick.GetTick().Script.Send(" vObj.SetTitle('" + AraTools.StringToStringJS(Title) + "');\n");
            }
        }

        private bool _Minimized = false;
        [AraDevProperty(false)]
        public bool Minimized
        {
            get{ return _Minimized;}
            set
            {
                _Minimized = value;
                this.TickScriptCall();
                Tick.GetTick().Script.Send(" vObj.SetMinimized(" + (_Minimized ? "true" : "false") + ");\n");
            }
        }

        public bool _Maximize = false;
        [AraDevProperty(false)]
        public bool Maximize
        {
            get { return _Maximize; }
            set
            {
                _Maximize = value;
                this.TickScriptCall();
                Tick.GetTick().Script.Send(" vObj.SetMaximize(" + (_Maximize ? "true" : "false") + ");\n");
            }
        }

        private Int64 _ZIndexWindow = 0;
        [AraDevProperty(0)]
        public Int64 ZIndexWindow
        {
            get { return _ZIndexWindow; }
            set
            {
                _ZIndexWindow = value;
                this.TickScriptCall();
                Tick.GetTick().Script.Send(" vObj.SetZIndex(" + _ZIndexWindow + ");\n");
            }
        }

        private bool _ButtonMinimize = true;
        [AraDevProperty(true)]
        public bool ButtonMinimize
        {
            get { return _ButtonMinimize; }
            set
            {
                _ButtonMinimize = value;
                this.TickScriptCall();
                Tick.GetTick().Script.Send(" vObj.SetButtonMnimize(" + (_ButtonMinimize ? "true" : "false") + ");\n");
            }
        }

        private bool _ButtonMaximize = true;
        [AraDevProperty(true)]
        public bool ButtonMaximize
        {
            get { return _ButtonMaximize; }
            set
            {
                _ButtonMaximize = value;
                this.TickScriptCall();
                Tick.GetTick().Script.Send(" vObj.SetVisibleMaximizeButton(" + (_ButtonMaximize ? "true" : "false") + ");\n");
            }
        }

        private bool _ButtonClose = true;
        [AraDevProperty(true)]
        public bool ButtonClose
        {
            get { return _ButtonClose; }
            set
            {
                _ButtonClose = value;
                this.TickScriptCall();
                Tick.GetTick().Script.Send(" vObj.SetButtonClose(" + (_ButtonClose ? "true" : "false") + ");\n");
            }
        }

        private bool _Resizable = true;

        [AraDevProperty(true)]
        public bool Resizable
        {
            get { return _Resizable; }
            set
            {
                _Resizable = value;
                this.TickScriptCall();
                Tick.GetTick().Script.Send(" vObj.SetResizable(" + (_Resizable ? "true" : "false") + ");\n");

                if (_Resizable == false)
                    ButtonMaximize = false;
            }
        }

        public void Close()
        {
            MyEventInternal("Unload");
        }

        public void Close(object vObjReturn)
        {
            ObjReturn = vObjReturn;
            Close();
        }

        [AraDevProperty(-1)]
        public int ZIndex
        {
            get { return -1; }
            set { }
        }

        #region Ara2Dev
        private string _Name = "";
        [AraDevProperty]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private AraEvent<DStartEditPropertys> _StartEditPropertys = null;
        public AraEvent<DStartEditPropertys> StartEditPropertys
        {
            get
            {
                if (_StartEditPropertys == null)
                {
                    _StartEditPropertys = new AraEvent<DStartEditPropertys>();
                    this.Click += this_ClickEdit;
                }

                return _StartEditPropertys;
            }
            set
            {
                _StartEditPropertys = value;
            }
        }

        private void this_ClickEdit(object sender, EventArgs e)
        {
            if (_StartEditPropertys.InvokeEvent != null)
                _StartEditPropertys.InvokeEvent(this);
        }

        private AraEvent<DStartEditPropertys> _ChangeProperty = new AraEvent<DStartEditPropertys>();
        public AraEvent<DStartEditPropertys> ChangeProperty
        {
            get
            {
                return _ChangeProperty;
            }
            set
            {
                _ChangeProperty = value;
            }
        }

        private System.Collections.Hashtable AraDevEvents = new System.Collections.Hashtable();

        #endregion

        #region Statics


        public static void SetWindowsBorderLeft(int vValue)
        {
            Tick.GetTick().Script.Send(" Ara.AraWindows.SetBorderLeft(" + vValue + ");\n");
        }

        public static void SetWindowsBorderTop(int vValue)
        {
            Tick.GetTick().Script.Send(" Ara.AraWindows.SetBorderTop(" + vValue + ");\n");
        }

        public static void SetWindowsBorderRight(int vValue)
        {
            Tick.GetTick().Script.Send(" Ara.AraWindows.SetBorderRight(" + vValue + ");\n");
        }

        public static void SetWindowsBorderBottom(int vValue)
        {
            Tick.GetTick().Script.Send(" Ara.AraWindows.SetBorderBottom(" + vValue + ");\n");
        }

        public enum EBasedSizeByObject
        {
            window,
            document
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vObject">default: widnow</param>
        public static void SetBasedSizeByObject(EBasedSizeByObject vObject)
        {
            Tick.GetTick().Script.Send(" Ara.AraWindows.SetBasedSizeByObject(" + (vObject== EBasedSizeByObject.window?"window":"document") + ");\n");
        }

        #endregion

    }
}
