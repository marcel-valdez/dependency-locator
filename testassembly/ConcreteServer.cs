namespace TestAssembly
{
    using System;
    using System.Xml;
    public class ConcreteServer : IServer
    {
        public ConcreteServer()
        {
            this.Servername = "test";
        }

        public ConcreteServer(string servername)
        {
            this.Servername = servername;
        }

        public string GerenciaName
        {
            get;
            set;
        }

        public string SubGerenciaName
        {
            get;
            set;
        }

        public string CentralName
        {
            get;
            set;
        }

        public string CentralShortName
        {
            get;
            set;
        }

        public string Servername
        {
            get;
            set;
        }

        public string IP
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }

        public string RegistrationServiceName
        {
            get;
            set;
        }

        public string GerenciaNum
        {
            get;
            set;
        }

        public string SubGerenciaNum
        {
            get;
            set;
        }

        public string CentralNum
        {
            get;
            set;
        }

        public string CorreoElectronicoDeAdministrador
        {
            get;
            set;
        }

        public ISmtpSettings SMTPAjustes
        {
            get;
            set;
        }

        public string CentralConfigName
        {
            get;
            set;
        }

        public XmlDocument SerializeToXml()
        {
            throw new NotImplementedException();
        }

        public IXmlDeserializer<IServer> GetXmlDeserializer()
        {
            throw new NotImplementedException();
        }
    }
}
