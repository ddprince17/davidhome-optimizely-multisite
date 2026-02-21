using EPiServer.Applications;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using EPiServer.Web;

namespace DavidHome.Optimizely.MultiSite.Ui;

/// <summary>
/// Represents an editor descriptor for the <see cref="CategoryList"/> property type that allows selecting multiple categories from under a specific category with the name of the currently edited site.
/// </summary>
[EditorDescriptorRegistration(TargetType = typeof(CategoryList), UIHint = UiHint, EditorDescriptorBehavior = EditorDescriptorBehavior.ExtendBase)]
public class MultiSiteCategoryEditorDescriptor : EditorDescriptor
{
    // ReSharper disable once MemberCanBePrivate.Global => Intended if someone wants the UIHint.
    public const string UiHint = "MultiSiteCategorySelector";
    
    private readonly CategoryRepository _categoryRepository;
    private readonly IApplicationResolver _applicationResolver;
    private readonly IRequestHostResolver _requestHostResolver;

    public MultiSiteCategoryEditorDescriptor(CategoryRepository categoryRepository, IApplicationResolver applicationResolver, IRequestHostResolver requestHostResolver)
    {
        _categoryRepository = categoryRepository;
        _applicationResolver = applicationResolver;
        _requestHostResolver = requestHostResolver;
    }

    public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
    {
        base.ModifyMetadata(metadata, attributes);

        var hostResolution = _applicationResolver.GetByHostname(_requestHostResolver.HostName, false);
        var categoryRoot = _categoryRepository.GetRoot();
        
        if (hostResolution.Application?.Name != null)
        {
            var foundCategory = categoryRoot
                .GetList()
                .Cast<Category>()
                .FirstOrDefault(category => hostResolution.Application.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase));

            if (foundCategory != null)
            {
                categoryRoot = foundCategory;
            }
        }
        
        ClientEditingClass = "epi-cms/widget/CategorySelector";
        EditorConfiguration["multiple"] = true;
        EditorConfiguration["root"] = categoryRoot.ID;
    }
}