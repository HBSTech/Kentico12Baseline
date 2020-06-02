using AutoMapper;
using CMS.DocumentEngine;
using CMS.Membership;
using Generic.Models;
using Generic.Models.User;

namespace Generic.AutoMapper
{
    public static class AutoMapperMaps
    {
        /// <summary>
        /// Your solution's Map go here
        /// </summary>
        /// <returns></returns>
        public static MapperConfiguration GetMapConfiguration()
        {
            return new MapperConfiguration(cfg =>
            {
                // Breadcrumbs
                cfg.CreateMap<TreeNode, Breadcrumb>()
                .ForMember(dest => dest.LinkText, opt => opt.MapFrom(src => src.DocumentName))
                .ForMember(dest => dest.LinkUrl, opt => opt.MapFrom(src => src.RelativeURL));

                // Used for when we get a NavItem from cache so the List of it is not the same
                cfg.CreateMap<NavigationItem, NavigationItem>()
                .ForMember(dest => dest.Children, opt => opt.Ignore());

                // Page to Navigation Item
                cfg.CreateMap<TreeNode, NavigationItem>()
                .BeforeMap((s, d) => d.LinkTarget = "_self")
                .ForMember(dest => dest.LinkText, opt => opt.MapFrom(src => src.DocumentName))
                .ForMember(dest => dest.LinkTarget, opt => opt.MapFrom(src => src.RelativeURL))
                .ForMember(dest => dest.LinkPagePath, opt => opt.MapFrom(src => src.NodeAliasPath))
                .ForMember(dest => dest.LinkPageGuid, opt => opt.MapFrom(src => src.NodeGUID))
                .ForMember(dest => dest.LinkPageID, opt => opt.MapFrom(src => src.NodeID));

                cfg.CreateMap<BasicUser, UserInfo>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserEmail));
            });
        }
    }
}