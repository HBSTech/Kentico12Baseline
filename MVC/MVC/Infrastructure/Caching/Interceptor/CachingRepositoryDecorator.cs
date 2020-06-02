﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using Castle.DynamicProxy;

namespace MVCCaching.Kentico
{
    /// <summary>
    /// Provides caching for repository methods that return a single content item or a collection of content items. 
    /// Only results of methods starting with 'Get' are cached. A minimum set of cache dependencies is also specified, 
    /// so that when a content item changes, the cached result is invalidated.
    /// </summary>
    public sealed class CachingRepositoryDecorator : IInterceptor
    {
        private readonly TimeSpan mCacheItemDuration;
        private readonly IContentItemMetadataProvider mContentItemMetadataProvider;
        private readonly bool mCacheEnabled;
        private readonly IOutputCacheDependencies mOutputCacheDependencies;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingRepositoryDecorator"/> class.
        /// </summary>
        /// <param name="cacheItemDuration">Time duration during which the repository method result is cached.</param>
        /// <param name="contentItemMetadataProvider">Object that provides information about pages and info objects using their runtime type.</param>
        /// <param name="cacheEnabled">Indicates whether caching is enabled.</param>
        public CachingRepositoryDecorator(TimeSpan cacheItemDuration, IContentItemMetadataProvider contentItemMetadataProvider, bool cacheEnabled, IOutputCacheDependencies outputCacheDependencies)
        {
            mCacheItemDuration = cacheItemDuration;
            mContentItemMetadataProvider = contentItemMetadataProvider;
            mCacheEnabled = cacheEnabled;
            mOutputCacheDependencies = outputCacheDependencies;
        }


        /// <summary>
        /// Returns the cached result for the specified method invocation, if possible. Otherwise proceeds with the invocation and caches the result.
        /// Only results of methods starting with 'Get' are affected.
        /// </summary>
        /// <param name="invocation">Method invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            if (!mCacheEnabled || !invocation.Method.IsPublic)
            {
                invocation.Proceed();

                return;
            }

            var returnType = invocation.Method.ReturnType;

            var cacheDependencyAttributes = invocation.MethodInvocationTarget.GetCustomAttributes<CacheDependencyAttribute>().ToList();
            var doNotCacheAttributes = invocation.MethodInvocationTarget.GetCustomAttributes<DoNotCacheAttribute>().ToList();

            // Either Cache or Retrieve, can modify and include custom logic for DependencyCacheKey generation
            if (doNotCacheAttributes.Count > 0)
            {
                invocation.Proceed();
            }
            else if (cacheDependencyAttributes.Count > 0)
            {
                invocation.ReturnValue = GetCachedResult(invocation, GetDependencyCacheKeyFromAttributes(cacheDependencyAttributes, invocation.Arguments));
            }
            else if (typeof(TreeNode).IsAssignableFrom(returnType))
            {
                invocation.ReturnValue = GetCachedResult(invocation, GetDependencyCacheKeyForPage(returnType));
            }
            else if (typeof(IEnumerable<TreeNode>).IsAssignableFrom(returnType))
            {
                invocation.ReturnValue = GetCachedResult(invocation, GetDependencyCacheKeyForPage(returnType.GenericTypeArguments[0]));
            }
            else if (typeof(BaseInfo).IsAssignableFrom(returnType))
            {
                invocation.ReturnValue = GetCachedResult(invocation, GetDependencyCacheKeyForObject(returnType));
            }
            else if (typeof(IEnumerable<BaseInfo>).IsAssignableFrom(returnType))
            {
                invocation.ReturnValue = GetCachedResult(invocation, GetDependencyCacheKeyForObject(returnType.GenericTypeArguments[0]));
            }
            else
            {
                invocation.Proceed();
            }
        }

