using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Csla;
using ProtoBuf;

namespace Winterspring.DataPortal
{
    [Serializable]
    public class PackagedDataTransferObject
    {
        private byte[] _h;  //Having a short field name makes the serialized version of this smaller.
        public byte[] Header { get { return _h; } private set { _h = value; } }

        private byte[] _m;  //Having a short field name makes the serialized version of this smaller.
        public byte[] Message
        {
            get { return _m; }
            internal set { _m = value; }
        }

        public static PackagedDataTransferObject PackageBO(object o)
        {
            var isDto = o.GetType().GetCustomAttributes(typeof(DataTransferObjectAttribute), false).Length > 0;
            return PackageDTO(isDto ? o : DataMapper.Instance.Map(o));
        }

        public static PackagedDataTransferObject PackageDTO(object o)
        {
            var m = new PackagedDataTransferObject();
            using (var ms = new MemoryStream())
            {
                var dataPortalMessageHeader = new DataPortalMessageHeader { SerializationType = SerializationType.ProtoBuf, Type = o.GetType().AssemblyQualifiedName, AuthenticationToken = ApplicationContext.ClientContext["AuthenticationToken"] as string };
                Serializer.Serialize(ms, dataPortalMessageHeader);
                m.Header = ms.ToArray();
            }

            //Protobuf can also do some automatic serializations
            //http://marcgravell.blogspot.ca/2011/08/automatic-serialization-whats-in-tuple.html
            using (var ms = new MemoryStream())
            {
                //Serialize o using ProtoBuf into ms.  Since Serialize is overloaded and generic, this is harder than normal.
                var genericSerializeMethod = typeof(Serializer)
                    .GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .First(x => x.ContainsGenericParameters && x.Name == "Serialize" && x.GetParameters()[0].ParameterType == typeof(Stream))
                    .MakeGenericMethod(o.GetType());
                genericSerializeMethod.Invoke(null, new[] { ms, o });

                m.Message = ms.ToArray();
            }

            return m;
        }

        public static PackagedDataTransferObject PackageServerException(Exception ex)
        {
            var m = new PackagedDataTransferObject();

            using (var ms = new MemoryStream())
            {
                var dataPortalMessageHeader = new DataPortalMessageHeader { SerializationType = SerializationType.Binary, Type = ex.GetType().AssemblyQualifiedName, AuthenticationToken = String.Empty };
                Serializer.Serialize(ms, dataPortalMessageHeader);
                m.Header = ms.ToArray();
            }

            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, ex);
                m.Message = ms.ToArray();
            }

            return m;
        }

        public UnpackagedDataTransferObject UnpackageDataTransferObject()
        {
            DataPortalMessageHeader header;
            object body;
            using (var ms = new MemoryStream(Header))
            {
                header = Serializer.Deserialize<DataPortalMessageHeader>(ms);
            }

            using (var ms = new MemoryStream(Message))
            {
                if (header.SerializationType == SerializationType.ProtoBuf)
                {
                    //Protobuf can also do some automatic serializations
                    //http://marcgravell.blogspot.ca/2011/08/automatic-serialization-whats-in-tuple.html

                    //Deserialize (specifying the expected type) into a DTO
                    var dtoType = Type.GetType(header.Type);
                    var genericDeserializeMethod = typeof(Serializer)
                        .GetMethod("Deserialize", BindingFlags.Static | BindingFlags.Public)
                        .MakeGenericMethod(dtoType);
                    body = genericDeserializeMethod.Invoke(null, new object[] { ms });
                }
                else
                {
                    //The only other option right now is binary serialization
                    var formatter = new BinaryFormatter();
                    body = formatter.Deserialize(ms);
                }
            }
            return new UnpackagedDataTransferObject { Body = body, Header = header };
        }

    }
}