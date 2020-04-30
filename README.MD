# FIESTA
*Free-range Immersive Environment to Support Team-based Analysis*

FIESTA is a multiplayer Unity program that allows users to collaboratively visually analyse data in a shared virtual/augmented reality environment. It can support an arbitrary number of users and works in both co-located and remote settings.

It was developed as a research prototype in the paper *Shared Surfaces and Spaces: Collaborative Data Visualisation in a Co-located Immersive Environment*.

## Requirements
To properly run FIESTA, you will need to have installed:
- Unity 2018.3.6f1
- SteamVR
- Photon Server (for self-hosted server)

This repository currently contains most necessary 3rd party plugins and files to run:
- IATK
- VRTK v3.3.0
- SteamVR v1.2.3
- Oculus Integration
- PUN v2
- DOTween

## How to run
### Setting up networking
As FIESTA is a multiplayer program, clients must connect to a server. We use the [Photon Unity Networking engine](https://www.photonengine.com/en-US/Photon) for this. 
You can either use the cloud servers hosted by Exit Games themselves (free up to 20 CCUs) or host your own one locally. The former requires less setup time, but the latter is less prone to latency and bandwidth restrictions and therefore is recommended.

In either case, you will need an AppId (API key) to use Photon. You get one for free by signing up on their [website](https://www.photonengine.com/en-US/Photon), and creating a new Photon PUN or RealTime app. You can then enter this key into Unity via Window > Photon Unity Networking > PUN Wizard and choosing Setup Project. In order for multiple clients to connect to the same server, all of them need to share the same API key.

If you want to host your own local server, refer to [this guide](https://doc.photonengine.com/en-us/server/current/getting-started/photon-server-in-5min) for instructions. You will need to enter the IP address of the server in the inspector via Window > Photon Unity Networking > Highlight Server Settings. Make sure "Use Name Server" is unchecked.

If you want to use a cloud server, check "Use Name Server" instead.

### Launching FIESTA
There are three main scenes of note when launching FIESTA:
* Start with StartupScene if running from a standalone build
* Start with ConnectingScene if running from the Editor and using VR (with IP addresses already set up in Photon's server settings)
* Start with NonVR_ConnectingScene if running from the Editor and not using VR (with IP addresses already set up in Photon's server settings)

### Scenes in FIESTA
FIESTA swaps between scenes in order to properly load players and their required objects. The order in which these scenes load are:
1. StartupScene
	* This assists with specifying the correct IP address to connect to, particularly for when FIESTA is being launched from a standalone build and not from the Editor
	* Clicking "Connect" will connect to the specified IP address. If left blank, it will default to localhost.
	* Clicking "Connect to Cloud" will instead connect to a cloud server, irrespective of what was entered in the IP address field
	* Checking "Connect as spectator (non VR)" modifies which scene is launched when connected to the server. When unchecked, the standard VR-enabled scene is launched (ConnectingScene). When checked, a separate desktop-only scene is launched (NonVR_ConnectingScene).
2. *(NonVR_)* ConnectingScene
	* This acts as the "lobby" in which players connect to before loading into the actual scene
	* The player's name and colour of their avatar can be customised with the keyboard and mouse
	* ConnectingScene will enable an avatar controlled in VR, NonVR_ConnectingScene will enable an avatar controlled using the keyboard and mouse
	* The game can then be started either by clicking on the "Start" button on-screen, or by grabbing onto the floating "Start" button
3. MainScene
	* This is where the actual data visualisation functionality of FIESTA exists
	* All important objects are simultaneously loaded for all players who were in the ConnectingScene

### Notes on running
There are some things to bear in mind about how FIESTA functions:
* It does not support users joining and leaving mid-game, therefore ensure that all users are connected before starting
* With enough users and/or a higher defined server tickrate, PUN may force disconnect a user if network traffic is too high to a cloud server. The easy solution is to use a private locally hosted server instead

## Setting up co-located environment
FIESTA can be used in both co-located and remote collaborative contexts. While remote collaboration is easier to setup, co-located collaboration requires that the VR tracking system is properly synchronised across all users to prevent accidental collisions.

There are different steps that need to be done depending on the setup you are using. This section assumes you are using VR backpacks with HTC Vives for tetherless VR.

##### VR backpacks
You will need to establish a remote desktop connection to each of the VR backpacks to properly start/stop/update FIESTA. You can use any software you prefer, such as TeamViewer, AnyDesk, or Window's Remote Desktop Connection.

You may notice certain programs, particularly those which rely on the GPU such as Unity, do not properly show up in the remote viewer. In this circumstance, you will need to connect a monitor to the backpack for Windows to begin running the GPU. To circumvent this, a HDMI/DisplayPort dummy plug (display emulator) can be connected to trick Windows into thinking there is a monitor plugged in, resolving the issue.

To minimise latency for both the remote desktop and FIESTA, it is highly recommended to connect all backpacks onto the same private network.

##### SteamVR room sync
It is preferable to use VR headsets using outside-in tracking as it inherently provides an accurate external frame of reference to any headset in the environment. The more tracking basestations that are being used the better, as it minimises accidental occlusion to the basestations due to multiple users. This essentially limits your options to using SteamVR 2.0 basestations with either the HTC Vive Pro or the Valve Index.

To synchronise positions of players and objects in FIESTA, we need to use the same room setup and configuration across all PCs. Begin by performing the SteamVR room setup process on one of the PCs. If any other PC hasn't run the first time setup yet, it is recommended that you do so as well. Then, copy and paste the following from the PC you ran the room setup with to all other PCs:

1. Navigate to \<steam install directory>\\config
2. Copy over chaperone_info.vrchap
3. Navigate to \<steam install directory>\config\lighthouse
4. Copy over lighthousedb.json

## Data Replayer
We used FIESTA to conduct a user study in our paper *Shared Surfaces and Spaces: Collaborative Data Visualisation in a Co-located Immersive Environment*. While we are unable to publish the video footage due to participant names being visible, we provide a means to "replay" the events of each group and task in the study using Unity.

### How to run
Other than having the FIESTA source code and Unity Editor, you will need the following:
* A Photon API key to properly see visualisation objects. Follow same instructions listed above
* The original log files, which you can download [here](https://drive.google.com/file/d/1pitxd03uAmUEDYTKQqUfPNuN3cXQjYe_/view?usp=sharing). When downloaded, extract the Logs folder into the FIESTA project's Assets folder without renaming any files. The final directory should be "FIESTA/Assets/Logs/". Note that you do not need to do anything else, as the scripts will find the required files at runtime

Once set up, you can watch the replay as follows:
1. On the Unity Editor, open the "DataWranglingScene" scene
2. Press the Play button to run the scene
3. Click on the "DataReplayer" GameObject in the Hierarchy
4. In the Inspector, select which study, group, and question (task) you want to view. Note that in this case, Groups 6 to 10 are accessed via Study 2, Groups 1 to 5 instead
5. Click on "Replay Data" at the bottom of the Inspector to start replaying the events in the study
	* To adjust replay speed, drag the "Replay Speed" slider
	* To pause/resume the replay , click on "Pause/Unpause Replay"
	* To reset the entire replay, click on "Clear Scene"

As the events are resimulated in Unity, you can pan and move the Scene camera any way you wish during the replay.
