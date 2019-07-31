using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class PhotonAvatarView : MonoBehaviour, IPunObservable
{

    private PhotonView photonView;
    private OvrAvatar ovrAvatar;
    private OvrAvatarRemoteDriver remoteDriver;

    private List<byte[]> packetData;
    
    public void Start()
    {
        photonView = GetComponent<PhotonView>();
        packetData = new List<byte[]>();

        //if (GetComponent<CustomAvatarLocalDriver>() != null)
        if (photonView.IsMine)
        {
            ovrAvatar = GetComponent<OvrAvatar>();
            ovrAvatar.RecordPackets = true;
            ovrAvatar.PacketRecorded += OnLocalAvatarPacketRecorded;
        }
        else
        {
            remoteDriver = GetComponent<OvrAvatarRemoteDriver>();
        }
    }

    public void OnDisable()
    {
        if (photonView.IsMine)
        {
            ovrAvatar.RecordPackets = false;
            ovrAvatar.PacketRecorded -= OnLocalAvatarPacketRecorded;
        }
    }

    private int localSequence;

    public void OnLocalAvatarPacketRecorded(object sender, OvrAvatar.PacketEventArgs args)
    {
        if (!PhotonNetwork.InRoom || (PhotonNetwork.CurrentRoom.PlayerCount < 2))
        {
            return;
        }

        using (MemoryStream outputStream = new MemoryStream())
        {
            BinaryWriter writer = new BinaryWriter(outputStream);

            var size = Oculus.Avatar.CAPI.ovrAvatarPacket_GetSize(args.Packet.ovrNativePacket);
            byte[] data = new byte[size];
            Oculus.Avatar.CAPI.ovrAvatarPacket_Write(args.Packet.ovrNativePacket, size, data);

            writer.Write(localSequence++);
            writer.Write(size);
            writer.Write(data);

            packetData.Add(outputStream.ToArray());
        }
    }

    private void DeserializeAndQueuePacketData(byte[] data)
    {
        using (MemoryStream inputStream = new MemoryStream(data))
        {
            BinaryReader reader = new BinaryReader(inputStream);
            int remoteSequence = reader.ReadInt32();

            int size = reader.ReadInt32();
            byte[] sdkData = reader.ReadBytes(size);

            System.IntPtr packet = Oculus.Avatar.CAPI.ovrAvatarPacket_Read((System.UInt32)data.Length, sdkData);
            remoteDriver.QueuePacket(remoteSequence, new OvrAvatarPacket { ovrNativePacket = packet });
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (packetData.Count == 0)
            {
                return;
            }

            stream.SendNext(packetData.Count);

            foreach (byte[] b in packetData)
            {
                stream.SendNext(b);
            }

            packetData.Clear();
        }

        if (stream.IsReading)
        {
            int num = (int)stream.ReceiveNext();

            for (int counter = 0; counter < num; ++counter)
            {
                byte[] data = (byte[])stream.ReceiveNext();

                DeserializeAndQueuePacketData(data);
            }
        }
    }
}
