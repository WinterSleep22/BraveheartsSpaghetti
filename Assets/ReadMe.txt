


The Main Scene has monobehaviour inherenting from Menu class that implements hide,load game and public functions for UI buttons.
The game has 2 implementations of network, using UNET and Photon Pun. Unet is for local LAN games only. 
PersistentCardGame class implements 'DB' funcionalities.
In The GameScene, the main class is Gameplay which initialize all things. This class keep the rhytm of the game using TurnManager and GameMechanics.
GameMechanics implements functions that should run locally.
PlayerNetwork represents all remote actions that a player can do.
PlayerInputControl class serves as interface for AI , for UI buttons and network to remotely play as opponent.
Board class has card slots which can contains cards and functionalities of gameplay like callbacks,events and change status of the game.
LogTextUI shows important messages that players needs to be notified.
Action represents a remote action, also implements the remote action using playerinputcontrol class.
CardManager keep reference of all instantiated cards in the game. Contains funtions that handle them.
