using Assets.Generic;
using System;

namespace Assets.Network
{
    /// <summary>
    /// This is used to send messages between server and clients. Please create an instance of this
    /// with a normal constructor. Use the data field as your way of inputing a class you want to send
    /// over the network.
    /// 
    /// Note that the class passed in data should be as small as possible. And you should add MessageTypes
    /// to use with differente messages that you want to send around. 
    /// </summary>
    [Serializable]
    public class MessageBase
    {
        public MessageType type;
        public byte[] data;

        /// <param name="data">The class to be sent. (It has to be serializable)</param>
        /// <param name="type">The message type that the listener will use to interpret the data.</param>
        public MessageBase(object data, MessageType type)
        {
            this.type = type;
            if (data != null)
            {
                this.data = Extension.ObjectToByteArray(data);
            }
            else
                this.data = null;
        }

        /// <summary>
        /// Convert this Message into a byte array.
        /// </summary>
        public byte[] ToByteArray()
        {
            return Extension.ObjectToByteArray(this);
        }
        /// <summary>
        /// This is used to transform a byte array sent from another device into a MessageBase.
        /// </summary>
        /// <param name="array">The transmited data.</param>
        public static MessageBase FromByteArray(byte[] array)
        {
            return Extension.ByteArrayToObject<MessageBase>(array);
        }
        /// <summary>
        /// Use this to extract the associated class from the MessageBase.
        /// </summary>
        /// <typeparam name="T">The class type to be extracted.</typeparam>
        public T GetData<T>()
        {
            if (data != null)
                return Extension.ByteArrayToObject<T>(data);
            else
                return default(T);
        }

        public override string ToString()
        {
            if (data != null)
                return data.ToString();
            else
                return "";
        }
    }
}