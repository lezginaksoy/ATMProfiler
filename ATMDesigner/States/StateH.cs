﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.Collections;
using ATMDesigner.Common;

namespace ATMDesigner.UI.States
{
    /// <summary>
    /// Customer class to be displayed in the property grid
    /// </summary>
    /// 
 
    public class StateH : StateBase
    {
        public StateH(ViewModelDesignerCanvas Canvas)
            : base(Canvas)
        {
            SetDafultData();
        }

       public StateH()
       {
           SetDafultData();
       }
     

       #region Converters

        public class DisplayIDConverter : Xceed.Wpf.Toolkit.PropertyGrid.Attributes.IItemsSource
        {
            public Xceed.Wpf.Toolkit.PropertyGrid.Attributes.ItemCollection GetValues()
            {
                Xceed.Wpf.Toolkit.PropertyGrid.Attributes.ItemCollection strings = new Xceed.Wpf.Toolkit.PropertyGrid.Attributes.ItemCollection();
                strings.Add(0,"Display X save to buffer C");               
                strings.Add(1,"Display normal save to buffer C");               
                strings.Add(2,"Display X save to buffer B");               
                strings.Add(3,"Display normal save to buffer B");              
              return strings;
            }
        }

        public class NumberOfZeroesConverter : Xceed.Wpf.Toolkit.PropertyGrid.Attributes.IItemsSource
        {
            public Xceed.Wpf.Toolkit.PropertyGrid.Attributes.ItemCollection GetValues()
            {
                Xceed.Wpf.Toolkit.PropertyGrid.Attributes.ItemCollection strings = new Xceed.Wpf.Toolkit.PropertyGrid.Attributes.ItemCollection();
                strings.Add(00,"00 Numeric");
                for (int i = 0; i < 32; i++)
                {
                    strings.Add((i + 1).ToString().PadLeft(2, '0'), (i + 1).ToString().PadLeft(2, '0') + " Alfanumeric");
                }
                
                return strings;
            }
        }
               
        #endregion
        
       #region State Parameters

        private string _screen;
        private string _timeoutNextState;
        private string _cancelNextState;
        private string _fdkANextState;
        private string _fdkBNextState;
        private string _fdkCNextState;
        private string _fdkDNextState;
        private string _displayID;
        private string _keyPad;
        private string _EntryModeBufferDisplay;
     
        [CategoryAttribute("State Parameters"), PropertyOrder(1), DescriptionAttribute("State Screen Number")]
        [Editor(typeof(AssignmentScreenButton), typeof(AssignmentScreenButton))]
        public string ScreenNumber
        {
            get
            {
                return _screen;
            }

            set
            {
                _screen = value.PadLeft(3,'0');
            }
        }

        [CategoryAttribute("State Parameters"), PropertyOrder(2), ReadOnly(true), DescriptionAttribute("Timeout Next State")]
        public string TimeoutNextState
        {
            get
            {
                return _timeoutNextState;
            }

            set
            {
                _timeoutNextState = value.PadLeft(3, '0');
            }
        }

        [CategoryAttribute("State Parameters"), PropertyOrder(3), ReadOnly(true), DescriptionAttribute("Cancel Next State")]
        public string CancelNextState
        {
            get
            {
                return _cancelNextState;
            }

            set
            {
                _cancelNextState = value.PadLeft(3, '0'); 
            }
        }
    
        [CategoryAttribute("State Parameters"), PropertyOrder(4), ReadOnly(true), DescriptionAttribute("FDK A Next State")]
        public string FDK_A_NextState
        {
            get
            {
                return _fdkANextState;
            }

            set
            {
                _fdkANextState = value.PadLeft(3, '0'); 
            }
        }

        [CategoryAttribute("State Parameters"), PropertyOrder(5), ReadOnly(true), DescriptionAttribute("FDK B Next State")]
        public string FDK_B_NextState
        {
            get
            {
                return _fdkBNextState;
            }

            set
            {
                _fdkBNextState = value.PadLeft(3, '0'); 
            }
        }

        [CategoryAttribute("State Parameters"), PropertyOrder(6), ReadOnly(true), DescriptionAttribute("FDK C Next State")]
        public string FDK_C_NextState
        {
            get
            {
                return _fdkCNextState;
            }

            set
            {
                _fdkCNextState = value.PadLeft(3, '0');
            }
        }

