using System;
using System.Linq;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Implementation;
using Orchard.DisplayManagement.Shapes;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Admin;

namespace Orchard.InlineEditing {
    public class InlineShapeFactory : IShapeFactoryEvents, IShapeDisplayEvents {
        private readonly IAuthorizer _authorizer;
        private readonly IShapeTableManager _shapeTableManager;
        private readonly IThemeManager _themeManager;
        private readonly WorkContext _workContext;

        public InlineShapeFactory(IAuthorizer authorizer, 
            IWorkContextAccessor workContextAccessor,
            IShapeTableManager shapeTableManager,
            IThemeManager themeManager) {
            _authorizer = authorizer;
            _shapeTableManager = shapeTableManager;
            _themeManager = themeManager;
            _workContext = workContextAccessor.GetContext();
        }

        private bool IsActivable(IContent content) {
            // activate on front-end only
            if (AdminFilter.IsApplied(new RequestContext(_workContext.HttpContext, new RouteData())))
                return false;

            if (!_authorizer.Authorize(Permissions.EditContent, content))
                return false;

            return true;
        }

        public void Creating(ShapeCreatingContext context) {
        }

        public void Created(ShapeCreatedContext context) {
        }

        public void Displaying(ShapeDisplayingContext context) {
            var shape = context.Shape;
            var content = shape.ContentItem as IContent;
            if (content == null)
                return;

            if (!IsActivable(content)) {
                return;
            }
            
            var shapeMetadata = (ShapeMetadata) context.Shape.Metadata;

            if (shapeMetadata.Wrappers.Contains("InlineShapeWrapper"))
                return;

            if (shapeMetadata.Type != "Widget"
                && shapeMetadata.Type != "EditorTemplate") {

                var currentTheme = _themeManager.GetRequestTheme(_workContext.HttpContext.Request.RequestContext);
                var shapeTable = _shapeTableManager.GetShapeTable(currentTheme.Id);

                foreach (var alternate in shapeMetadata.Alternates) {
                    if (shapeTable.Descriptors.ContainsKey(alternate + "_InlineEdit")) {
                        shapeMetadata.Wrappers.Add("InlineShapeWrapper");
                        return;
                    }
                }

                if (shapeTable.Descriptors.ContainsKey(shapeMetadata.Type + "_InlineEdit")) {
                    shapeMetadata.Wrappers.Add("InlineShapeWrapper");
                }
                
            }
        }

        public void Displayed(ShapeDisplayedContext context) {
        }    
    }
}