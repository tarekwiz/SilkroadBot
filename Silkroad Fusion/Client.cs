using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Silkroad_Fusion.API;

namespace Silkroad_Fusion
{
    
        public class Client
        {

            #region Events

            public delegate void PacketReceivedEventHandler(Packet packet);
            public event PacketReceivedEventHandler PacketReceived;

            public delegate void ConnectedEventHandler();
            public event ConnectedEventHandler Connected;

            public delegate void DisconnectedEventHandler();
            public event DisconnectedEventHandler Disconnected;

            #endregion

            #region Fields

            Socket local_listener;
            Socket local_socket;

            Security local_security;

            TransferBuffer local_recv_buffer;
            List<Packet> local_recv_packets;
            List<KeyValuePair<TransferBuffer, Packet>> local_send_buffers;

            Thread thPacketProcessor;
            bool isClosing;
            bool doPacketProcess;

            #endregion

            //Listen for ClientConnection on port
            public void Listen(ushort port)
            {
                local_security = new Security();
                local_security.GenerateSecurity(true, true, true);

                local_recv_buffer = new TransferBuffer(8192, 0, 0);
                local_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                local_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //Thread Management
                thPacketProcessor = new Thread(ThreadedPacketProcessing);
                thPacketProcessor.Name = "Proxy.Network.Client.PacketProcessor";
                thPacketProcessor.IsBackground = true;
                thPacketProcessor.Start();

                try
                {
                    if (local_listener.IsBound == false)
                    {
                        local_listener.Bind(new IPEndPoint(IPAddress.Loopback, port));
                        local_listener.Listen(1);
                    }
                    local_listener.BeginAccept(new AsyncCallback(OnClientConnect), null);
                    Console.WriteLine("Listing for Client on 127.0.0.1:" + port);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            private void Listen()
            {
                doPacketProcess = false;

                if (local_socket != null)
                {
                    local_socket.Shutdown(SocketShutdown.Both);
                    local_socket.Close();
                }

                local_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //Accept a new Client.
                local_listener.BeginAccept(new AsyncCallback(OnClientConnect), null);
            }

            public void Shutdown()
            {
                doPacketProcess = false;
                isClosing = true;

                thPacketProcessor.Join();

                //Close Socket
                if (local_socket != null)
                {
                    local_socket.Shutdown(SocketShutdown.Both);
                    local_socket.Close();
                }
                local_socket = null;

                //Close listener
                if (local_listener != null)
                {
                    local_listener.Close();
                    local_listener = null;
                }

                local_security = null;

                local_recv_buffer = null;
                local_recv_packets = null;
                local_send_buffers = null;

                if (thPacketProcessor != null)
                {
                    thPacketProcessor = null;
                }
            }

            private void OnClientConnect(IAsyncResult ar)
            {
                if (!isClosing)
                {
                    try
                    {
                        doPacketProcess = true;
                        local_socket = local_listener.EndAccept(ar);
                        local_socket.BeginReceive(local_recv_buffer.Buffer, 0, 8192, SocketFlags.None, new AsyncCallback(WaitForData), local_socket);

                        local_security = new Security();
                        local_security.GenerateSecurity(false, false, false);

                        //RaiseEvent if event is linked
                        if (Connected != null)
                        {
                            Connected();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw (new Exception("Network.Client.OnClientConnect: " + ex.Message, ex));
                    }
                }
            }

            private void WaitForData(IAsyncResult ar)
            {
                if (!isClosing && doPacketProcess)
                {
                    Socket worker = null;
                    try
                    {
                        worker = (Socket)ar.AsyncState;
                        int rcvdBytes = worker.EndReceive(ar);
                        if (rcvdBytes > 0)
                        {
                            local_recv_buffer.Size = rcvdBytes;
                            local_security.Recv(local_recv_buffer);
                        }
                        else
                        {
                            Console.WriteLine("Client Disconnected");
                            //RaisEvent
                            if (Disconnected != null)
                            {
                                Disconnected();
                            }
                            Listen();
                        }
                    }
                    catch (SocketException se)
                    {
                        if (se.SocketErrorCode == SocketError.ConnectionReset) //Client Disconnected
                        {
                            Console.WriteLine("Client Disconnected");
                            //RaisEvent
                            if (Disconnected != null)
                            {
                                Disconnected();
                            }
                            Listen();

                            //Mark worker as null to stop reciveing.
                            worker = null;
                        }
                        else
                        {
                            throw (new Exception("Proxy.Network.Client.WaitForData: " + se.Message, se));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw (new Exception("Proxy.Network.Client.WaitForData: " + ex.Message, ex));
                    }
                    finally
                    {
                        if (worker != null)
                        {
                            worker.BeginReceive(local_recv_buffer.Buffer, 0, 8192, SocketFlags.None, new AsyncCallback(WaitForData), worker);
                        }
                    }
                }
            }

            private void Send(byte[] buffer)
            {
                if (!isClosing && doPacketProcess) //if not closing and also packetprocessing
                {
                    if (local_socket.Connected)
                    {
                        try
                        {
                            local_socket.Send(buffer);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
            public void Send(Packet packet)
            {              
                local_security.Send(packet);
            }

            private void ThreadedPacketProcessing()
            {
            begin:

                //Wait until we should process packets
                while (!doPacketProcess && !isClosing)
                {
                    Thread.Sleep(1);
                }

                while (doPacketProcess && !isClosing)
                {
                    _processClientPackets();
                    Thread.Sleep(1);
                }

                if (isClosing)
                {
                    return; //Jump out.
                }

                goto begin; //goto begin and wait until we should process again
            }
            private void _processClientPackets()
            {
                if (!isClosing && doPacketProcess)
                {
                    local_recv_packets = local_security.TransferIncoming();
                    if (local_recv_packets != null)
                    {
                        foreach (var packet in local_recv_packets)
                        {
                            if (packet.Opcode == 0x5000 || packet.Opcode == 0x9000 || packet.Opcode == 0x2001)
                            {
                                continue;
                            }

                            //Send to PacketHandler
                            if (PacketReceived != null)
                            {
                                PacketReceived(packet);
                            }
                        }
                    }

                    local_send_buffers = local_security.TransferOutgoing();
                    if (local_send_buffers != null)
                    {
                        foreach (var buffer in local_send_buffers)
                        {
                            byte[] packet_bytes = buffer.Value.GetBytes();

                                //Console.WriteLine("[P->C][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", buffer.Value.Opcode.ToString("X4"), packet_bytes.Length, buffer.Value.Encrypted ? "[Encrypted]" : "", buffer.Value.Massive ? "[Massive]" : "", Environment.NewLine, Utility.HexDump(packet_bytes), Environment.NewLine);
                                Send(buffer.Key.Buffer);
                        }
                    }
                }
            }
        }
    
}
