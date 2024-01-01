namespace API;


// we also need to receive the message from the clients. 
public class CreateMessageDto
{
    public string RecipientUsername { get; set; }

    public string Content { get; set; }
}
