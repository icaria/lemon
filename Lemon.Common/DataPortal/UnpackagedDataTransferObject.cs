namespace Winterspring.DataPortal
{
    public class UnpackagedDataTransferObject
    {
        public DataPortalMessageHeader Header { get; set; }
        public object Body { get; set; }

        public object UnpackageBO()
        {
            var isDto = Body.GetType().GetCustomAttributes(typeof(DataTransferObjectAttribute), false).Length > 0;
            return isDto ? Body : DataMapper.Instance.Map(Body);
        }
    }
}