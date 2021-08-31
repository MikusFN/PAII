namespace Assets.Network
{
    /// <summary>
    /// This enumerator holds information for all possible messages to be sent. Please add more.
    /// </summary>
    public enum MessageType : byte
    {
        ClientSendData,     // Send Client-class basic data from Server To Client                                       
                            // Client-class
        ClientReady,        // Tell the server the player is ready.
        ClientUnready,      // Tells the server the player is not ready.
        ClientUIUpdate,     // Sends the client data of the player to the Server, to update the name and color UI.          
                            // Client-class
        ClientJoined,       // Sends to all clients basic data about the player that just joined.                       
                            // List<ShareableClient>-class
        ClientLeft,         // Sends toa all client that a player just left.                                            
                            // ShareableClient-class
        GameStart,          // Tells the server the game should start.
        GameStarting,       // Tells the clients the game is starting.
        GameEnd,            // Tells the clients the game ended (with or without victory).
        TimesUp,            // Tells the client that the time has finished. (Maybe add some victory message with this)

        PlayerHoldsCard,    //Tells the server a player is holding a card, ready to play.
        PlayerDroppedCard,  //Tells the sever the player didn't play the card.
        PlayCard,           // Sends the server an ID to play a card.                                                    
                            // short
        PlayDiseaseCard,    //Sends the server an ID and a Vector2.
                            //DiseaseCardWrapper
        PlayMentalDiseaseCard,
                            //Sends the server a Card ID and a PlayerID.
                            //MentalDiseaseCardWrapper
        DrawCards,          // Draw one or multiple cards. Uses a byte to tell how many cards to draw. And use a Short to get what card to load.
        DrawSpecificCard,   //Draw one spcific card from the server, use cardID to ask for the card. Use a String for the id

        ClientSendText,      //It send a message TO THE SERVER with a message to resend to other player
        SetAsFirstPlayer,    //Ît set the btn for the first layer to use
    
        SetMoney,           //Sets Player Money in the client.
                            //Float
        SetReputation,      //Sets the Player Reputation in the client.
                            //ushort
        SetMentalDisease,   //Sets the Player with a mental disease.
                            //PerceptionEnum
        SetDiseases,        //Sets the ShareableDiseases in the android.
                            //List<ShareableDisease>
        AcusingAPlayer,      //Send two strings to acuse a player
        AskForVote,          //SErver ask for vote, sends a player id and a deases id
        DecsionVote,         //Clients send a bool with the answer
        ResultsOfVote,       //SErve gives or takes reputation and money



    }
}