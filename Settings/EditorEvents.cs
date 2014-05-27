//using System.Collections.Generic;
//using Orchard.ContentManagement;
//using Orchard.ContentManagement.MetaData;
//using Orchard.ContentManagement.MetaData.Builders;
//using Orchard.ContentManagement.MetaData.Models;
//using Orchard.ContentManagement.ViewModels;

//namespace Orchard.InlineEditing.Settings {
//    public class EditorEvents : ContentDefinitionEditorEventsBase {
//        public override IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition) {
//            var model = definition.Settings.GetModel<ShapeInlineEditing>();
//            yield return DefinitionTemplate(model);
//        }

//        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel updateModel) {
//            var model = new ShapeInlineEditing();
//            updateModel.TryUpdateModel(model, "ShapeInlineEditing", null, null);
//            builder.WithSetting("ShapeInlineEditing.Included", model.Included.ToString());

//            yield return DefinitionTemplate(model);
//        }
//    }

//    public class ShapeInlineEditing {
//        public bool Included { get; set; }
//    }
//}