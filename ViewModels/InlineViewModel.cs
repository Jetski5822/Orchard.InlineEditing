using System.Collections.Generic;
using Orchard.ContentManagement;

namespace Orchard.InlineEditing.ViewModels {
    public class InlineViewModel {
        public InlineViewModel() {
            ExternalShapes = new List<dynamic>();
        }

        public dynamic DisplayShape { get; set; }
        public IContent Content { get; set; }
        public IEnumerable<dynamic> ExternalShapes { get; set; }
    }
}