using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Silkroad_Fusion.API;
using System.Threading;

namespace Silkroad_Fusion
{
    public class Proxy
    {
        #region Kernel Shit
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        static extern uint ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, uint lpbuffer, uint nSize, uint lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        static extern uint WriteProcessMemory(IntPtr hProcess, uint lpBaseAddress, byte[] lpBuffer, int nSize, uint lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        static extern uint VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32.dll")]
        static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("kernel32")]
        static extern uint GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32.dll")]
        static extern uint WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32.dll")]
        static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
        #endregion

        #region Variables
        public static IntPtr Handle;
        public const byte SRLocal = 18; //18=iSRO,40=rSRO
        const int SRVersion = 439;

        private static bool mClientConnected;
        public static bool ClientConnected
        {
            get { return mClientConnected; }
        }

        private static ushort mLocalPort;
        private static string mGatewayIP;

        private static Client mClientSocket;
        private static Server mGatewaySocket;
        private static Server mAgentSocket;
        private static string agentServerIP;
        private static ushort agentServerPort;
        private static bool doAgentServerConnect;
        private static bool isConnectedToAgentServer;
        public static bool isUsingBotLogin = false;
        public static uint sessionID;
        public static bool isLoggedIn = false;
        public static string Passcode;
        public static string ID;
        public static string PW;
        public static string charname;
        public static bool loginSuccess;
        #endregion


        public static void Start(ushort localPort, string gatewayIP)
        {
            //Store parameters
            mLocalPort = localPort;
            mGatewayIP = gatewayIP;

            //Create ClientSocket instance and link the Events
            mClientSocket = new Client();
            mClientSocket.Connected += new Client.ConnectedEventHandler(ClientSocket_Connected);
            mClientSocket.Disconnected += new Client.DisconnectedEventHandler(ClientSocket_Disconnected);
            mClientSocket.PacketReceived += new Client.PacketReceivedEventHandler(ClientSocket_PacketReceived);

            //Listen for Client        
            mClientSocket.Listen(localPort);

            //Create GatewayServer instance and link the Events
            mGatewaySocket = new Server();
            mGatewaySocket.Connected += new Server.ConnectedEventHandler(GatewaySocket_Connected);
            mGatewaySocket.Disconnected += new Server.DisconnectedEventHandler(GatewaySocket_Disconnected);
            mGatewaySocket.Kicked += new Server.KickedEventHandler(GatewaySocket_Kicked);
            mGatewaySocket.PacketReceived += new Server.PacketReceivedEventHandler(GatewaySocket_PacketReceived);

            //Create AgentSocket instance and link the Events
            mAgentSocket = new Server();
            mAgentSocket.Connected += new Server.ConnectedEventHandler(AgentSocket_Connected);
            mAgentSocket.Disconnected += new Server.DisconnectedEventHandler(AgentSocket_Disconnected);
            mAgentSocket.Kicked += new Server.KickedEventHandler(AgentSocket_Kicked);
            mAgentSocket.PacketReceived += new Server.PacketReceivedEventHandler(AgentSocket_PacketReceived);
        }

        #region ClientSocket EventHandlers

        private static void ClientSocket_PacketReceived(Packet packet)
        {
            byte[] packet_bytes = packet.GetBytes();

                Console.WriteLine("[C->P][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", packet.Opcode, packet_bytes.Length, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, Utility.HexDump(packet_bytes), Environment.NewLine);
            
                //------>>> SEND TO LOGICAL PACKET HANDLER for ClientPackets
            //Bot.PacketHandler.HandleClientPacket(packet);

            if (isConnectedToAgentServer)
            {
                
                if (packet.Opcode == 0x6100)
                {
                    return;
                }
                if (packet.Opcode == 0x6103)
                {
                    return;
                }

                mAgentSocket.Send(packet);
            }
            else
            {
                mGatewaySocket.Send(packet);
            }
        }
        private static void Send6103(string ID6103, string PW6103)
        {
            Packet login = new Packet((ushort)0x6103);
            login.WriteUInt32(sessionID);
            login.WriteAscii(ID6103);
            login.WriteAscii(PW6103);
            login.WriteUInt64(Proxy.SRLocal);
            Proxy.SendAgent(login);
        }
        private static void ClientSocket_Disconnected()
        {
            //Secures reconnect for Client
            mClientConnected = false;
            if (isConnectedToAgentServer)
            {
                isConnectedToAgentServer = false;
                doAgentServerConnect = false;
                mAgentSocket.Disconnect();
            }
            else
            {
                mGatewaySocket.Disconnect();
            }
        }

        private static void ClientSocket_Connected()
        {
            //Connects the Proxy to the right Server.
            mClientConnected = true;
            if (doAgentServerConnect)
            {
                mAgentSocket.Connect(agentServerIP, agentServerPort);
                mGatewaySocket.Disconnect();
            }
            else
            {
                mGatewaySocket.Connect(mGatewayIP, 15779);
            }

        }

        #endregion

        #region GatewaySocket EventHandlers

