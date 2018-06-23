namespace ValueType.Extensions.Types
{
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Gets the value of the DisplayName or Description attribute on an MemberInfo.
        /// </summary>
        /// <param name="memberInfo">The member info of a property</param>
        /// <returns>The description, falling back to ToString()</returns>
        public static string GetDataAnnotationDisplayName(this MemberInfo memberInfo)
        {
            var displayNameAttributes = memberInfo.GetCustomAttributes<DisplayNameAttribute>(false);

            if (displayNameAttributes.Any())
            {
                return displayNameAttributes.First().DisplayName;
            }

            var descriptionAttributes = memberInfo.GetCustomAttributes<DescriptionAttribute>(false);

            if (descriptionAttributes.Any())
            {
                return descriptionAttributes.First().Description;
            }

            return memberInfo.Name;
        }
    }
}
