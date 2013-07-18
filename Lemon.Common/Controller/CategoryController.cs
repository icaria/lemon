using System.Linq;
using Winterspring.Extensions;

namespace Lemon.Common
{
    public static class CategoryController
    {
        public static ICategoryCache Cache { get { return ServiceLocator.Get<ICategoryCache>(); } }

        public static CategoryCacheObject GetByName(string name)
        {
            return Cache.FirstOrDefault(x => x.Name.Trim().ToLower() == name.Trim().ToLower());
        }

        public static string GetCategoryName(int? categoryId)
        {
            return categoryId == null ? "" : GetCategoryName((int)categoryId);
        }

        public static string GetCategoryName(int categoryId)
        {
            return Cache[categoryId].Try(x => x.Name) ?? "";
        }
    }
}