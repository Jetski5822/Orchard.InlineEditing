using Orchard.UI.Resources;

namespace Orchard.InlineEditing {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest.DefineScript("InlineEditUI").SetUrl("orchard-inlineedit.ui.js").SetDependencies("jQuery");
            manifest.DefineScript("InlineEdit").SetUrl("orchard-inlineedit.js").SetDependencies("InlineEditUI");
            
            manifest.DefineStyle("InlineShape").SetUrl("orchard-inlineshape.css");

            manifest.DefineScript("InlineEditor_DefaultEditor").SetUrl("orchard-inlineedit-defaulteditor.js").SetDependencies("InlineEdit");
        }
    }
}