        /// <summary>
        /// Gets the Cached result of the function based on the Invocation Name (Cache Key)
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="dependencyCacheKey"></param>
        /// <returns></returns>
        private object GetCachedResult(IInvocation invocation, string dependencyCacheKey)
        {
            var cacheKey = GetCacheItemKey(invocation);
            var cacheSettings = CreateCacheSettings(cacheKey, dependencyCacheKey);

            // Add dependencies to output caching
            mOutputCacheDependencies.AddCacheItemDependencies(dependencyCacheKey.Split(TextHelper.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

            Func<Object> provideData = () =>
            {
                invocation.Proceed();
                return invocation.ReturnValue;
            };
            return CacheHelper.Cache(provideData, cacheSettings);
        }

        /// <summary>
        /// Retrieves the Cache Key based on the Invocation, can modify if desired
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        private string GetCacheItemKey(IInvocation invocation)
        {
            var DoNotIncludeCultureInCacheName = invocation.MethodInvocationTarget.GetCustomAttributes<CacheNameNoCultureAttribute>().ToList().Count > 0;
            var DoNotIncludeSiteInCacheName = invocation.MethodInvocationTarget.GetCustomAttributes<CacheNameNoSiteAttribute>().ToList().Count > 0;

            var builder = new StringBuilder(127)
                          .Append("CachingRepositoryDecorator|Data")
                          .Append("|").Append(DoNotIncludeSiteInCacheName ? "" : SiteContext.CurrentSiteName)
                          .Append("|").Append(invocation.TargetType.FullName)
                          .Append("|").Append(invocation.Method.Name)
                          .Append("|").Append(DoNotIncludeCultureInCacheName ? "" : CultureInfo.CurrentCulture.Name);

            foreach (var value in invocation.Arguments)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, "|{0}", GetArgumentCacheKey(value));
            }

            return builder.ToString();
        }


        private CacheSettings CreateCacheSettings(string cacheKey, string dependencyCacheKey)
        {
            return new CacheSettings(mCacheItemDuration.TotalMinutes, cacheKey)
            {
                GetCacheDependency = () => CacheHelper.GetCacheDependency(dependencyCacheKey)
            };
        }

        private string GetDependencyCacheKeyForPage(Type type)
        {
            return String.Format(PagesCacheDependencyAttribute.KEY_FORMAT, SiteContext.CurrentSiteName.ToLowerInvariant(), mContentItemMetadataProvider.GetClassNameFromPageRuntimeType(type));
        }


        private string GetDependencyCacheKeyForObject(Type type)
        {
            return String.Format("{0}|all", mContentItemMetadataProvider.GetObjectTypeFromInfoObjectRuntimeType(type));
        }

        /// <summary>
        /// Resolves the Cache Keys, passing the SiteName to replace ##SITENAME## in the macros
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="methodArguments"></param>
        /// <returns></returns>
        private string GetDependencyCacheKeyFromAttributes(List<CacheDependencyAttribute> attributes, object[] methodArguments)
        {
            return GetDependencyCacheKeyArrayFromAttributes(attributes, methodArguments).Join(TextHelper.NewLine);
        }

        /// <summary>
        /// Gets the Cache Dependency Keys from the given attributes
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="methodArguments"></param>
        /// <returns></returns>
        private string[] GetDependencyCacheKeyArrayFromAttributes(List<CacheDependencyAttribute> attributes, object[] methodArguments)
        {
            return attributes.Select(attribute => attribute.ResolveKey(SiteContext.CurrentSiteName.ToLowerInvariant(), methodArguments)).ToArray();
        }


        private string GetArgumentCacheKey(object argument)
        {
            if (argument == null)
            {
                return string.Empty;
            }
            if (argument is ICacheKey)
            {
                return ((ICacheKey)argument).GetCacheKey();
            }
            if(argument is IEnumerable<object>)
            {
                return string.Join("-", ((IEnumerable<object>)argument).Select(x => {
                    return (x is ICacheKey ? ((ICacheKey)x).GetCacheKey() : x.ToString());
                }));
            }
            return argument.ToString();
        }
    }
}