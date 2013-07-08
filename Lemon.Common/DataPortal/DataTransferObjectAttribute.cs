using System;

namespace Winterspring.DataPortal
{
    //This attribute lets the WCF service know that it's already a DTO, so we don't need to use the data mapper
    public class DataTransferObjectAttribute : Attribute
    {
    }
}