using System.Web.Mvc;

namespace Orchard.InlineEditing {
    public static class UrlHelperExtenstions {
        public static string GetEditShape(this UrlHelper urlHelper, int contentItemId, string metadataType) {
            return urlHelper.Action("Edit", "Shape", new { area = Constants.LocalArea, id = contentItemId, metadataType = metadataType });
        }
        
        public static string GetEditShape(this UrlHelper urlHelper, int contentItemId, string metadataType, string fieldTypeName) {
            return urlHelper.Action("Edit", "Shape", new { area = Constants.LocalArea, id = contentItemId, metadataType = metadataType, fieldTypeName = fieldTypeName });
        }

        public static string EditShape(this UrlHelper urlHelper, int contentItemId, string metadataType, string modelType, string partTypeName) {
            return urlHelper.Action("Edit", "Shape", new { area = Constants.LocalArea, id = contentItemId, metadataType = metadataType, modelType = modelType, partTypeName = partTypeName });
        }

        public static string EditShape(this UrlHelper urlHelper, int contentItemId, string metadataType, string modelType, string partTypeName, string fieldTypeName) {
            return urlHelper.Action("Edit", "Shape", new { area = Constants.LocalArea, id = contentItemId, metadataType = metadataType, modelType = modelType, partTypeName = partTypeName, fieldTypeName = fieldTypeName });
        }
    }
}