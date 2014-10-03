using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Traderdata.Client.TerminalWEB.Util
{
    public class ClassMessageInspector : IClientMessageInspector
    {
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {

        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            //Cabeçalho
            MessageHeader mhg = MessageHeader.CreateHeader("loginCliente", "Traderdata.Headers", ServiceWCF.LoginUsuario);
            request.Headers.Add(mhg);
                        

            //Cabeçalho
            MessageHeader mhg2 = MessageHeader.CreateHeader("userGuid", "Traderdata.Headers", ServiceWCF.Guids);
            request.Headers.Add(mhg2);

            //Cabeçalho
            MessageHeader mhg3 = MessageHeader.CreateHeader("loginSistema", "Traderdata.Headers", ServiceWCF.LoginSistema);
            request.Headers.Add(mhg3);

            //Retorno
            return request;
        }
    }
}
