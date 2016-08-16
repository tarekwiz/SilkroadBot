using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System;
using Silkroad_Fusion.API;

namespace Silkroad_Fusion
{
    public class Server
    {

        #region Events

        public delegate void ConnectedEventHandler(string ip, ushort port);
        public event ConnectedEventHandler Connected;

        public delegate void DisconnectedEventHandler();
        public event DisconnectedEventHandler Disconnected;

        // Not sure about this
        public delegate void KickedEventHandler();
        public event KickedEventHandler Kicked;

        public delegate void PacketReceivedEventHandler(Packet packet);
        public event PacketReceivedEventHandler PacketReceived;

        #endregion

        #region Fields

        Socket remote_socket;

        Security remote_security;

        //Used for Transfare and Processing
        TransferBuffer remote_recv_buffer;
        List<Packet> remote_recv_packets;
        List<KeyValuePair<TransferBuffer, Packet>> remote_send_buffers;

        Thread thPacketProcessor;

        //Used to Provide secure connect/disconnect while changing from Gateway to Agent
        bool isClosing;
        bool doPacketProcess;

        #endregion

        public void Connect(string ip, ushort port)
        {
            if (remote_socket != null)
            {
                Disconnect();
            }

            //Create objects 
            remote_recv_buffer = new TransferBuffer(8192, 0, 0);
            remote_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Thread Management
            if (thPacketProcessor == null)
            {
                thPacketProcessor = new Thread(ThreadedPacketProcessing);
                thPacketProcessor.Name = "Proxy.Network.Server.PacketProcessor";
                thPacketProcessor.IsBackground = true;
                thPacketProcessor.Start();
            }

            try
            {
                //Recreate the Security
                remote_security = new Security();

                //Connect
                remote_socket.Connect(ip, port);

                if (remote_socket.Connected)
                {
                    if (Connected != null)
                    {
                        Connected(ip, port);
                    }
                    doPacketProcess = true;
                    remote_socket.BeginReceive(remote_recv_buffer.Buffer, 0, 8192, SocketFlags.None, new AsyncCallback(WaitForServerData), remote_socket);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Disconnect()
        {
            doPacketProcess = false;
            isClosing = true;

            if (remote_socket != null)
            {
                if (remote_socket.Connected)
                {
                    remote_socket.Shutdown(SocketShutdown.Both);
                }
                remote_socket.Close();
                remote_socket = null;
            }

            isClosing = false;
        }

        private void WaitForServerData(IAsyncResult ar)
        {
            if (!isClosing && doPacketProcess) //if not closing and also packetprocessing
            {
                Socket worker = null;
                try
                {
                    worker = (Socket)ar.AsyncState;
                    int rcvdBytes = worker.EndReceive(ar);
                    if (rcvdBytes > 0)
                    {
                        remote_recv_buffer.Size = rcvdBytes;
                        remote_security.Recv(remote_recv_buffer);
                    }
                    else
                    {
                        //RaiseEvent
                        if (Kicked != null)
                        {
                            Kicked();
                            worker = null;
                        }
                        else
                        {
                            Console.WriteLine("You have been kicked by the Security Software.");
                        }

                    }
                }
                catch (SocketException se)
                {
                    if (se.SocketErrorCode == SocketError.ConnectionReset) //Disconnected
                    {
                        Console.WriteLine("You have been disconnected from the Server.");

                        //RaiseEvent
                        if (Disconnected != null)
                        {
                            Disconnected();
                        }

                        //Mark worker as null to stop reciving
                        worker = null;
                    }
                    else
                    {
                        Console.WriteLine(se.ErrorCode);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (worker != null)
                    {
                        worker.BeginReceive(remote_recv_buffer.Buffer, 0, 8192, SocketFlags.None, new AsyncCallback(WaitForServerData), worker);
                    }
                }
            }
        }

        private void Send(byte[] buffer)
        {
            if (!isClosing && doPacketProcess) //if not closing and also packetprocessing
            {
                if (remote_socket.Connected)
                {
                    try
                    {
                        remote_socket.Send(buffer);
                        if (buffer.Length == 0)
                        {
                            Console.WriteLine("buffer.Length == 0");
                        }
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
            remote_security.Send(packet);
        }

        #region PacketProcessor

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
                _processServerPackets();
                Thread.Sleep(1);
            }

            if (isClosing)
            {
                return; //Jump out.
            }

            goto begin; //goto begin and wait until we should process again
        }

        private void _processServerPackets()
        {
            if (!isClosing && doPacketProcess)
            {
                #region Handle Recive

                remote_recv_packets = remote_security.TransferIncoming();
                if (remote_recv_packets != null)
                {
                    foreach (var packet in remote_recv_packets)
                    {
                        if (packet.Opcode == 0x5000 || packet.Opcode == 0x9000)
                        {
                            continue;
                        }

                        //RaiseEvent if event is assigned.
                        if (PacketReceived != null)
                        {
                            PacketReceived(packet);
                        }
                    }
                }
                #endregion

                #region Handle Send

                remote_send_buffers = remote_security.TransferOutgoing();
                if (remote_send_buffers != null)
                {
                    foreach (var buffer in remote_send_buffers)
                    {
                        byte[] packet_bytes = buffer.Value.GetBytes();

                            Console.WriteLine("[P->S][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", buffer.Value.Opcode.ToString("X4"), packet_bytes.Length, buffer.Value.Encrypted ? "[Encrypted]" : "", buffer.Value.Massive ? "[Massive]" : "", Environment.NewLine, Utility.HexDump(packet_bytes), Environment.NewLine);
                            Send(buffer.Key.Buffer);
                    }
                }

                #endregion

            }
        }

        #endregion

    }
}
