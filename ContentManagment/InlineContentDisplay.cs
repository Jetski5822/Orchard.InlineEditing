using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Shapes;
using Orchard.Events;
using Orchard.InlineEditing.ViewModels;
using Orchard.Logging;
using Orchard.UI.Zones;

namespace Orchard.InlineEditing.ContentManagment {
    public interface IInlineContentDisplay : IDependency {
        dynamic BuildEditor(IContent content, string metadataType, string fieldTypeName);
        dynamic UpdateEditor(IContent content, string metadataType, string modelType, string partTypeName, string fieldTypeName, IUpdateModel updateModel);
    }

    public interface IInlineContentHandler : IEventHandler {
        void UpdateEditorShape(IContent content, dynamic context, dynamic shapeFactory);
    }

    public class InlineContentDisplay : IInlineContentDisplay {
        private readonly IShapeFactory _shapeFactory;
        private readonly Lazy<IShapeTableLocator> _shapeTableLocator;
        private readonly IEnumerable<IContentPartDriver> _partDrivers;
        private readonly IEnumerable<IContentFieldDriver> _fieldDrivers;
        private readonly IContentDisplay _contentDisplay;
        private readonly RequestContext _requestContext;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IEnumerable<IInlineContentHandler> _handlers;

        public InlineContentDisplay(
            IShapeFactory shapeFactory,
            Lazy<IShapeTableLocator> shapeTableLocator,
            IEnumerable<IContentPartDriver> partDrivers,
            IEnumerable<IContentFieldDriver> fieldDrivers,
            IContentDisplay contentDisplay,
            RequestContext requestContext,
            IWorkContextAccessor workContextAccessor,
            IEnumerable<IInlineContentHandler> handlers) {
            _shapeFactory = shapeFactory;
            _shapeTableLocator = shapeTableLocator;
            _partDrivers = partDrivers;
            _fieldDrivers = fieldDrivers;
            _contentDisplay = contentDisplay;
            _requestContext = requestContext;
            _workContextAccessor = workContextAccessor;
            _handlers = handlers;

            Logger = NullLogger.Instance;
            Shape = shapeFactory;
        }

        public ILogger Logger { get; set; }            
    
        dynamic Shape { get; set; }

        public dynamic BuildEditor(IContent content, string metadataType, string fieldTypeName) {
            var shapes = _contentDisplay.BuildDisplay(content);
            InternalShapeHolder displayShape = GetShapeFromBuilt(shapes, metadataType, fieldTypeName);

            var prefix = string.Empty;

            if (displayShape.IsField) {
                prefix = displayShape.Shape.ContentPart.PartDefinition.Name + "." + displayShape.Shape.ContentField.Name;
            }
            else {
                prefix = displayShape.Shape.Metadata.Prefix;
            }

            var editorShapeType = string.Format("{0}_InlineEdit", metadataType);
            var editorShapeTemplate = editorShapeType.Replace("_", ".");

            var viewModel = new InlineViewModel { Content = content, DisplayShape = displayShape.Shape };
            _handlers.Invoke(handler => handler.UpdateEditorShape(content, viewModel, _shapeFactory), Logger);

            var editorShape = Shape.EditorTemplate(TemplateName: editorShapeTemplate, Model: viewModel, Prefix: prefix);

            return editorShape;
        }

        public dynamic UpdateEditor(IContent content, string metadataType, string modelType, string partTypeName, string fieldTypeName, IUpdateModel updateModel) {
            dynamic itemShape = CreateItemShape(metadataType);

            itemShape.ContentItem = content.ContentItem;

            var workContext = _workContextAccessor.GetContext(_requestContext.HttpContext);
            
            var theme = workContext.CurrentTheme;
            var shapeTable = _shapeTableLocator.Value.Lookup(theme.Id);

            var context = new UpdateEditorContext(itemShape, content, updateModel, string.Empty, _shapeFactory, shapeTable);

            if (string.Equals(modelType, "Part", StringComparison.OrdinalIgnoreCase)) {
                var drivers = FindPartDrivers(partTypeName);

                foreach (var driver in drivers) {
                    var result = driver.UpdateEditor(context);

                    if (result != null) {
                        result.Apply(context);
                    }
                }
            }
            if (string.Equals(modelType , "Field", StringComparison.OrdinalIgnoreCase)) {
                var contentPart =  content.ContentItem.Parts.Single(o => o.GetType().Name == partTypeName);
                var drivers = FindFieldDrivers(contentPart, fieldTypeName);

                foreach (var driver in drivers) {
                    var result = driver.UpdateEditorShape(context);

                    if (result != null) {
                        result.Apply(context);
                    }
                }
            }
            
            // Get the Drivers for the Field or the Part that we are trying to talk to.
            return itemShape;
        }

        private IEnumerable<IContentPartDriver> FindPartDrivers(string partName) {
            return from driver in _partDrivers
                   let partInfos = driver.GetPartInfo()
                   let partInfo = partInfos.FirstOrDefault(x => x.PartName == partName)
                   where partInfo != null
                   select driver;
        }

        private IEnumerable<IContentFieldDriver> FindFieldDrivers(ContentPart contentPart, string fieldName) {
            var fieldTypeName = contentPart.PartDefinition.Fields.Where(x => x.Name == fieldName).Select(x => x.FieldDefinition.Name).FirstOrDefault();
            return from driver in _fieldDrivers
                   let fieldInfos = driver.GetFieldInfo()
                   let fieldInfo = fieldInfos.FirstOrDefault(x => x.FieldTypeName == fieldTypeName)
                   where fieldInfo != null
                   select driver;
        }

        private dynamic CreateItemShape(string actualShapeType) {
            return _shapeFactory.Create(actualShapeType, Arguments.Empty(), () => new ZoneHolding(() => _shapeFactory.Create("ContentZone", Arguments.Empty())));
            //var zoneHoldingBehavior = new ZoneHoldingBehavior((Func<dynamic>)(() => _shapeFactory.Create("ContentZone", Arguments.Empty())), _workContextAccessor.GetContext().Layout);
            //return _shapeFactory.Create(actualShapeType, Arguments.Empty(), new[] { zoneHoldingBehavior });
        }

        private InternalShapeHolder GetShapeFromBuilt(dynamic editorShape, string shapeType, string fieldTypeName) {
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
                                return new InternalShapeHolder{Shape = shapeInShapeShape, IsField = true, IsPart = false};
                        }
                        else
                            return new InternalShapeHolder { Shape = shapeInShapeShape, IsField = false, IsPart = true };
                    }
                }
            }

            return null;
        }

        private class InternalShapeHolder {
            public dynamic Shape { get; set; }
            public bool IsField { get; set; }
            public bool IsPart { get; set; }
        }
    }
}