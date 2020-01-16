using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    #region Ports
    public enum Ports
    {
        LoginServerPort = 5610,
        LoginClientPort = 5611,
        GameServerPort = 5612,
        GameClientPort = 5613,
        SyncServerPort = 5614,
        ClientServerPort = 5615
    }
    #endregion

    public enum AssetType
    {
        NONE,
        CLIENT,
        SERVER
    }
    public enum ConnectionType
    {
        NONE,
        GAMESERVER,
        CLIENT,
        LOGINSERVER,
        SYNCSERVER
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
}
