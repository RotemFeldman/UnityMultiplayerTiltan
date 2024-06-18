using DefaultNamespace;
using UnityEngine;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;

public class RoomListMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform _content;
    [SerializeField] private RoomListing _roomListing;

    private List<RoomListing> _listings = new List<RoomListing>();


    public override void OnJoinedLobby()
    {

        base.OnJoinedLobby();
        foreach (var listing in _listings)
        {
            Destroy(listing.gameObject);
        }
        _listings.Clear();
    }
    

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        
        foreach (var info in roomList)
        {
            if (info.RemovedFromList)
            {
                Debug.Log("Removed from list!!!!!!!");
                
                int index = _listings.FindIndex(x => x.Info.Name == info.Name);
                if (index != -1)
                {
                    Destroy((_listings[index].gameObject));
                    _listings.RemoveAt(index);
                }
            }
            else 
            {
                
                int index = _listings.FindIndex(x => x.Info.Name == info.Name);
                
                if (index != -1) // room exist - than update
                {
                    _listings[index].SetRoomInfo(info);

                    if (info.PlayerCount == 0)
                    {
                        Destroy((_listings[index].gameObject));
                        _listings.RemoveAt(index);
                    }
                        
                }
                else // room doesnt exist - create new listing
                {
                    RoomListing listing = Instantiate(_roomListing, _content);
                    if (listing != null)
                    {
                        listing.SetRoomInfo(info);
                        _listings.Add(listing);
                    }
                }
            }
            
            

        }
    }


}
