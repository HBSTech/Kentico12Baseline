using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using HBS.LocalizedValidationAttributes.Kentico.MVC;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace Generic.Attributes
{
    /// <summary>
    /// Checks if the password matches the current Kentico Settings Password policy
    /// </summary>
    public class LocalizedPasswordPolicyAttribute : LocalizedValidationAttribute, IClientValidatable
    {

        public override bool IsValid(object value)
        {
            if (value is string)
            {
                string Password = value.ToString();
                ISiteRepository _SiteRepo = DependencyResolver.Current.GetService<ISiteRepository>();
                IUserService _UserService = DependencyResolver.Current.GetService<IUserService>();
                return _UserService.ValidatePasswordPolicy(Password, _SiteRepo.CurrentSiteName());
            }
            return false;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ISiteSettingsRepository _SiteSettingsRepo = DependencyResolver.Current.GetService<ISiteSettingsRepository>();
            ISiteRepository _SiteRepo = DependencyResolver.Current.GetService<ISiteRepository>();
            var PasswordSettings = _SiteSettingsRepo.GetPasswordPolicy(_SiteRepo.CurrentSiteName());
            List<ModelClientValidationRule> rules = new List<ModelClientValidationRule>();
            if (!PasswordSettings.UsePasswordPolicy)
            {
                return rules;
            }
            string ErrorMessage = GetErrorMessage(metadata.GetDisplayName());
            if (PasswordSettings.MinLength > 0)
            {
                rules.Add(new ModelClientValidationMinLengthRule(ErrorMessage, PasswordSettings.MinLength));
            }
            // can only have one regex
            if (!string.IsNullOrWhiteSpace(PasswordSettings.Regex))
            {
                rules.Add(new ModelClientValidationRegexRule(ErrorMessage, PasswordSettings.Regex));
            }
            else if (PasswordSettings.NumNonAlphanumericChars > 0)
            {
                string Rule = "(?=.*";
                // Add a \W_ (at least 1) + Anything else rule for each non alphanumeric
                for (int nc = 0; nc < PasswordSettings.NumNonAlphanumericChars; nc++)
                {
                    Rule += "[\\W_]+.*";
                }
                Rule += ").";
                if (PasswordSettings.MinLength > 0)
                {
                    Rule += "{" + PasswordSettings.MinLength + ",}";
                }
                else
                {
                    Rule += "*";
                }
                rules.Add(new ModelClientValidationRegexRule(ErrorMessage, Rule));
            }
            return rules;
        }

        /// <summary>
        /// Get localized error message, can be overwritten by Password Policy message in settings
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetErrorMessage(string name)
        {
            ISiteSettingsRepository _SiteSettingsRepo = DependencyResolver.Current.GetService<ISiteSettingsRepository>();
            ISiteRepository _SiteRepo = DependencyResolver.Current.GetService<ISiteRepository>();
            var PasswordSettings = _SiteSettingsRepo.GetPasswordPolicy(_SiteRepo.CurrentSiteName());
            string NewErrorMessage = LocalizeString(!string.IsNullOrWhiteSpace(PasswordSettings.ViolationMessage) ? PasswordSettings.ViolationMessage : ErrorMessage);
            return string.Format(CultureInfo.CurrentCulture, (NewErrorMessage ?? ""), name);
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, GetErrorMessage(name), name);
        }
    }
}