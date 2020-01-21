using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    #region Ports
    public enum Ports
    {
        [Description("Login Server Server Port")]
        LoginServerPort = 5610,
        [Description("Login Server Client Port")]
        LoginClientPort = 5611,
        [Description("Game Server Server Port")]
        GameServerPort = 5612,
        [Description("Game Server Client Port")]
        GameClientPort = 5613,
        [Description("Sync Server Server Port")]
        SyncServerPort = 5614,
        [Description("Client Server Port")]
        ClientServerPort = 5615,
        [Description("Tool Server Port")]
        ToolServerPort = 5616
    }
    #endregion

    public enum AssetType
    {
        NONE,
        [Description("Client")]
        CLIENT,
        [Description("Server")]
        SERVER,
        [Description("Tool")]
        TOOL
    }
    public enum ConnectionType
    {
        NONE,
        [Description("Game Server")]
        GAMESERVER,
        [Description("Client")]
        CLIENT,
        [Description("Login Server")]
        LOGINSERVER,
        [Description("Sync Server")]
        SYNCSERVER,
        [Description("Tool")]
        TOOL
    }
    public enum LogType
    {
        Information,
        Error,
        Debug,
        Warning,
        Connection,
        TransmissionOut,
        TransmissionIn
    }

    #region Client
    public enum ClientSendPacketNumbers
    {
        Invalid,
        LoginResponse,
        RegistrationResponse,
        CharacterList,
        ConfirmWhiteList,
        CreateCharacterResponse
    }
    public enum ClientProcessPacketNumbers
    {
        Invalid,
        LoginRequest,
        RegistrationRequest,
        CharacterListRequest,
        CreateCharacter
    }
    #endregion

    #region Game Server
    public enum GameServerProcessPacketNumbers
    {
        Invalid,
        AuthenticateServer,
        ConfirmWhiteList
    }
    public enum GameServerSendPacketNumbers
    {
        Invalid,
        AuthenticateServer,
        WhiteList,
        CreateCharacterResponse
    }
    #endregion

    #region Sync Server
    public enum SyncServerProcessPacketNumbers
    {
        Invalid,
        AuthenticateServer
    }
    public enum SyncServerSendPacketNumbers
    {
        Invalid,
        AuthenticateServer,
        RegistrationNotification,
        CreateCharacterResponse
    }
    #endregion

    #region Console Application
    public enum ServerState
    {
        None,
        Starting,
        Running,
        ShuttingDown
    }
    public enum ToolProcessPacketNumbers
    {
        SendLogs,
        AuthenticateServer
    }
    public enum ToolSendPacketNumbers
    {
        RequestLogs,
        AuthenticateServer
    }
    #endregion

    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = System.Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAttribute = memInfo[0]
                            .GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .FirstOrDefault() as DescriptionAttribute;

                        if (descriptionAttribute != null)
                        {
                            return descriptionAttribute.Description;
                        }
                    }
                }
                return "";
            }
            return null;
        }
    }
}



