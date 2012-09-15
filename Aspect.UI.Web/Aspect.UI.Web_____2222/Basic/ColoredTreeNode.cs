using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Aspect.UI.Web.Basic
{
    public class ColoredTreeNode : TreeNode
    {
        public ColoredTreeNode(string text, string value)
            : base(text, value)
        {
            //this.TrackViewState();   
        }
        private string _Color;
        public string Color
        {
            get
            {
                return this._Color;
            }
            set
            {
                this._Color = value;
            }
        }

        /*protected override void LoadViewState(object state)
        {
            if (state != null)
            {
                object[] myState = (object[])state;
                if (myState[0] != null)
                    base.LoadViewState(myState[0]);
                if (myState[1] != null)
                    Color = (String)myState[1];
            }
        }

        protected override object SaveViewState()
        {
            object baseState = base.SaveViewState();
            object[] allStates = new object[2];
            allStates[0] = baseState;
            allStates[1] = Color;
            return allStates;
        }*/
        
        protected override void RenderPreText(System.Web.UI.HtmlTextWriter writer)
        {
            writer.WriteBeginTag("span");
            writer.WriteAttribute("style", String.Concat("background-color: ", this._Color));

            writer.Write(System.Web.UI.HtmlTextWriter.TagRightChar);
        }

        protected override void RenderPostText(System.Web.UI.HtmlTextWriter writer)
        {
            writer.WriteEndTag("span");
        }
    }
}
