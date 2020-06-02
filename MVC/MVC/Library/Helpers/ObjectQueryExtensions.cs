using CMS.DataEngine;

namespace Generic.Library.Helpers
{
    /// <summary>
    /// Extends the ObjectQuery to include a method that can take Columns but handle if it's null.
    /// </summary>
    public static class ObjectQueryExtensions
    {
        public static ObjectQuery<TObject> ColumnsNullHandled<TObject>(this ObjectQuery<TObject> baseQuery,  string[] Columns) where TObject : BaseInfo, new()
        {
            if(Columns == null)
            {
                return baseQuery;
            } else
            {
                return baseQuery.Columns(Columns);
            }
        }

        public static ObjectQuery ColumnsNullHandled(this ObjectQuery baseQuery, string[] Columns)
        {
            if (Columns == null)
            {
                return baseQuery;
            }
            else
            {
                return baseQuery.Columns(Columns);
            }
        }
    }
}