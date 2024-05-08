using DevExpress.Maui.Core;

namespace CrmDemo.Helpers;

public class StringLoader : Localizer.IStringLoader {
    public bool TryGetString(string key, out string value) {
        if (key == "CollectionViewStringId.GroupCaptionDisplayFormat") {
            value = "{1}";
            return true;
        }
        value = null;
        return false;
    }
}