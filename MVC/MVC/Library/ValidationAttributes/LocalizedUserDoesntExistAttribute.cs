using HBS.LocalizedValidationAttributes.Kentico.MVC;

namespace Generic.Attributes
{
    /// <summary>
    /// Checks if the User doesn't exist
    /// </summary>
    public class LocalizedUserDoesntExistAttribute : LocalizedValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return !new LocalizedUserExistsAttribute().IsValid(value);
        }
    }
}