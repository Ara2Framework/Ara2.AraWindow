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
using Ara2.Components;

namespace Ara2.Components
{
    [Serializable]
    [AraDevComponent(vConteiner:false,vDisplayToolBar:false,vResizable:false,vDraggable:false)]
    public class AraWindowTaskbar : AraComponentVisualAnchorConteiner , ITaskbar
    {
        public AraWindowTaskbar(IAraContainerClient ConteinerFather)
            : base(AraObjectClienteServer.Create(ConteinerFather, "Div"), ConteinerFather, "AraWindowTaskbar")
        {
            Click = new AraComponentEvent<EventHandler>(this, "Click");
            this.EventInternal += AraDiv_EventInternal;
            this.Height = 27;
            this.Anchor.Left = 5;
            this.Anchor.Right = 5;
            this.Anchor.Bottom = 5;
        }

        public override void LoadJS()
        {
            AraWindow.LoadJSStatic();

            Tick vTick = Tick.GetTick();
            vTick.Session.AddJs("Ara2/Components/AraWindowTaskbar/AraWindowTaskbar.js");
        }

        public void AraDiv_EventInternal(String vFunction)
        {
            switch (vFunction.ToUpper())
            {
                case "CLICK":
                    Click.InvokeEvent(this, new EventArgs());
                    break;
            }
        }

        
        #region Eventos
        public AraComponentEvent<EventHandler> Click;        
        #endregion

        public void AddClass(String vNameClass)
        {
            Tick vTick = Tick.GetTick();
            this.TickScriptCall();
            vTick.Script.Send(" vObj.AddClass('" + AraTools.StringToStringJS(vNameClass) + "'); \n");
        }

        public void DelClass(String vNameClass)
        {
            Tick vTick = Tick.GetTick();
            this.TickScriptCall();
            vTick.Script.Send(" vObj.DelClass('" + AraTools.StringToStringJS(vNameClass) + "'); \n");
        }


        
    }
}
