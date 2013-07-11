using ProtoBuf.Meta;

namespace Winterspring.DataPortal
{
    public static class Initializer
    {
        public static void Initialize()
        {
            DataMapper.Instance.Initialize();

            RuntimeTypeModel.Default.Add(typeof(Csla.Server.EmptyCriteria), false);
        }

    }
}
