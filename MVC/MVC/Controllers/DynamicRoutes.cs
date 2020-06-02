using CMS.DocumentEngine.Types.Generic;
using DynamicRouting.Kentico.MVC;

[assembly: DynamicRouting("PageTypes/Footer", typeof(Footer), Footer.CLASS_NAME)]
[assembly: DynamicRouting("PageTypes/Navigation", typeof(Navigation), Navigation.CLASS_NAME)]
[assembly: DynamicRouting("PageTypes/GenericPage", typeof(GenericPage), GenericPage.CLASS_NAME, useOutputCaching: true)]