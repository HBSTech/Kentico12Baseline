using CMS.DocumentEngine;
using MVCCaching;
using System;

namespace Generic.Repositories.Interfaces
{
    public interface IMediaRepository : IRepository
    {
        /// <summary>
        /// Gets the Attachment Url
        /// </summary>
        /// <param name="Page">The Page</param>
        /// <param name="ImageGuid">The Attachment Guid</param>
        /// <returns></returns>
        string GetAttachmentImage(TreeNode Page, Guid ImageGuid);

        /// <summary>
        /// Gets the Media File Url
        /// </summary>
        /// <param name="FileGuid">The media file Guid</param>
        /// <returns>The Media File Url</returns>
        string GetMediaFileUrl(Guid FileGuid);
    }
}