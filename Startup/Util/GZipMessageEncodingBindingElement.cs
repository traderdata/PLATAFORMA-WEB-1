//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Xml;
using System.ServiceModel;

namespace Microsoft.Samples.GZipEncoder
{
    // This is constants for GZip message encoding policy.
    static class GZipMessageEncodingPolicyConstants
    {
        public const string GZipEncodingName = "GZipEncoding";
        public const string GZipEncodingNamespace = "http://schemas.microsoft.com/ws/06/2004/mspolicy/netgzip1";
        public const string GZipEncodingPrefix = "gzip";
    }

    //This is the binding element that, when plugged into a custom binding, will enable the GZip encoder
    public class GZipMessageEncodingBindingElement 
                        : MessageEncodingBindingElement //BindingElement
                        
    {

        //We will use an inner binding element to store information required for the inner encoder
        MessageEncodingBindingElement innerBindingElement;

        //By default, use the default text encoder as the inner encoder
        public GZipMessageEncodingBindingElement()
            : this(new TextMessageEncodingBindingElement()) { }

        public GZipMessageEncodingBindingElement(MessageEncodingBindingElement messageEncoderBindingElement)
        {
            this.innerBindingElement = messageEncoderBindingElement;
        }

        public MessageEncodingBindingElement InnerMessageEncodingBindingElement
        {
            get { return innerBindingElement; }
            set { innerBindingElement = value; }
        }

        //Main entry point into the encoder binding element. Called by WCF to get the factory that will create the
        //message encoder
        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new GZipMessageEncoderFactory(innerBindingElement.CreateMessageEncoderFactory());
        }
       
        public override MessageVersion MessageVersion
        {
            get { return innerBindingElement.MessageVersion; }
            set { innerBindingElement.MessageVersion = value; }
        }

        public override BindingElement Clone()
        {
            return new GZipMessageEncodingBindingElement(this.innerBindingElement);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
            {
                return innerBindingElement.GetProperty<T>(context);
            }
            else 
            {
                return base.GetProperty<T>(context);
            }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

     }

   
}
