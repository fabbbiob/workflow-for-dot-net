﻿using System;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using ClientDependency.Core;
using log4net;
using Moriyama.Workflow.Interfaces.Application;
using Moriyama.Workflow.Interfaces.Application.Event;
using Moriyama.Workflow.Interfaces.Domain;
using Moriyama.Workflow.Interfaces.Ui;
using Moriyama.Workflow.Umbraco6.Web.Extensions;
using umbraco.BasePages;


[assembly: WebResource("Moriyama.Workflow.Umbraco6.Web.Workflow.Js.Util.js", "text/javascript")]
[assembly: WebResource("Moriyama.Workflow.Umbraco6.Web.Workflow.Js.Autocomplete.js", "text/javascript")]
[assembly: WebResource("Moriyama.Workflow.Umbraco6.Web.Workflow.Css.Controls.css", "text/css")]
namespace Moriyama.Workflow.Umbraco6.Web.Workflow
{
    public partial class EditInstantiationCriteria : UmbracoEnsuredPage
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IGlobalisationService TheGlobalisationService { get; set; }

        public IWorkflowInstantiationCriteriaService TheWorkflowInstantiationCriteriaService { get; set; }
        public IUiResolver TheWorkflowEntityUiResolver { get; set; }
        public IEventService TheEventService { get; set; }

        private IWorkflowInstantiationCriteria _instantiationCriteria;
        private IWorkflowEntityUi _entityUi;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.AddResourceToClientDependency("Moriyama.Workflow.Umbraco6.Web.Workflow.Js.Util.js", ClientDependencyType.Javascript);
            this.AddResourceToClientDependency("Moriyama.Workflow.Umbraco6.Web.Workflow.Js.Autocomplete.js", ClientDependencyType.Javascript);
            this.AddResourceToClientDependency("Moriyama.Workflow.Umbraco6.Web.Workflow.Css.Controls.css", ClientDependencyType.Css);


        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SaveCriteriaButton.Text = TheGlobalisationService.GetString("save_criteria");

            var criteriaId = Convert.ToInt32(Request["id"]);
            _instantiationCriteria = TheWorkflowInstantiationCriteriaService.GetCriteria(criteriaId);

            Log.Debug(string.Format("Got criteria of type {0}", _instantiationCriteria.GetType()));

            _entityUi = TheWorkflowEntityUiResolver.Resolve(_instantiationCriteria);

            // if (IsPostBack) return;

            foreach (var control in _entityUi.Render(_instantiationCriteria))
            {
                CiteriaControlsPanel.Controls.Add(control);
            }
        }

        protected void SaveCriteriaButtonClick(object sender, EventArgs e)
        {
            var values = _entityUi.UiProperties.ToDictionary(uiProperty => uiProperty.PropertyName, uiProperty => uiProperty.Value);

            TheWorkflowInstantiationCriteriaService.SetConfigurationProperties(_instantiationCriteria.Id, values);
            TheEventService.RegisterEvents();

            SavedLiteral.Visible = true;

        }
    }
}