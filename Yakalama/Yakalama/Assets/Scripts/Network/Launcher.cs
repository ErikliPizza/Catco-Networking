using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
namespace Com.NoirContactDT.Cady
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;
        #endregion
        
        #region Private Fields
        string gameVersion = "1"; //Users are separated from each other by gameVersion.
        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;
        bool isConnecting;
        #endregion

        #region monocall
        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        private void Start()
        {
            if (GameObject.FindGameObjectWithTag("controlPanel"))
                controlPanel = GameObject.FindGameObjectWithTag("controlPanel");
            else
                Debug.LogError("controlPanel Bulunamadư");
            
            if (GameObject.FindGameObjectWithTag("progressLabel"))
                progressLabel = GameObject.FindGameObjectWithTag("progressLabel");
            else
                Debug.LogError("progressLabel Bulunamadư");

            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("conn isconnectedi");
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }
        #endregion
        
        #region PunCallbacks Callbacks
        public override void OnConnectedToMaster() // burasư sahne yüklendiđi anda yüklenir.
        {
            if (isConnecting)
            {
                // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            isConnecting = false;
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }
        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the 'Room for 1' ");


                // #Critical
                // Load the Room Level.
                PhotonNetwork.LoadLevel("Room for 1");
            }
        }


        #endregion
    }
}
