using System;
using System.Collections;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.DisplayManagement.Shapes;
using Orchard.InlineEditing.ContentManagment;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Security;

namespace Orchard.InlineEditing.Controllers {
    [ValidateInput(false)]
    public class ShapeController : Controller, IUpdateModel {
        private readonly IAuthorizer _authorizer;
        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;
        private readonly IInlineContentDisplay _inlineContentDisplay;
        private readonly IContentDisplay _contentDisplay;
        protected ILogger Logger { get; set; }

        public ShapeController(
            IAuthorizer authorizer, 
            IOrchardServices orchardServices,
            IInlineContentDisplay inlineContentDisplay,
            IContentDisplay contentDisplay) {
            _authorizer = authorizer;
            _orchardServices = orchardServices;
            _contentManager = _orchardServices.ContentManager;
            _inlineContentDisplay = inlineContentDisplay;
            _contentDisplay = contentDisplay;
            Logger = NullLogger.Instance;
        }

        public ActionResult Edit(int id, string metadataType, string fieldTypeName) {
            var contentItem = _contentManager.Get(id);
            if (!_authorizer.Authorize(Permissions.EditContent, contentItem))
                throw new UnauthorizedAccessException();

            var editorShape = _inlineContentDisplay.BuildEditor(contentItem, metadataType, fieldTypeName);

            return PartialView((object)editorShape);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(int id, string metadataType, string modelType, string partTypeName, string fieldTypeName) {
            var contentItem = _contentManager.Get(id);
            if (!_authorizer.Authorize(Permissions.EditContent, contentItem))
                throw new UnauthorizedAccessException();

            var editorShape = _inlineContentDisplay.UpdateEditor(contentItem, metadataType, modelType, partTypeName, fieldTypeName, this);

            if (!ModelState.IsValid) {
                _orchardServices.TransactionManager.Cancel();

                return PartialView((object)null);
            }

            var displayShape = (Shape)_contentDisplay.BuildDisplay(contentItem);

            displayShape.Metadata.Wrappers.Clear();

            dynamic theShape = GetShapeFromBuilt(displayShape, metadataType, fieldTypeName);

            return new ShapePartialResult(this, (object)theShape);
        }

        private dynamic GetShapeFromBuilt(dynamic editorShape, string shapeType, string fieldTypeName) {
            foreach (DictionaryEntry entry in editorShape.Properties) {
                var internalShape = entry.Value as Shape;
                if (internalShape == null)
                    continue;

                foreach (var item in internalShape.Items) {
                    var shapeInShapeShape = item as Shape;
                    if (shapeInShapeShape == null)
                        continue;

                    if (shapeInShapeShape.Metadata.Type == shapeType) {
                        if (!string.IsNullOrWhiteSpace(fieldTypeName)) {
                            var temp = shapeInShapeShape.Properties["ContentField"] as ContentField;
                            if (temp != null && temp.Name == fieldTypeName)
                                return shapeInShapeShape;
                        }
                        else
                            return shapeInShapeShape;
                    }
                }
            }

            return null;
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

        private class ViewDataContainer : IViewDataContainer {
            public ViewDataDictionary ViewData { get; set; }
        }
    }
}