        [CategoryAttribute("State Parameters"), PropertyOrder(7), ReadOnly(true), DescriptionAttribute("FDK D Next State")]
        public string FDK_D_NextState
        {
            get
            {
                return _fdkDNextState;
            }

            set
            {
                _fdkDNextState = value.PadLeft(3, '0'); 
            }
        }
           
        [Category("State Parameters"), PropertyOrder(8), DescriptionAttribute("Display")]
        [Xceed.Wpf.Toolkit.PropertyGrid.Attributes.ItemsSource(typeof(DisplayIDConverter))]       
        public string DisplayID
        {
            get 
            { 
                return _displayID;
            }
            set
            {
                _displayID = value; 
            }

        }
              
        [Category("State Parameters"), PropertyOrder(9), DescriptionAttribute("Entry Mode")]
        [Xceed.Wpf.Toolkit.PropertyGrid.Attributes.ItemsSource(typeof(NumberOfZeroesConverter))]  
        public string Keypad
        {
            get 
            { 
                return _keyPad;
            }
            set 
            {
                _keyPad = value; 
            }
        }

              
        [Browsable(false)]
        public string EntryModeBufferDisplay
        {
            get 
            { 
                return _EntryModeBufferDisplay=_keyPad+_displayID;
            }
            set 
            {
                _EntryModeBufferDisplay = value;
                //_keyPad = value.Substring(0, 2);
                //_displayID = value.Substring(2, 1);
            }
        }


        #endregion
       
       #region Events and Methods
        
        public override Object StateChanged(string SelectedProperty, string newValue, Object ClassInstance, PropertyGrid SelectedPgrid)
        {
            //FillStateFromPropertyGrid(ClassInstance, SelectedPgrid);
            StateH Selectedstate = new StateH();
            StateH Dynamicstate = new StateH();

            Selectedstate = (StateH)SelectedPgrid.SelectedObject;
            Dynamicstate = (StateH)ClassInstance;

            Dynamicstate._Description = Selectedstate.StateDescription;
            Dynamicstate.BrandId = Selectedstate.BrandId;
            Dynamicstate.ConfigId = Selectedstate.ConfigId;
          
            Dynamicstate._cancelNextState = Selectedstate._cancelNextState;
            Dynamicstate._fdkANextState = Selectedstate._fdkANextState;
            Dynamicstate._fdkBNextState = Selectedstate._fdkBNextState;
            Dynamicstate._fdkCNextState = Selectedstate._fdkCNextState;
            Dynamicstate._fdkDNextState = Selectedstate._fdkDNextState;
            Dynamicstate._screen = Selectedstate._screen;
            Dynamicstate._timeoutNextState = Selectedstate._timeoutNextState;
            Dynamicstate._keyPad = Selectedstate._keyPad;
            Dynamicstate._displayID = Selectedstate._displayID;
            Dynamicstate._EntryModeBufferDisplay = Selectedstate._EntryModeBufferDisplay;
            return Dynamicstate;
        }

       public override Object FillPropertyGridFromState(Object ClassInstance, PropertyGrid SelectedPgrid)
        {
            StateH Selectedstate = new StateH();
            StateH Dynamicstate = new StateH();

            Selectedstate = (StateH)SelectedPgrid.SelectedObject;
            Dynamicstate = (StateH)ClassInstance;
            Dynamicstate.BrandId = Selectedstate.BrandId;
            Dynamicstate.ConfigId = Selectedstate.ConfigId;
            Dynamicstate._Description= Selectedstate.StateDescription;
          
            //Dynamicstate._cancelnextState = Selectedstate._cancelnextState;
            //Dynamicstate._fdkANextState = Selectedstate._fdkANextState;
            //Dynamicstate._fdkBNextState = Selectedstate._fdkBNextState;
            //Dynamicstate._fdkCNextState = Selectedstate._fdkCNextState;
            //Dynamicstate._fdkDNextState = Selectedstate._fdkDNextState;
            Dynamicstate._screen = Selectedstate._screen;
            //Dynamicstate._timeoutnextState = Selectedstate._timeoutnextState;
            Dynamicstate._keyPad = Selectedstate._keyPad;
            Dynamicstate._displayID = Selectedstate._displayID;
            Dynamicstate._EntryModeBufferDisplay = Selectedstate._EntryModeBufferDisplay;
            return Dynamicstate;
        }

