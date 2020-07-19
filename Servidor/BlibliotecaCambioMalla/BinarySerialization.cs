using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Reflection;

namespace BlibliotecaCambioMalla
{
    public class BinarySerialization
    {

        //Metodo para serializar
        public static byte[] Serializate(object toSerializate)
        {
            //instancia de MemoryStream
            MemoryStream memory = new MemoryStream();

            //
            BinaryFormatter formatter = new BinaryFormatter();

            //se guarda en el stream el objeto serializable
            formatter.Serialize(memory, toSerializate);

            //se convierte en un array
            return memory.ToArray();
        }


        public static Object Deserializate(byte[] data)
        {
            //instancia de MemoryStream con el array que se quiere 
            //deserializar
            MemoryStream memory = new MemoryStream(data);

            //
            BinaryFormatter formatter = new BinaryFormatter();

            //se retorna un objeto a partir de el stream
            Object objDes = formatter.Deserialize(memory);


            return objDes;

            
        }

    }
}