        /// <summary>
        /// Switches the Gatewayserver packets to the Client
        /// </summary>
        /// <param name="packet"></param>
        private static void GatewaySocket_PacketReceived(Packet packet)
        {
            byte[] packet_bytes = packet.GetBytes();
            Console.WriteLine("[S->P][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", packet.Opcode, packet_bytes.Length, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, Utility.HexDump(packet_bytes), Environment.NewLine);
            #region Client/Clientless Support
            #region Clientless
            //Clientless
            if (!mClientConnected)
            {
                //Request Patches
                if (packet.Opcode == 0x2001)
                {
                    //Generate the Patchverification
                    Packet response = new Packet(0x6100, true);
                    response.WriteByte(SRLocal);
                    response.WriteAscii("SR_Client"); //ServiceName
                    response.WriteUInt(SRVersion);

                    mGatewaySocket.Send(response);
                }

               //Request the Serverlist.
                if (packet.Opcode == 0xA100)
                {
                    byte errorCode = packet.ReadByte();
                    if (errorCode == 1) //Success
                    {
                        Packet response = new Packet(0x6101, true);
                        mGatewaySocket.Send(response);
                    }
                    else
                    {
                        Console.WriteLine("There is an update or you are using an invalid silkroad version.");
                    }
                }
                //Reconnect to AgentServer on successfull login.
                if (packet.Opcode == 0xA102)
                {
                    byte result = packet.ReadByte();
                    if (result == 1)
                    {

                        sessionID = packet.ReadUInt();
                        agentServerIP = packet.ReadAscii();
                        agentServerPort = packet.ReadUShort();                   
                        mGatewaySocket.Disconnect();
                        mAgentSocket.Connect(agentServerIP, agentServerPort);
                        loginSuccess = true;
                        if (isUsingBotLogin)
                        {
                            Send6103(ID, PW);
                            //Passcode
                            Packet p = new Packet(Opcodes.Opcode.LOGIN_CLIENT_SECONDARY_PASSCODE);
                            p.WriteUInt8(2);
                            p.WriteAscii(Passcode);
                            Proxy.SendAgent(p);                            
                        }
                    }
                    else
                    {
                        Console.WriteLine("Wrong Login ID/PW");
                    }
                }

            }
            #endregion
            #region Client
            else //Client
            {
                if (packet.Opcode == (ushort)Opcodes.Opcode.LOGIN_SERVER_LIST)
                {
                    Login.HandleServerList(packet);
                }
                //Redirect the Client
                if (packet.Opcode == 0xA102)
                {
                    byte result = packet.ReadByte();
                    if (result == 1)
                    {
                        sessionID = packet.ReadUInt();
                        agentServerIP = packet.ReadAscii();
                        agentServerPort = packet.ReadUShort();
                        doAgentServerConnect = true;
                        //Create fake response for Client to redirect to localIP/localPort
                        Packet response = new Packet(0xA102, true);
                        response.WriteByte(result);
                        response.WriteUInt(sessionID);
                        response.WriteAscii("127.0.0.1");
                        response.WriteUShort(mLocalPort);
                        loginSuccess = true;
                        //Override the orginal packet with the fake.
                        packet = response;
                    }
                }
            }
            #endregion
            #endregion

            //------>>> SEND TO LOGICAL PACKET HANDLER FOR GatewayPackets
            //Bot.PacketHandler.HandleGatewayPacket(packet);
            
                mClientSocket.Send(packet);
            
            
        }
        public static void PerformLogin(string IDlogin, string PWlogin, string passcodefrmMain, string charnamelogin)
        {
            try
            {
                //ID - PW
                ID = IDlogin;
                PW = PWlogin;
                Packet login = new Packet(Opcodes.Opcode.LOGIN_CLIENT_AUTH);
                login.WriteUInt8(Proxy.SRLocal);//isro locale
                login.WriteAscii(ID);
                login.WriteAscii(PW);
                login.WriteAscii("");//mobile vertification code
                login.WriteUInt8(0xFF);
                login.WriteUInt16(267); // Server
                Proxy.SendGateway(login);
                isUsingBotLogin = true;
                Passcode = passcodefrmMain;
                charname = charnamelogin;                                    
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            
        }

        private static void GatewaySocket_Kicked()
        {
            Console.WriteLine("GatewaySocket_Kicked");
            mGatewaySocket.Disconnect();
        }

        private static void GatewaySocket_Disconnected()
        {
            Console.WriteLine("GatewaySocket_Disconnected");
        }

        private static void GatewaySocket_Connected(string ip, ushort port)
        {
            Console.WriteLine("GatewaySocket connected to " + ip + ":" + port);
        }

        #endregion

        #region AgentSocket EventHandlers

        /// <summary>
        /// Switches the Agentserver packets to the Client
        /// </summary>
        /// <param name="packet"></param>
        private static void AgentSocket_PacketReceived(Packet packet)
        {
            byte[] packet_bytes = packet.GetBytes();
            Console.WriteLine("[S->P][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", packet.Opcode, packet_bytes.Length, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, Utility.HexDump(packet_bytes), Environment.NewLine);
            switch (packet.Opcode)
            {
                case (ushort)Opcodes.Opcode.SERVER_CHARACTER:
                    {
                           //Login.HandleCharList(packet);
   /*                        #region Character Listing
                            byte check = packet.ReadUInt8();
                            if (check == 2)
                            {
                                if (packet.ReadUInt8() == 1)
                                {
                                    byte charcount = packet.ReadUInt8();
                                    for (int i = 0; i < charcount; i++)
                                    {
                                        packet.ReadUInt32();// charactertype
                                        string name = packet.ReadAscii();

                                        ushort volume = packet.ReadUInt8();
                                        packet.ReadByte();
                                        packet.ReadByte();
                                        ushort level = packet.ReadUInt8();
                                        packet.ReadUInt64();//exp shit.
                                        ushort str = packet.ReadUInt16();
                                        ushort INT = packet.ReadUInt16();
                                        ushort sp = packet.ReadUInt16();
                                        uint hp = packet.ReadUInt32();
                                        uint mp = packet.ReadUInt32();
                                        //uint currenthp = packet.ReadUInt32();
                                        //uint currentmp = packet.ReadUInt32();// do u have edxLoader ? maybe .. let me check
                                        if (packet.ReadUInt8() == 1)// deletion shit checking for the if the character deleting
                                        {
                                            uint time = packet.ReadUInt32();
                                        }
                                        //Working Untill here i dont have all these items :D so it gives wrong lol :D
                                        ushort itemscount = packet.ReadUInt16();
                                        for (int x = 0; x < itemscount; x++)
                                        {
                                            packet.ReadUInt32();
                                            packet.ReadByte();
                                        }
                                        ushort avataritemcount = packet.ReadUInt8();
                                        for (int x = 0; x < avataritemcount; x++)
                                        {
                                            packet.ReadUInt32();
                                            packet.ReadByte();
                                        }
                                    }
                                }
                            }
                            #endregion  */
                            #region Character Selection if Login through BOT
                            if (charname != "") // this is the choose character thing...  i see
                            {
                                Packet p1 = new Packet(0x7001);
                                p1.WriteAscii(charname);
                                Proxy.SendAgent(p1);
                            }
                            #endregion

                        break;
                    }


                case (ushort)Opcodes.Opcode.SERVER_CHARDATA:
                    {
                        Login.Server_CharacterData(packet);
                        break;
                    }


                case (ushort)Opcodes.Opcode.SERVER_CHAR_ID:
                    {
                        uint CharID = packet.ReadUInt32();
                        Global.Player.General.UniqueID = CharID;
                        Console.WriteLine("CharID: " + CharID);
                        break;
                    }


                case (ushort)Opcodes.Opcode.SERVER_LOGIN_RESULT:
                    {
                #region Character Selection when login success
                
                        if (!mClientConnected)
                        {
                            byte result = packet.ReadByte();
                            if (result == 1)
                            {
                                if (charname != "")
                                {
                                    Packet p1 = new Packet(0x7001);
                                    p1.WriteAscii(charname);
                                    Proxy.SendAgent(p1);
                                }
                            }
                        }
                        break;
                    
                #endregion
                    }


                #region Character Moving
                case (ushort)Opcodes.Opcode.SERVER_MOVEMENT:
                    {
                        Movement.CharacterMoving(packet);
                        break;
                    }
                #endregion


                #region Passcode Login
                case (ushort)Opcodes.Opcode.LOGIN_SERVER_RESULT:
                    {
                        byte result = packet.ReadByte();
                        if (result == 1 && Passcode != "")
                        {
                            Packet p = new Packet(Opcodes.Opcode.LOGIN_CLIENT_SECONDARY_PASSCODE);
                            p.WriteUInt8(2);
                            p.WriteAscii(Passcode);
                            Proxy.SendAgent(p);
                        }
                        break;
                    }
                #endregion

            }
            mClientSocket.Send(packet);
        }

        private static void AgentSocket_Kicked()
        {
            //When logging out.
            isConnectedToAgentServer = false;
            doAgentServerConnect = false;
            mAgentSocket.Disconnect();
        }

        private static void AgentSocket_Disconnected()
        {
            Console.WriteLine("AgentSocket_Disconnected");
        }

        private static void AgentSocket_Connected(string ip, ushort port)
        {
            isConnectedToAgentServer = true;
            Console.WriteLine("AgentSocket connected to " + ip + ":" + port);
            if (isUsingBotLogin && loginSuccess)
            {
                //Client 6103
                Send6103(ID, PW);
            }
        }

        #endregion

        public static void PerformClientless()
        {
            mClientConnected = false;
            
            mGatewaySocket.Connect("121.128.133.28", 15779);
        }

        public static void SendAgent(Packet packet)
        {
            try
            {
                mAgentSocket.Send(packet);
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem sending AgentServer Packet.");
            }
        }

        public static void SendGateway(Packet packet)
        {
            try
            {
                mGatewaySocket.Send(packet);
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem sending GatewayServer Packet.");
            }
        }

    }
}
