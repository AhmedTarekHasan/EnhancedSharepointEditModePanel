using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Publishing.WebControls;
using System.Web.UI.WebControls;
using System.Web.UI;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.WebControls;
using System.Security.Permissions;
using System.ComponentModel;
using Microsoft.SharePoint;
using System.Web;

namespace DevelopmentSimplyPut.Sharepoint.WebControls
{
    [CLSCompliant(false)]
    public class ModePanel : Panel, INamingContainer, IParserAccessor
    {
        #region Fields
        private bool shouldRender;
        #endregion Fields

        #region Properties
        [Category("Appearance"), DefaultValue(1)]
        public PageDisplayMode PageDisplayMode
        {
            get
            {
                object obj2 = this.ViewState["PageDisplayMode"];
                if (obj2 != null)
                {
                    return (PageDisplayMode)obj2;
                }
                return PageDisplayMode.Edit;
            }
            set
            {
                this.ViewState["PageDisplayMode"] = value;
            }
        }
        [Category("Appearance"), DefaultValue(false)]
        public bool SuppressTag
        {
            get
            {
                object obj2 = this.ViewState["RenderTag"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["RenderTag"] = value;
            }
        }
        #endregion Properties

        #region Contructors
        public ModePanel()
        {
        }
        #endregion Constructors    

        #region Methods
        [SharePointPermission(SecurityAction.Demand, ObjectModel = true)]
        protected override void AddParsedSubObject(object obj)
        {
            this.CalculateShouldRender();
            if (this.shouldRender)
            {
                base.AddParsedSubObject(obj);
            }
        }
        public virtual SPControlMode GetContextualFormModeFromPostedForm()
        {
            SPControlMode invalid = SPControlMode.Invalid;
            bool IsDocLibListItem = ((SPContext.Current.ListItem != null) && (SPContext.Current.ItemId != 0));

            if (HttpContext.Current != null)
            {
                HttpRequest request = HttpContext.Current.Request;
                if (IsDocLibListItem)
                {
                    invalid = (request.Form.Get("MSOAuthoringConsole_FormContext") == "1") ? SPControlMode.Edit : SPControlMode.Display;
                }
                if ((invalid == SPControlMode.Display) && (request.QueryString.Get("ControlMode") == "Edit"))
                {
                    invalid = SPControlMode.Edit;
                }
            }
            return invalid;
        }
        protected virtual void CalculateShouldRender()
        {
            SPControlMode contextualFormModeFromPostedForm = GetContextualFormModeFromPostedForm();
            //SPControlMode contextualFormModeFromPostedForm = SPContext.Current.FormContext.FormMode;
            if ((SPControlMode.Display == contextualFormModeFromPostedForm) && (PageDisplayMode.Display == this.PageDisplayMode))
            {
                this.shouldRender = true;
            }
            else if ((SPControlMode.Edit == contextualFormModeFromPostedForm) && (PageDisplayMode.Edit == this.PageDisplayMode))
            {
                this.shouldRender = true;
            }
            else
            {
                this.shouldRender = false;
            }

            this.Visible = this.shouldRender;
        }
        [SharePointPermission(SecurityAction.Demand, ObjectModel = true)]
        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            if (!this.SuppressTag)
            {
                base.RenderBeginTag(writer);
            }
        }
        [SharePointPermission(SecurityAction.Demand, ObjectModel = true)]
        public override void RenderEndTag(HtmlTextWriter writer)
        {
            if (!this.SuppressTag)
            {
                base.RenderEndTag(writer);
            }
        }
        #endregion Methods
    }
}
