using System;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.DisplayManagement.Shapes;
using Orchard.InlineEditing.ViewModels;
using Orchard.Mvc.Html;

namespace Orchard.InlineEditing {
    public static class HtmlHelperExtensions {
        public static IDisposable InlineEdit(this HtmlHelper htmlhelper,
            Orchard.Mvc.ViewEngines.Razor.WebViewPage<dynamic> webViewPage) {

            var viewModel = (InlineViewModel)webViewPage.Model;
            
            TagBuilder tagBuilder = null;

            Shape shape = (Shape)viewModel.DisplayShape;

            var contentPart = shape.Properties["ContentPart"] as ContentPart;
            var contentField = shape.Properties["ContentField"] as ContentField;

            if (contentPart != null && contentField == null) {
                tagBuilder = InlineEditForPartTagBuilder(webViewPage, viewModel, contentPart.GetType().Name);
            }
            else if (contentPart != null && contentField != null) {
                tagBuilder = InlineEditForFieldTagBuilder(webViewPage, viewModel, contentPart.GetType().Name, contentField.Name);
            }
            else
                return null;
            
            return webViewPage.Capture(html => {
                webViewPage.Output.Write(tagBuilder.ToString(TagRenderMode.StartTag));
                webViewPage.Output.Write(html);
                webViewPage.Output.Write(webViewPage.Display.ExternalShapes(Shapes: viewModel.ExternalShapes));
                webViewPage.Output.Write(webViewPage.Display.ShapeEditorActions());
                webViewPage.Output.Write(htmlhelper.AntiForgeryTokenOrchard());
                webViewPage.Output.Write(tagBuilder.ToString(TagRenderMode.EndTag));
            });
        }

        private static TagBuilder InlineEditForPartTagBuilder(Mvc.ViewEngines.Razor.WebViewPage<dynamic> webViewPage, InlineViewModel viewModel, string partTypeName) {

            var tagBuilder = new TagBuilder("form");

            tagBuilder.MergeAttribute("action", webViewPage.Url.EditShape(viewModel.Content.Id, (string)viewModel.DisplayShape.Metadata.Type, "Part", partTypeName));
            tagBuilder.MergeAttribute("method", "post", true);
            tagBuilder.MergeAttribute("enctype", "multipart/form-data", true);

            return tagBuilder;
        }

        private static TagBuilder InlineEditForFieldTagBuilder(Mvc.ViewEngines.Razor.WebViewPage<dynamic> webViewPage, InlineViewModel viewModel, string partTypeName, string fieldTypeName) {

            var tagBuilder = new TagBuilder("form");

            tagBuilder.MergeAttribute("action", webViewPage.Url.EditShape(viewModel.Content.Id, (string)viewModel.DisplayShape.Metadata.Type, "Field", partTypeName, fieldTypeName));
            tagBuilder.MergeAttribute("method", "post", true);
            tagBuilder.MergeAttribute("enctype", "multipart/form-data", true);

            return tagBuilder;
        }
    }
}