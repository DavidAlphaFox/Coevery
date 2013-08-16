﻿using System;
using System.Linq;
using Coevery.Core.Services;
using Coevery.Entities.Settings;
using Coevery.Entities.ViewModels;
using Coevery.FormDesigner.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Utility.Extensions;

namespace Coevery.Entities.Services {
    public class FieldService : Component, IFieldService {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly ISchemaUpdateService _schemaUpdateService;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public FieldService(
            IContentDefinitionService contentDefinitionService,
            ISchemaUpdateService schemaUpdateService,
            IContentDefinitionManager contentDefinitionManager) {
            _contentDefinitionService = contentDefinitionService;
            _schemaUpdateService = schemaUpdateService;
            _contentDefinitionManager = contentDefinitionManager;
        }

        public void CreateCheck(string entityName, AddFieldViewModel viewModel, IUpdateModel updateModel) {
            if (String.IsNullOrWhiteSpace(viewModel.DisplayName)) {
                updateModel.AddModelError("DisplayName", T("The Display Name name can't be empty."));
            }

            if (String.IsNullOrWhiteSpace(viewModel.Name)) {
                updateModel.AddModelError("Name", T("The Technical Name can't be empty."));
            }

            if (viewModel.Name.ToLower() == "id") {
                updateModel.AddModelError("Name", T("The Field Name can't be any case of 'Id'."));
            }

            if (_contentDefinitionService.GetPart(entityName).Fields.Any(t => String.Equals(t.Name.Trim(), viewModel.Name.Trim(), StringComparison.OrdinalIgnoreCase))) {
                updateModel.AddModelError("Name", T("A field with the same name already exists."));
            }

            if (!String.IsNullOrWhiteSpace(viewModel.Name) && !viewModel.Name[0].IsLetter()) {
                updateModel.AddModelError("Name", T("The technical name must start with a letter."));
            }

            if (!String.Equals(viewModel.Name, viewModel.Name.ToSafeName(), StringComparison.OrdinalIgnoreCase)) {
                updateModel.AddModelError("Name", T("The technical name contains invalid characters."));
            }

            if (_contentDefinitionService.GetPart(entityName).Fields.Any(t => String.Equals(t.DisplayName.Trim(), Convert.ToString(viewModel.DisplayName).Trim(), StringComparison.OrdinalIgnoreCase))) {
                updateModel.AddModelError("DisplayName", T("A field with the same Display Name already exists."));
            }

            var prefix = viewModel.FieldTypeName + "Settings";
            var clientSettings = new FieldSettings();
            updateModel.TryUpdateModel(clientSettings, prefix, null, null);
            if (clientSettings.IsSystemField) {
                updateModel.AddModelError("IsSystemField", T("Can't modify the IsSystemField field."));
            }

            _contentDefinitionService.CreateFieldCheck(entityName, viewModel.Name, viewModel.FieldTypeName, updateModel);
        }

        public void Create(string entityName, AddFieldViewModel viewModel, IUpdateModel updateModel) {
            try {
                _contentDefinitionService.AddFieldToPart(viewModel.Name, viewModel.DisplayName, viewModel.FieldTypeName, entityName);
                _contentDefinitionService.CreateField(entityName, viewModel.Name, updateModel);
                _schemaUpdateService.CreateColumn(entityName, viewModel.Name, viewModel.FieldTypeName);
            }
            catch (Exception ex) {
                updateModel.AddModelError("ErrorInfo", T("Add field failed."+ex.Message));
            }
        }

        public void Delete(string name, string parentname) {
            _contentDefinitionService.RemoveFieldFromPart(name, parentname);
            var layoutManager = new LayoutManager(_contentDefinitionManager);
            layoutManager.DeleteField(parentname, name);
            _schemaUpdateService.DropColumn(parentname, name);
        }
    }
}