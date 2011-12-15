namespace TestAssembly
{
    using System.Xml;

    public interface IServer
    {
        string GerenciaName { get; set; }

        string SubGerenciaName { get; set; }

        string CentralName { get; set; }

        string CentralShortName { get; set; }

        string Servername { get; set; }

        string IP { get; set; }

        int Port { get; set; }

        string Path { get; set; }

        string RegistrationServiceName { get; set; }

        string GerenciaNum { get; set; }

        string SubGerenciaNum { get; set; }

        string CentralNum { get; set; }

        string CorreoElectronicoDeAdministrador { get; set; }

        ISmtpSettings SMTPAjustes { get; set; }

        string CentralConfigName { get; set; }

        XmlDocument SerializeToXml();

        IXmlDeserializer<IServer> GetXmlDeserializer();
    }
}