       public override object CreateInsertCommandScript(PropertyGrid SelectedPgrid, string ProjectName, string TransactionName, int ExtensionStateNumber)
       {
           StateH State = new StateH();
           State = (StateH)SelectedPgrid.SelectedObject;
           List<string> SqlStringList = new List<string>();

           string sql = SqlStr;

           sql = string.Format(sql, Guid, Status, DateTime.Now.ToString("yyyyMMddHHmmss"), State.StateNumber, State.StateDescription,
               State.StateType, ProjectName, TransactionName, State.ScreenNumber, State.TimeoutNextState, State.CancelNextState,
               State.FDK_A_NextState, State.FDK_B_NextState, State.FDK_C_NextState, State.FDK_D_NextState, State.EntryModeBufferDisplay,
               State.ConfigId,State.BrandId, State.ConfigVersion);
           SqlStringList.Add(sql);

           return SqlStringList;
       }

       private void SetDafultData()
       {
           StateType = "H";
           StateDescription = "Information Entry State";
           _screen = "000";
           _timeoutNextState = "255";
           _cancelNextState = "255";
           _fdkANextState = "255";
           _fdkBNextState = "255";
           _fdkCNextState = "255";
           _fdkDNextState = "255";           
           _displayID = "0";
           _keyPad = "00";
           _EntryModeBufferDisplay = "000";
       }



       public override object FillStatesFromDB(object[] processRow, ArrayList StateList)
       {
           StateH state = new StateH();
           ModelCanvasStateObject TransStateObj = new ModelCanvasStateObject();
           List<ModelParentStateObject> ParentobjList = new List<ModelParentStateObject>();
           List<ModelChildStateObject> ChildobjList = new List<ModelChildStateObject>();
            
           state.Status = processRow[1].ToString();
           state.StateNumber = processRow[3].ToString();
           state.StateDescription = processRow[4].ToString();
           state.StateType = processRow[5].ToString();

           state._screen = processRow[8].ToString();
           state._timeoutNextState = processRow[9].ToString();
           state._cancelNextState = processRow[10].ToString();
           state._fdkANextState = processRow[11].ToString();
           state._fdkBNextState = processRow[12].ToString();
           state._fdkCNextState = processRow[13].ToString();
           state._fdkDNextState = processRow[14].ToString();
           state._EntryModeBufferDisplay = processRow[15].ToString();
           state._keyPad = state.EntryModeBufferDisplay.Substring(0, 2);
           state._displayID = state.EntryModeBufferDisplay.Substring(2, 1);

           state.ConfigId = processRow[16].ToString();
           state.BrandId = processRow[17].ToString();
           state.ConfigVersion = processRow[18].ToString();

           //NextState Kontrolu
           #region nextState arrange-parent and pointer

           if (state.TimeoutNextState != "255")
               ChildobjList.Add(GetChildState("TimeoutNextState", state.TimeoutNextState, StateList, processRow[7].ToString(), state.StateType, state.StateNumber));

           if (state.CancelNextState != "255")
               ChildobjList.Add(GetChildState("CancelNextState", state.CancelNextState, StateList, processRow[7].ToString(), state.StateType, state.StateNumber));

           if (state.FDK_A_NextState != "255")
               ChildobjList.Add(GetChildState("FDK_A_NextState", state.FDK_A_NextState, StateList, processRow[7].ToString(), state.StateType, state.StateNumber));

           if (state.FDK_B_NextState != "255")
               ChildobjList.Add(GetChildState("FDK_B_NextState", state.FDK_B_NextState, StateList, processRow[7].ToString(), state.StateType, state.StateNumber));

           if (state.FDK_C_NextState != "255")
               ChildobjList.Add(GetChildState("FDK_C_NextState", state.FDK_C_NextState, StateList, processRow[7].ToString(), state.StateType, state.StateNumber));

           if (state.FDK_D_NextState != "255")
               ChildobjList.Add(GetChildState("FDK_D_NextState", state.FDK_D_NextState, StateList, processRow[7].ToString(), state.StateType, state.StateNumber));

           #endregion

           TransStateObj.BrandId = state.BrandId;
           TransStateObj.ConfigId = state.ConfigId;
           TransStateObj.Id = state.StateNumber;
           TransStateObj.StateDescription = state.StateDescription;
           TransStateObj.Type = state.StateType;
           TransStateObj.TransactionName = processRow[7].ToString();

           TransStateObj.PropertyGrid.SelectedObject = state;
           TransStateObj.ParentStateList = ParentobjList;
           TransStateObj.ChildStateList = ChildobjList;
           designerCanvas.TransactionList.Add(TransStateObj);

           return StateList;
       }
    
        #endregion

      
    }
}